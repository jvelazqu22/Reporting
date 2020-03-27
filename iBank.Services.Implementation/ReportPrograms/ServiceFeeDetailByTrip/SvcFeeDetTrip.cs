using CODE.Framework.Core.Utilities;
using CrystalDecisions.CrystalReports.Engine;
using Domain;
using Domain.Helper;
using Domain.Models.ReportPrograms.ServiceFeeDetailByTripReport;
using Domain.Orm.iBankClientQueries;
using iBank.Server.Utilities.Helpers;
using iBank.Services.Implementation.Shared;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.ServiceFeeDetailByTrip
{

    public class SvcFeeDetTrip : ReportRunner<RawData, FinalData>
    {
        private bool IsReservationReport = false;

        public int UdidNumber { get; set; }
        public bool AccountBreak { get; set; }
        
        private readonly SvcFeeDetailByTripCalculations _calc = new SvcFeeDetailByTripCalculations();

        private Dictionary<int, string> _routeItineraries = new Dictionary<int, string>();
        private readonly SvcFeeDetailTripSqlCreator _creator = new SvcFeeDetailTripSqlCreator();
        private readonly SvcFeeDetailByTripDataProcessor _processor = new SvcFeeDetailByTripDataProcessor();

        public SvcFeeDetTrip() {}

        public override bool InitialChecks()
        {
            if (!IsGoodCombo()) return false;

            if (!IsDateRangeValid()) return false;
            
            if (!IsUdidNumberSuppliedWithUdidText()) return false;

            if (!IsOnlineReport()) return false;

            return true;
        }
        
        public override bool GetRawData()
        {
            SetProperties();

            if (!SetRawDataList()) return false;

            if (!DataExists(RawDataList)) return false;
            if (!IsUnderOfflineThreshold(RawDataList)) return false;

            if (!SetRouteItinerariesList()) return false;
            
            return true;
        }
        
        public override bool ProcessData()
        {
            FinalDataList = _processor.MapRawDataToFinal(RawDataList, _routeItineraries, new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency), AccountBreak,
                    clientFunctions, Globals).ToList();

            if (!DataExists(FinalDataList)) return false;

            PerformCurrencyConversion(FinalDataList);

            return true;
        }

        public override bool GenerateReport()
        {
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    var exportFieldList = _calc.GetExportFields(AccountBreak).ToList();

                    if (Globals.OutputFormat == DestinationSwitch.Xls)
                        ExportHelper.ListToXlsx(FinalDataList, exportFieldList, Globals);
                    else
                        ExportHelper.ConvertToCsv(FinalDataList, exportFieldList, Globals);
                    break;
                default: //Generate a PDF file
                    var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;
                    ReportSource = new ReportDocument();
                    ReportSource.Load(rptFilePath);
                    ReportSource.SetDataSource(FinalDataList);

                    CrystalFunctions.SetupCrystalReport(ReportSource, Globals);
                    CrystalFunctions.CreatePdf(ReportSource, Globals);
                    break;
            }

            return true;
        }

        private void SetProperties()
        {
            CrystalReportName = _calc.GetCrystalReportName();
            UdidNumber = GlobalCalc.GetUdidNumber();
            AccountBreak = Globals.User.AccountBreak;
            Globals.SetParmValue(WhereCriteria.PREPOST, "2");
        }

        private bool SetRawDataList()
        {
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, true, false, false, false, true, true, true, false, false, false)) return false;

            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }

            BuildWhere.AddSecurityChecks();

            var rawDataSql = _creator.CreateRawDataSql(BuildWhere.WhereClauseFull, UdidNumber, GlobalCalc.IncludeVoids());
            RawDataList = RetrieveRawData<RawData>(rawDataSql, IsReservationReport, false).ToList();

            return true;
        }

        private bool SetRouteItinerariesList()
        {
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: false, buildCarWhere: false, buildHotelWhere: false, buildUdidWhere: true, buildDateWhere: true, inMemory: true, isRoutingBidirectional: false, legDit: false, ignoreTravel: false)) return false;

            BuildWhere.AddSecurityChecks();

            var sql = _creator.CreateRouteItinerariesSql(BuildWhere.WhereClauseFull, UdidNumber);

            //var fullSql = SqlProcessor.ProcessSql(sql.FieldList, false, sql.FromClause, sql.WhereClause, sql.OrderBy, Globals);
            //var routeItinerariesList = ClientDataRetrieval.GetOpenQueryDataWithLogging<RouteItineraries>(fullSql, Globals, BuildWhere.Parameters);
            var routeItinerariesList = RetrieveRawData<RouteItineraries>(sql, IsReservationReport, false).ToList();

            //Create lookup table for route itineraries
            _routeItineraries = SharedProcedures.GetRouteItinerary(routeItinerariesList, true);

            return true;
        }

    }
}
