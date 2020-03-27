using CODE.Framework.Core.Utilities;
using CODE.Framework.Core.Utilities.Extensions;
using CrystalDecisions.CrystalReports.Engine;
using Domain.Constants;
using Domain.Helper;
using Domain.Models.ReportPrograms.Graphs;
using Domain.Models.ReportPrograms.CarTopBottomAccountsReport;
using Domain.Orm.Classes;
using Domain.Orm.iBankClientQueries;

using iBank.Services.Implementation.Shared;
using iBankDomain.RepositoryInterfaces;
using System.Collections.Generic;
using System.Linq;
using Domain;

namespace iBank.Services.Implementation.ReportPrograms.CarTopBottomAccounts
{
    class CarTopBottomAccounts : ReportRunner<RawData, FinalData>
    {
        decimal _nTotRateFinal;
        decimal _nTotBookCntFinal;
        string _groupBy = string.Empty;
        private List<string> graphOutputTypes = new List<string> { "4", "6", "TG", "XG" };
        private string _sortBy;

        IQuery<IList<MasterAccountInformation>> _getAllMasterAccountsQuery;
        IQuery<IList<MasterAccountInformation>> _getAllParentAccountsQuery;

        public CarTopBottomAccounts()
        {
            CrystalReportName = ReportNames.CAR_TOP_BOTTOM_ACCOUNTS_RPT;
        }

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
            _sortBy = Globals.GetParmValue(WhereCriteria.SORTBY);
            
            _getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);
            _getAllParentAccountsQuery = new GetAllParentAccountsQuery(ClientStore.ClientQueryDb);

