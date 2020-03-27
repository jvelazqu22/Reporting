using CODE.Framework.Core.Utilities.Extensions;
using Domain.Helper;
using iBank.Repository.SQL.Interfaces;
using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using iBank.Services.Implementation.Utilities.ClientData;
using System.Linq;
using Domain.Models.ReportPrograms.PTATravelersDetailReport;
using System.Collections.Generic;
using Domain.Orm.iBankClientQueries;
using iBank.Services.Implementation.Utilities;
using System;
using iBank.Server.Utilities;
using iBank.Services.Implementation.Shared;
using Domain;

namespace iBank.Services.Implementation.ReportPrograms.PTATravelersDetail
{

    public class TravAuthDetailRawDataRetriever
    {
        private DateTime? _beginParseDateStamp;
        private DateTime? _endParseDateStamp;
        private readonly ReportRunConditionals _conditionals = new ReportRunConditionals();        
        private DataRetriever _retriever;        
        private BuildWhere BuildWhere { get; set; }
        private ReportGlobals Globals { get; set; }
        public List<RawData> RawDataList { get; set; }
        public List<TripAuthorizerRawData> TripAuthorizerRawDataList { get; set; }
        public List<CarRawData> CarRawDataList { get; set; }
        public List<HotelRawData> HotelRawDataList { get; set; }
        public List<SummaryFinalData> SummaryDataList { get; set; }
        public GlobalsCalculator GlobalCalc { get; set; }
        public bool IsReservation { get; set; }

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

            var udidNumber = Globals.GetParmValue(WhereCriteria.UDIDNBR).TryIntParse(-1);

            var sqlBuilder = new SqlBuilder();
            _retriever = new DataRetriever(ClientDataStore.ClientQueryDb);

            var airSql = sqlBuilder.GetSql(udidNumber > 0, BuildWhere, _beginParseDateStamp, _endParseDateStamp, Globals);
            RawDataList = _retriever.GetData<RawData>(airSql, BuildWhere, false, false, IsReservation, includeAllLegs:false, checkForDuplicatesAndRemoveThem:true, handleAdvanceParamsAtReportLevelOnly:true).ToList();

            //get cancelled trips. First get sql again to clear out extra fields. 
            airSql = sqlBuilder.GetSql(udidNumber > 0, BuildWhere, _beginParseDateStamp, _endParseDateStamp, Globals);
            airSql.FromClause = udidNumber > 0
                ? "ibCancTrips T1, ibCancLegs T2, ibCancUdids T3, ibTravAuth T7, ibTravAuthorizers T8 "
                : "ibCancTrips T1, ibCancLegs T2, ibTravAuth T7, ibTravAuthorizers T8 ";

            if (BuildWhere.HasRoutingCriteria || BuildWhere.HasFirstDestination || BuildWhere.HasFirstOrigin)
            {
                RawDataList.AddRange(_retriever.GetData<RawData>(airSql, BuildWhere, false, false, IsReservation, includeAllLegs: true, checkForDuplicatesAndRemoveThem: true, handleAdvanceParamsAtReportLevelOnly: true).ToList());

                var pusher = new ReportDelayer(ClientDataStore, new MasterDataStore(), Globals);
                if (!_conditionals.IsUnderOfflineThreshold(RawDataList.Count, Globals, pusher)) return false;

                var segmentData = Collapser<RawData>.Collapse(RawDataList, Collapser<RawData>.CollapseType.Both);
                segmentData = GlobalCalc.IsAppliedToLegLevelData() ? BuildWhere.ApplyWhereRoute(segmentData, true) : BuildWhere.ApplyWhereRoute(segmentData, false);
                RawDataList = GetRawDataFromFilteredSegmentData(RawDataList, segmentData);
            }
            else
            {
                RawDataList.AddRange(_retriever.GetData<RawData>(airSql, BuildWhere, false, false, IsReservation, includeAllLegs:false, checkForDuplicatesAndRemoveThem:true, handleAdvanceParamsAtReportLevelOnly:true).ToList());
            }

            if (!_conditionals.DataExists(RawDataList, Globals)) return false;

            var authSql = sqlBuilder.GetSqlTripAuthorizer(udidNumber > 0, false, BuildWhere, Globals);
            TripAuthorizerRawDataList = _retriever.GetData<TripAuthorizerRawData>(authSql, BuildWhere, false, false, IsReservation, includeAllLegs: false, checkForDuplicatesAndRemoveThem: true, handleAdvanceParamsAtReportLevelOnly: true).ToList();

            //get cancelled trips. First get sql again to clear out extra fields. 
            authSql = sqlBuilder.GetSqlTripAuthorizer(udidNumber > 0, false, BuildWhere, Globals);
            authSql.FromClause = sqlBuilder.GetCancelledTripFromClause(udidNumber);

            TripAuthorizerRawDataList.AddRange(_retriever.GetData<TripAuthorizerRawData>(authSql, BuildWhere, false, false, IsReservation, includeAllLegs: false, checkForDuplicatesAndRemoveThem: true, handleAdvanceParamsAtReportLevelOnly: true).ToList());

            var carSql = sqlBuilder.GetSqlCar(udidNumber > 0, BuildWhere, Globals);
            CarRawDataList = _retriever.GetData<CarRawData>(carSql, BuildWhere, false, false, IsReservation, includeAllLegs: false, checkForDuplicatesAndRemoveThem: true, handleAdvanceParamsAtReportLevelOnly: true).ToList();

            //get cancelled trips. First get sql again to clear out extra fields. 
            carSql = sqlBuilder.GetSqlCar(udidNumber > 0, BuildWhere, Globals);
            carSql.FromClause = sqlBuilder.GetCancelledCarFromClause(udidNumber);

            CarRawDataList.AddRange(_retriever.GetData<CarRawData>(carSql, BuildWhere, false, false, IsReservation, includeAllLegs: false, checkForDuplicatesAndRemoveThem: true, handleAdvanceParamsAtReportLevelOnly: true).ToList());

            var hotelSql = sqlBuilder.GetSqlHotel(udidNumber > 0, BuildWhere, Globals);
            HotelRawDataList = _retriever.GetData<HotelRawData>(hotelSql, BuildWhere, false, false, IsReservation, includeAllLegs: false, checkForDuplicatesAndRemoveThem: true, handleAdvanceParamsAtReportLevelOnly: true).ToList();

            //get cancelled trips. First get sql again to clear out extra fields. 
            hotelSql = sqlBuilder.GetSqlHotel(udidNumber > 0, BuildWhere, Globals);
            hotelSql.FromClause = sqlBuilder.GetCancelledHotelFromClause(udidNumber);

            HotelRawDataList.AddRange(_retriever.GetData<HotelRawData>(hotelSql, BuildWhere, false, false, IsReservation, includeAllLegs:false, checkForDuplicatesAndRemoveThem:true, handleAdvanceParamsAtReportLevelOnly:true).ToList());

            return true;
        }

        private List<RawData> GetRawDataFromFilteredSegmentData(IList<RawData> rawData, IList<RawData> filteredSegData)
        {
            var filteredSegmentDataRecKeys = filteredSegData.Select(x => x.RecKey).Distinct();
            return rawData.Where(x => filteredSegmentDataRecKeys.Contains(x.RecKey)).ToList();
        }
    }
}
