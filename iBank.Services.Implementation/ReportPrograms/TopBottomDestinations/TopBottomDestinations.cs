using CODE.Framework.Core.Utilities;
using CODE.Framework.Core.Utilities.Extensions;
using CrystalDecisions.CrystalReports.Engine;
using Domain;
using Domain.Constants;
using Domain.Helper;
using Domain.Models.ReportPrograms.TopBottomDestinations;
using Domain.Orm.iBankClientQueries;
using iBank.Server.Utilities;
using iBank.Services.Implementation.Shared;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.TopBottomDestinations
{
    public class TopBottomDestinations : ReportRunner<RawData, FinalData>
    {
        public List<RawData> RawDataListYtd { get; set; }

        public int TotCnt { get; set; }
        public int TotCnt2 { get; set; }
        public decimal TotChg { get; set; }
        public decimal TotChg2 { get; set; }

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
            // Set default grouping if no grouping
            if (!Globals.GetParmValue(WhereCriteria.GROUPBY).IsBetween("1", "6"))
            {
                Globals.SetParmValue(WhereCriteria.GROUPBY, "1");
            }

            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: false, buildCarWhere: false,
                buildHotelWhere: false, buildUdidWhere: true, buildDateWhere: true, inMemory: false, isRoutingBidirectional: false,
                legDit: false, ignoreTravel: false)) return false;

            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }

            BuildWhere.AddSecurityChecks();

            // Build sql script
            var udidNumber = Globals.GetParmValue(WhereCriteria.UDIDONRPT1).TryIntParse(-1);
            var sql = RawDataSqlCreator.CreateScript(BuildWhere.WhereClauseFull, udidNumber, GlobalCalc.IsReservationReport());

            // Get data from filters
            RawDataList = RetrieveRawData<RawData>(sql, GlobalCalc.IsReservationReport()).ToList();
            RawDataList = PerformCurrencyConversion(RawDataList);

            // Get year to date data
            BuildWhere.SqlParameters[0] = new SqlParameter("t1BeginDate", new DateTime(Globals.BeginDate.Value.Year, 1, 1));

            RawDataListYtd = RetrieveRawData<RawData>(sql, GlobalCalc.IsReservationReport()).ToList();
            RawDataListYtd = PerformCurrencyConversion(RawDataListYtd);

            //Data checks
            if (!DataExists(RawDataList)) return false;
            if (!IsUnderOfflineThreshold(RawDataList)) return false;
            return true;
        }

        public override bool ProcessData()
        {
            // Handle home country filter
            RawDataList = FinalDataCalculator.FilterHomeCountry(RawDataList, Globals, MasterStore);

            FinalDataCalculator.SetFirstOrg(RawDataList);
            FinalDataCalculator.SetFirstOrg(RawDataListYtd);
            RawDataList = Collapser<RawData>.Collapse(RawDataList, Collapser<RawData>.CollapseType.Both);
            RawDataListYtd = Collapser<RawData>.Collapse(RawDataListYtd, Collapser<RawData>.CollapseType.Both);

            if (!Globals.ParmValueEquals(WhereCriteria.DDWHICHDEST, "2"))
            {
                //First destination
                RawDataList = RawDataList.Where(s => s.Seg_Cntr == 1).ToList();
                RawDataListYtd = RawDataListYtd.Where(s => s.Seg_Cntr == 1).ToList();
            }
            else
            {
                //Furthest destination
                RawDataList = FinalDataCalculator.DetermineFurthestDestination(RawDataList);
                RawDataListYtd = FinalDataCalculator.DetermineFurthestDestination(RawDataListYtd);
            }

            // Initial grouping
            FinalDataList = Globals.ParmValueEquals(WhereCriteria.GROUPBY, "6")
                ? FinalDataCalculator.GroupAndCombineTripCityPair(RawDataList, RawDataListYtd, Globals)
                : FinalDataCalculator.GroupAndCombine(RawDataList, RawDataListYtd, Globals);

            // Check data
            if (!DataExists(FinalDataList)) return false;

            // Calculate total values
            TotCnt = FinalDataList.Sum(s => s.Trips);
            TotCnt2 = FinalDataList.Sum(s => s.Ytdtrips);
            TotChg = FinalDataList.Sum(s => s.Volume);
            TotChg2 = FinalDataList.Sum(s => s.Ytdvolume);

            // Final processing
            FinalDataList = SortHandler.SortAndGroupFinalData(FinalDataList, Globals, MasterStore);

            return true;
        }

        public override bool GenerateReport()
        {
            var groupBy = Globals.GetParmValue(WhereCriteria.GROUPBY);
            GenerateReportName(groupBy);

            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    // xlsx/csv report generation
                    var exportFields = FinalDataCalculator.GetExportFields(groupBy);

                    if (Globals.OutputFormat == DestinationSwitch.Csv)
                        ExportHelper.ConvertToCsv(FinalDataList, exportFields, Globals);
                    else
                        ExportHelper.ListToXlsx(FinalDataList, exportFields, Globals);
                    break;
                default:
                    // PDF report generation
                    CreatePdf(groupBy);
                    break;
            }

            return true;
        }

        private void CreatePdf(string groupBy)
        {
            var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

            ReportSource = new ReportDocument();
            ReportSource.Load(rptFilePath);

            ReportSource.SetDataSource(FinalDataList);

            ReportSource.SetParameterValue("NTOTCNT", TotCnt);
            ReportSource.SetParameterValue("NTOTCHG", TotChg);
            ReportSource.SetParameterValue("NTOTCNT2", TotCnt2);
            ReportSource.SetParameterValue("NTOTCHG2", TotChg2);
            ReportSource.SetParameterValue("LBL100PCT", "100%");
            ReportSource.SetParameterValue("CCOLHEAD1", FinalDataCalculator.GetColHead1(groupBy));
            if (CrystalReportName == ReportNames.TOP_BOTTOM_DESTINATIONS_RPT_1)
            {
                ReportSource.SetParameterValue("LBLTOTLISTED", "Total Listed");
                ReportSource.SetParameterValue("LBLNOTLISTED", "Total Not Listed");
                ReportSource.SetParameterValue("LBLRPTTOTALS", "Report Totals");
            }
            else
            {
                ReportSource.SetParameterValue("CCOLHEAD2", FinalDataCalculator.GetColHead2(groupBy));
            }

            ReportSource.SetParameterValue("CSUBTITLE", Globals.ParmValueEquals(WhereCriteria.DDWHICHDEST, "2")
                ? "Based on Turnaround Point within Itinerary"
                : "Based on First Destination within Itinerary");
            var beginDate = Globals.BeginDate;
            Globals.BeginDate = new DateTime(Globals.BeginDate.Value.Year, 1, 1);
            var cDateDesc2 = "YTD - " + Globals.BuildDateDesc();
            ReportSource.SetParameterValue("CDATEDESC2", cDateDesc2);
            Globals.BeginDate = beginDate;

            CrystalFunctions.SetupCrystalReport(ReportSource, Globals);
            CrystalFunctions.CreatePdf(ReportSource, Globals);
        }

        private void GenerateReportName(string groupBy)
        {
            var report2 = new List<string> { "2", "3", "5" };
            CrystalReportName = report2.Contains(groupBy)
                ? CrystalReportName = ReportNames.TOP_BOTTOM_DESTINATIONS_RPT_2
                : CrystalReportName = ReportNames.TOP_BOTTOM_DESTINATIONS_RPT_1;
        }
    }
}
