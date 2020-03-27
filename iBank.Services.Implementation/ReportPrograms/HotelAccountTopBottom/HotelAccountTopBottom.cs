using CODE.Framework.Core.Utilities;
using CODE.Framework.Core.Utilities.Extensions;
using CrystalDecisions.CrystalReports.Engine;
using Domain;
using Domain.Constants;
using Domain.Helper;
using Domain.Models.ReportPrograms.Graphs;
using Domain.Models.ReportPrograms.HotelAccountTopBottom;
using Domain.Orm.Classes;
using Domain.Orm.iBankClientQueries;
using iBank.Services.Implementation.Shared;
using iBankDomain.RepositoryInterfaces;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.HotelAccountTopBottom
{
    public class HotelAccountTopBottom : ReportRunner<RawData, FinalData>
    {
        decimal _nTotRateFinal;
        decimal _nTotBookCntFinal;
        string _groupBy = string.Empty;
        IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery;
        IQuery<IList<MasterAccountInformation>> getAllParentAccountsQuery;
        public HotelAccountTopBottom()
        {
            CrystalReportName = ReportNames.HOTEL_ACCOUNTS_TOP_BOTTOM_RPT;
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
            getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);
            getAllParentAccountsQuery = new GetAllParentAccountsQuery(ClientStore.ClientQueryDb);

            if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: false, buildCarWhere: false, buildHotelWhere: true, buildUdidWhere: true, buildDateWhere: true,inMemory: false, isRoutingBidirectional: false, legDit: false, ignoreTravel: false)) return false;

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

            var sqlCreator = new HotelAccountsTopBottomSqlCreator();
            var sql = sqlCreator.CreateScript(BuildWhere.WhereClauseFull, udid, isReservation);
            RawDataList = RetrieveRawData<RawData>(sql, isReservation, false).ToList();

            if (!DataExists(RawDataList)) return false;

            PerformCurrencyConversion(RawDataList);
            _groupBy = Globals.GetParmValue(WhereCriteria.GROUPBY);

            RawDataList = isReservation
                ? new HotelAccountTopBottomRawDataCalculator(clientFunctions).GetSummaryReservationRawData(getAllMasterAccountsQuery, RawDataList, _groupBy, getAllParentAccountsQuery)
                : new HotelAccountTopBottomRawDataCalculator(clientFunctions).GetSummaryBakcOfficeRawData(getAllMasterAccountsQuery, RawDataList, _groupBy, getAllParentAccountsQuery);

            return true;
        }

        public override bool ProcessData()
        {
            var exportToCvs = Globals.OutputFormat == DestinationSwitch.Xls;

            var calc = new HotelAccountTopBottomFinalDataCalculator(clientFunctions);
            FinalDataList = calc.GetFinalDataFromRawData(getAllMasterAccountsQuery, MasterStore,
                getAllParentAccountsQuery, RawDataList, Globals, _groupBy, Globals.Agency, Globals.GetParmValue(WhereCriteria.DDGRPFIELD), exportToCvs);

            var calculator = new HotelAccountTopBottomFinalDataCalculator(clientFunctions);
            _nTotRateFinal = calculator.GetFinalListTotalRate(FinalDataList);
            _nTotBookCntFinal = calculator.GetFinalListTotalBookCount(FinalDataList);

            FinalDataList = new HotelAccountTopBottomData().SortList(FinalDataList, Globals.GetParmValue(WhereCriteria.SORTBY), Globals.GetParmValue(WhereCriteria.RBSORTDESCASC), Globals.GetParmValue(WhereCriteria.HOWMANY));
            return true;
        }

        public override bool GenerateReport()
        {
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    var exportFields = new HotelAccountTopBottomData().GetExportFields();
                    new HotelAccountTopBottomFinalDataCalculator(clientFunctions).CalculateAveBook(FinalDataList);

                    if (Globals.OutputFormat == DestinationSwitch.Csv)
                        ExportHelper.ConvertToCsv(FinalDataList, exportFields, Globals);
                    else
                        ExportHelper.ListToXlsx(FinalDataList, exportFields, Globals);
                    break;
                default:
                    var outputType = Globals.GetParmValue(WhereCriteria.OUTPUTTYPE);
                    if ("4,6,TG,XG".Contains(outputType))
                    {
                        return GenerateGraph();
                    }

                    var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

                    ReportSource = new ReportDocument();
                    ReportSource.Load(rptFilePath);

                    ReportSource.SetDataSource(FinalDataList);

                    var rawDatacalculator = new HotelAccountTopBottomRawDataCalculator(clientFunctions);
                    ReportSource.SetParameterValue("cColHead1", new HotelAccountTopBottomData().GetColumnName(_groupBy));
                    ReportSource.SetParameterValue("lbfReportTotals", "Report Totals:");
                    ReportSource.SetParameterValue("nTotCnt", rawDatacalculator.GetRawListTotalStays(RawDataList));
                    ReportSource.SetParameterValue("nTotNites", rawDatacalculator.GetRawListTotalNights(RawDataList));
                    ReportSource.SetParameterValue("nTotCost", rawDatacalculator.GetRawListTotalHotelCosts(RawDataList));
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
            var sortBy = Globals.GetParmValue(WhereCriteria.SORTBY);
            var graphTitle = "Volume Booked";
            switch (sortBy)
            {
                case "1":
                    graphTitle = "Total Spend Volume";
                    break;
                case "2":
                    graphTitle = "# of Trips";
                    break;
            }

            var recordsToRemove = FinalDataList.Where(w => w.Hotelcost == 0).ToList();
            recordsToRemove.ForEach(r => FinalDataList.Remove(r));

            var graphData = FinalDataList.Select(s => new Graph1FinalData
            {
                CatDesc = s.AccountNumber,
                Data1 = sortBy.Equals("2") ? s.Hotelcost : s.Hotelcost
            }).ToList();

            CrystalReportName = "ibGraph1";

            var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

            ReportSource = new ReportDocument();
            ReportSource.Load(rptFilePath);

            ReportSource.SetDataSource(graphData);

            CrystalFunctions.SetupCrystalReport(ReportSource, Globals);
            ReportSource.SetParameterValue("cGrDataType", "2,3".Contains(sortBy) ? "N" : "C");
            ReportSource.SetParameterValue("cCatDesc", ReportNames.HOTEL_TOP_BOTTOM_ACCOUNTS_ACCOUNT_COLUMN_NAME);
            ReportSource.SetParameterValue("cSubTitle", graphTitle);
            ReportSource.SetParameterValue("cGrColHdr1", Globals.BeginDate.Value.ToShortDateString() + " - " + Globals.EndDate.Value.ToShortDateString());

            CrystalFunctions.CreatePdf(ReportSource, Globals);
            return true;
        }

    }
}
