using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using iBank.Services.Implementation.Utilities;
using iBank.Services.Implementation.Utilities.ClientData;
using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Models.ReportPrograms.PTARequestActivityReport;
using iBank.Repository.SQL.Repository;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Helpers;
using CODE.Framework.Core.Utilities;
using Domain.Helper;
using Domain.Orm.iBankClientQueries;
using CODE.Framework.Core.Utilities.Extensions;
using iBank.Server.Utilities;
using iBank.Services.Implementation.Shared;
using Domain;

namespace iBank.Services.Implementation.ReportPrograms.PTARequestActivity
{
    public class TravAuthStatusDetRawDataRetriever
    {
        private DateTime? _beginParseDateStamp;
        private DateTime? _endParseDateStamp;
        private readonly ReportRunConditionals _conditionals = new ReportRunConditionals();
        private DataRetriever _retriever;

        private BuildWhere BuildWhere { get; set; }
        private ReportGlobals Globals { get; set; }
        public List<RawData> RawDataList { get; set; }
        public GlobalsCalculator GlobalCalc { get; set; }
        public bool IsReservation { get; set; }
        public double OffsetHours { get; set; }
        public string TimeZoneName { get; set; }

        private IClientDataStore _clientStore;
        private IClientDataStore ClientDataStore
        {
            get
            {
                return _clientStore ?? (_clientStore = new ClientDataStore(Globals.AgencyInformation.ServerName,
                                                                             Globals.AgencyInformation.DatabaseName));
            }
        }
        
        private void InitialSetup()
        {
            var start = Globals.GetParmValue(WhereCriteria.TXTPARSEDTSTART);
            DateTime.TryParse(start, out var startDate);
            _beginParseDateStamp = startDate == DateTime.MinValue ? Constants.ModifiedDateTimeMinValue : startDate;

            var end = Globals.GetParmValue(WhereCriteria.TXTPARSEDTEND);
            DateTime.TryParse(end, out var endDate);
            endDate = endDate == DateTime.MinValue ? Constants.ModifiedDateTimeMinValue : endDate;
            //see if there is a time here. If not, default to midnight. 
            _endParseDateStamp = !end.Contains(":")
               ? endDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59)
               : endDate;
        }

        public bool GetRawData(BuildWhere buildWhere)
        {
            BuildWhere = buildWhere;
            Globals = BuildWhere.ReportGlobals;
            InitialSetup();

            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientDataStore.ClientQueryDb, Globals.Agency);

            //if we apply routing in both directions "SpecRouting" is true. 
            BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: true, buildCarWhere: false, buildHotelWhere: false, buildUdidWhere: true, buildDateWhere: true, inMemory: true, isRoutingBidirectional: false, legDit: true, ignoreTravel: false);

            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }

            BuildWhere.AddSecurityChecks();

            var sqlBuilder = new SqlBuilder();
            _retriever = new DataRetriever(ClientDataStore.ClientQueryDb);

            var udidNumber = Globals.GetParmValue(WhereCriteria.UDIDNBR).TryIntParse(-1);

            var airSql = sqlBuilder.GetSql(udidNumber > 0, BuildWhere, _beginParseDateStamp, _endParseDateStamp, Globals);
            RawDataList = _retriever.GetData<RawData>(airSql, BuildWhere, false, false, IsReservation, includeAllLegs:false, checkForDuplicatesAndRemoveThem:true, handleAdvanceParamsAtReportLevelOnly:true).ToList();

            //get cancelled trips. First get sql again to clear out extra fields. 
            airSql = sqlBuilder.GetSql(udidNumber > 0, BuildWhere, _beginParseDateStamp, _endParseDateStamp, Globals);
            airSql.FromClause = sqlBuilder.GetCancelledTripFromClause(udidNumber);
            
            var delayer = new ReportDelayer(ClientDataStore, new MasterDataStore(), Globals);

            if (BuildWhere.HasRoutingCriteria || BuildWhere.HasFirstDestination || BuildWhere.HasFirstOrigin)
            {
                RawDataList.AddRange(_retriever.GetData<RawData>(airSql, BuildWhere, false, false, IsReservation, true).ToList());
                if (!_conditionals.IsUnderOfflineThreshold(RawDataList.Count, Globals, delayer)) return false;

                var segmentData = Collapser<RawData>.Collapse(RawDataList, Collapser<RawData>.CollapseType.Both);
                segmentData = GlobalCalc.IsAppliedToLegLevelData() ? BuildWhere.ApplyWhereRoute(segmentData, true) : BuildWhere.ApplyWhereRoute(segmentData, false);
                RawDataList = GetRawDataFromFilteredSegmentData(RawDataList, segmentData);
            }
            else
            {
                RawDataList.AddRange(_retriever.GetData<RawData>(airSql, BuildWhere, false, false, IsReservation, false).ToList());
            }

            //NEED TO GET THE DEPTIME FROM THE APPROPRIATE ROW IN THE curRpt1A CURSOR. 
            var recKeyAndSeqNo = RawDataList.GroupBy(s => s.RecKey, (key, recs) =>
            {
                return new Tuple<int, int>(key, recs.Min(s => s.SeqNo));
            });
            
            RawDataList = RawDataList.Join(recKeyAndSeqNo, r => new Tuple<int, int>(r.RecKey, r.SeqNo), s => s,
                (r, s) =>
                {
                    var rec = new RawData();
                    Mapper.Map(r, rec);
                    rec.BookDate = SetDateTime(r.Bookedgmt);
                    rec.Statustime = SetDateTime(r.Statustime);
                    rec.DetStatTim = SetDateTime(r.DetStatTim);
                    rec.DepTime = SharedProcedures.ConvertTime(r.DepTime);
                    return rec;
                }).ToList();

            var travAuth = Globals.GetParmValue(WhereCriteria.DDTRAVAUTH).TryIntParse(-1);
            if (travAuth != -1)
            {
                RawDataList = RawDataList.Where(s => s.AuthrzrNbr == travAuth).ToList();
            }

            var apprDeclComms = Globals.GetParmValue(WhereCriteria.TXTAPPRDECLCOMMS);
            if (!string.IsNullOrEmpty(apprDeclComms))
            {
                RawDataList = RawDataList.Where(s => s.ApvReason.Contains(apprDeclComms)).ToList();
                Globals.WhereText += "; Comments contain " + apprDeclComms;
            }

            if (!_conditionals.DataExists(RawDataList, Globals)) return false;
            if (!_conditionals.IsUnderOfflineThreshold(RawDataList.Count, Globals, delayer)) return false;

            return true;
        }

        private List<RawData> GetRawDataFromFilteredSegmentData(IList<RawData> rawData, IList<RawData> filteredSegData)
        {
            var filteredSegmentDataRecKeys = filteredSegData.Select(x => x.RecKey).Distinct();
            return rawData.Where(x => filteredSegmentDataRecKeys.Contains(x.RecKey)).ToList();
        }
        private DateTime SetDateTime(DateTime? date)
        {
            if (Globals.IsParmValueOn(WhereCriteria.OBSERVEDST))
            {
                if (date.GetValueOrDefault().IsDaylightSavingTime())
                {
                    return date.GetValueOrDefault().AddHours(OffsetHours + 1);
                }
            }
            return date.GetValueOrDefault().AddHours(OffsetHours);
        }

    }
}
