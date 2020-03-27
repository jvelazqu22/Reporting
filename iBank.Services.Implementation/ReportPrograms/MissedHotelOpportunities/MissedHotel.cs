using CODE.Framework.Core.Utilities;
using CrystalDecisions.CrystalReports.Engine;
using Domain;
using Domain.Helper;
using Domain.Models.ReportPrograms.MissedHotelOpportunitiesReport;
using Domain.Orm.iBankClientQueries;
using iBank.Server.Utilities.Helpers;
using iBank.Services.Implementation.Shared;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.MissedHotelOpportunities
{
    public class MissedHotel : ReportRunner<RawData, FinalData>
    {
        private UserBreaks UserBreaks { get; set; }
        private bool IsReservationReport { get; set; }
        private bool IncludeTripsWithHotels { get; set; }
        private int TripDuration { get; set; }
        private bool ExcludeCarOnly { get; set; }
        private bool AccountBreak { get; set; }
        private bool GroupByHomeCountry { get; set; }
        private bool BreakByAgenId { get; set; }

        private readonly MissedHotelSqlCreator _creator = new MissedHotelSqlCreator();
        private readonly MissedHotelDataProcessor _processor = new MissedHotelDataProcessor();
        private readonly MissedHotelCalculations _calc = new MissedHotelCalculations();

        private IList<RawData> _carRawData = new List<RawData>();
        private IList<RawData> _hotelRawData = new List<RawData>();
        
        public MissedHotel()
        {
            CrystalReportName = "ibMissedHotel";
        }

        public override bool InitialChecks()
        {
            if (!IsGoodCombo()) return false;

            if (!IsDateRangeValid()) return false;

            if (!IsUdidNumberSuppliedWithUdidText()) return false;

            if (!IsOnlineReport()) return false;

            SetupParameters();

            return true;
        }
        
        public override bool GetRawData()
        {
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: true, buildCarWhere: false, buildHotelWhere: false, buildUdidWhere: true, buildDateWhere: true, inMemory: true, isRoutingBidirectional: false, legDit: false, ignoreTravel: true)) return false;

            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }

            BuildWhere.AddSecurityChecks();
            
            var originalWhereClause = string.Format("{0} and datediff(day,TripStart,TripEnd) >= {1}", BuildWhere.WhereClauseFull, TripDuration);
            var udidNumber = GlobalCalc.GetUdidNumber();
            
            //leg data
            var legSql = _creator.CreateLegSql(originalWhereClause, IncludeTripsWithHotels, IsReservationReport, udidNumber);
            if (!IncludeTripsWithHotels) legSql = _creator.AddInExcludeHotelClause(legSql, originalWhereClause, IsReservationReport);
            RawDataList = RetrieveRawData<RawData>(legSql, IsReservationReport, true).ToList();
            
            if (!DataExists(RawDataList)) return false;
            if (!IsUnderOfflineThreshold(RawDataList)) return false;
            
            //car data
            if (!ExcludeCarOnly)
            {
                var carSql = _creator.CreateCarSql(originalWhereClause, IsReservationReport, udidNumber, legSql.FieldList);
                if (!IncludeTripsWithHotels) carSql = _creator.AddInExcludeHotelClause(carSql, originalWhereClause, IsReservationReport);
                _carRawData = RetrieveRawData<RawData>(carSql, IsReservationReport, false);
            }

            //hotel data
            if (IncludeTripsWithHotels)
            {
                var hotelSql = _creator.CreateHotelSql(originalWhereClause, IsReservationReport, udidNumber, legSql.FieldList);
                _hotelRawData = RetrieveRawData<RawData>(hotelSql, IsReservationReport, false);
            }

            return true;
        }

        public override bool ProcessData()
        {
            var routeItineraries = SharedProcedures.GetRouteItinerary(RawDataList, true);

            if (BuildWhere.HasRoutingCriteria || BuildWhere.HasFirstOrigin || BuildWhere.HasFirstDestination)
            {
                if (GlobalCalc.IsAppliedToLegLevelData())
                {
                    RawDataList = BuildWhere.ApplyWhereRoute(RawDataList, true);
                }
                else
                {
                    var segmentData = Collapser<RawData>.Collapse(RawDataList, Collapser<RawData>.CollapseType.Both);
                    segmentData = BuildWhere.ApplyWhereRoute(segmentData, false);
                    RawDataList = GetLegDataFromFilteredSegData(RawDataList, segmentData);
                }
            }
            
            RawDataList = GlobalCalc.IsAppliedToLegLevelData() ? BuildWhere.ApplyWhereRoute(RawDataList, true) : BuildWhere.ApplyWhereRoute(RawDataList, false);
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            FinalDataList = new MissedHotelDataProcessor().GetFinalData(RawDataList, GroupByHomeCountry, routeItineraries, Globals, 
                BreakByAgenId, MasterStore, AccountBreak, clientFunctions, getAllMasterAccountsQuery, UserBreaks, ExcludeCarOnly, 
                _carRawData, IncludeTripsWithHotels, _hotelRawData, BuildWhere);
            
            return true;
        }

        public override bool GenerateReport()
        {
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    var exportFields = _calc.GetExportFields(GroupByHomeCountry, Globals, UserBreaks);

                    if (Globals.OutputFormat == DestinationSwitch.Csv)
                    {
                        ExportHelper.ConvertToCsv(FinalDataList, exportFields, Globals);
                    }
                    else
                    {
                        ExportHelper.ListToXlsx(FinalDataList, exportFields, Globals);
                    }
                    break;
                default:
                    var graphData = _processor.GetSubReportData(FinalDataList).ToList();

                    var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

                    ReportSource = new ReportDocument();
                    ReportSource.Load(rptFilePath);

                    ReportSource.SetDataSource(FinalDataList);
                    ReportSource.Subreports[0].SetDataSource(graphData);

                    CrystalFunctions.SetupCrystalReport(ReportSource, Globals);
                    ReportSource.SetParameterValue("LLOGGEN1", IncludeTripsWithHotels);
                    ReportSource.SetParameterValue("LLOGGEN2", BreakByAgenId);

                    CrystalFunctions.CreatePdf(ReportSource, Globals);
                    break;
            }
            return true;
        }

        private void SetupParameters()
        {
            IsReservationReport = GlobalCalc.IsReservationReport();

            IncludeTripsWithHotels = Globals.IsParmValueOn(WhereCriteria.CBINCLTRIPSWITHHOTELS);

            GroupByHomeCountry = Globals.IsParmValueOn(WhereCriteria.CBGRPBYHOMECTRY);

            BreakByAgenId = Globals.IsParmValueOn(WhereCriteria.CBBRKBYAGENTID);

            /** ONLY WANT TRIPS WITH A DURATION > "X" DAYS, **
             ** WHERE "X" IS DETERMINED BY THE USER.        **
             ** WE CANNOT APPLY "# OF PAX ON A PLANE" CRITERIA UNTIL WE SUMMARIZE. **/

            int tripDuration;
            if (int.TryParse(Globals.GetParmValue(WhereCriteria.NBRDAYSDURATION), out tripDuration))
            {
                TripDuration = (tripDuration < 1) ? 1 : tripDuration;
            }

            AccountBreak = Globals.User.AccountBreak;

            UserBreaks = SharedProcedures.SetUserBreaks(Globals.User.ReportBreaks);

            ExcludeCarOnly = Globals.IsParmValueOn(WhereCriteria.CBEXCLUDECARONLY)
                                || BuildWhere.HasRoutingCriteria
                                || BuildWhere.HasFirstDestination
                                || BuildWhere.HasFirstOrigin;
        }
        
    }
}