            if (!BuildWhere.BuildAll(_getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: false, buildCarWhere: true, buildHotelWhere: false, buildUdidWhere: true, buildDateWhere: true, inMemory: false, isRoutingBidirectional: false, legDit: false, ignoreTravel: false)) return false;

            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }

            BuildWhere.AddSecurityChecks();

            int udid = Globals.GetParmValue(WhereCriteria.UDIDNBR).TryIntParse(0);
            var isReservation = Globals.ParmValueEquals(WhereCriteria.PREPOST, "1");

            var sqlCreator = new CarTopBottomAccountsSqlCreator();
            var sql = sqlCreator.CreateScript(BuildWhere.WhereClauseFull, udid, isReservation);
            RawDataList = RetrieveRawData<RawData>(sql, isReservation, false).ToList();

            if (!DataExists(RawDataList)) return false;

            // TODO: dbo.ibprocess.reclimit = -1 for processkey 52
            //if (!IsUnderOfflineThreshold(RawDataList)) return false;

            PerformCurrencyConversion(RawDataList);
            _groupBy = Globals.GetParmValue(WhereCriteria.GROUPBY);

            RawDataList = isReservation
                ? new CarTopBottomAccountRawDataCalculator(clientFunctions).GetSummaryReservationRawData(_getAllMasterAccountsQuery, RawDataList, _groupBy, _getAllParentAccountsQuery)
                : new CarTopBottomAccountRawDataCalculator(clientFunctions).GetSummaryBakcOfficeRawData(_getAllMasterAccountsQuery, RawDataList, _groupBy, _getAllParentAccountsQuery);

            return true;
        }

        public override bool ProcessData()
        {
            bool exportToCvs = Globals.OutputFormat == DestinationSwitch.Xls;

            FinalDataList = new CarTopBottomAccountFinalDataCalculator(clientFunctions).GetFinalDataFromRawData(_getAllMasterAccountsQuery, MasterStore,
                _getAllParentAccountsQuery, RawDataList, Globals, _groupBy, Globals.Agency, Globals.GetParmValue(WhereCriteria.DDGRPFIELD), exportToCvs);

            var calculator = new CarTopBottomAccountFinalDataCalculator(clientFunctions);
            _nTotRateFinal = calculator.GetFinalListTotalRate(FinalDataList);
            _nTotBookCntFinal = calculator.GetFinalListTotalBookCount(FinalDataList);

            FinalDataList = new CarTopBottomAccountsData().SortList(FinalDataList, Globals.GetParmValue(WhereCriteria.SORTBY), Globals.GetParmValue(WhereCriteria.RBSORTDESCASC), Globals.GetParmValue(WhereCriteria.HOWMANY));
            return true;
        }

        public override bool GenerateReport()
        {
            var outputType = Globals.GetParmValue(WhereCriteria.OUTPUTTYPE);
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    var exportFields = new CarTopBottomAccountsData().GetExportFields();

                    if (Globals.OutputFormat == DestinationSwitch.Csv)
                        ExportHelper.ConvertToCsv(FinalDataList, exportFields, Globals);
                    else
                        ExportHelper.ListToXlsx(FinalDataList, exportFields, Globals);
                    break;
                default:

                    if ("4,6,RG,XG".Contains(outputType))
                        return GenerateGraph();

                    var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

                    ReportSource = new ReportDocument();
                    ReportSource.Load(rptFilePath);

                    ReportSource.SetDataSource(FinalDataList);

                    var rawDatacalculator = new CarTopBottomAccountRawDataCalculator(clientFunctions);
                    ReportSource.SetParameterValue("cColHead1", new CarTopBottomAccountsData().GetColumnName(_groupBy));
                    ReportSource.SetParameterValue("lbfReportTotals", "Report Totals:");
                    ReportSource.SetParameterValue("nTotCnt", rawDatacalculator.GetRawListTotalRentals(RawDataList));
                    ReportSource.SetParameterValue("nTotDays", rawDatacalculator.GetRawListTotalDays(RawDataList));
                    ReportSource.SetParameterValue("nTotCost", rawDatacalculator.GetRawListTotalCarCosts(RawDataList));
                    ReportSource.SetParameterValue("nTotRate", _nTotRateFinal);
                    ReportSource.SetParameterValue("nTotBookCnt", _nTotBookCntFinal);
                    ReportSource.SetParameterValue("ftr100pct", "100.00 %");

                    var missing = CrystalFunctions.SetupCrystalReport(ReportSource, Globals);
                    CrystalFunctions.CreatePdf(ReportSource, Globals);
                    break;
            }

            return true;
        }

        private bool GenerateGraph()
        {
            string graphTitle;
            switch (_sortBy)
            {
                case "2":
                    graphTitle = "# of Rentals";
                    break;
                case "3":
                    graphTitle = "# of Days";
                    break;
                case "4":
                    graphTitle = "Avg Booked Rate";
                    break;
                default:
                    graphTitle = "Volume Booked";
                    break;
            }

            var graphData = FinalDataList.Select(s => new Graph1FinalData
            {
                CatDesc = s.AccountNumber,
                //CatDesc = s.Account.Left(30),
                Data1 = CarTopBottomAccountsHelper.GetGraphData1(s, _sortBy)
            }).ToList();

            CrystalReportName = "ibGraph1";
            var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

            ReportSource = new ReportDocument();
            ReportSource.Load(rptFilePath);

            ReportSource.SetDataSource(graphData);

            CrystalFunctions.SetupCrystalReport(ReportSource, Globals);
            ReportSource.SetParameterValue("cGrDataType", _sortBy.Equals("3") ? "N" : "C");
            ReportSource.SetParameterValue("cCatDesc", CarTopBottomAccountsHelper.GetColHead1(Globals.GetParmValue(WhereCriteria.GROUPBY)));
            ReportSource.SetParameterValue("cSubTitle", graphTitle);
            ReportSource.SetParameterValue("cGrColHdr1", Globals.BeginDate.Value.ToShortDateString() + " - " + Globals.EndDate.Value.ToShortDateString());

            CrystalFunctions.CreatePdf(ReportSource, Globals);
            return true;
        }

    }
}
