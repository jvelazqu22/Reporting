using System;
using System.Linq;
using CODE.Framework.Core.Utilities;
using CrystalDecisions.CrystalReports.Engine;
using Domain.Helper;
using iBank.Server.Utilities;
using iBank.Services.Implementation.Shared;
using Domain.Models.ReportPrograms.AccountSummary12MonthTrend;
using Domain.Orm.iBankClientQueries;
using Domain;

namespace iBank.Services.Implementation.ReportPrograms.AccountSummaryAir12MonthTrend
{
    public class AcctSum2 : ReportRunner<RawData, FinalData>
    {
        private readonly AcctSum12MonthCalculations _calc = new AcctSum12MonthCalculations();

        private readonly AcctSum12MonthSqlCreator _creator = new AcctSum12MonthSqlCreator();

        private readonly AcctSum12MonthDataProcessor _processor = new AcctSum12MonthDataProcessor();

        private DateTime StartDate { get; set; }
        private DateTime EndDate { get; set; }
        private string ColumnHeader { get; set; }
        private bool IsReservationReport { get; set; }

        public AcctSum2()
        {
            StartDate = DateTime.Now;
            EndDate = DateTime.Now;

            Globals.SetParmValue(WhereCriteria.PREPOST, "2");
        }

        private void SetProperties()
        {
            CrystalReportName = _calc.GetCrystalReportName();
            IsReservationReport = GlobalCalc.IsReservationReport();
            ColumnHeader = _calc.IsGroupByParentAccount(Globals) ? "Parent Account" : "Account";

        }
        public override bool InitialChecks()
        {
            if (!IsOnlineReport()) return false;
            
            var startMonth = _calc.GetStartMonth(Globals);
            var startYear = _calc.GetStartYear(Globals);

            if (startMonth == 0 || !startYear.IsBetween(1998, 2020))
            {
                Globals.ReportInformation.ReturnCode = 2;
                Globals.ReportInformation.ErrorMessage = "You need to specify a month and year.";
                return false;
            }

            StartDate = _calc.GetStartDate(startMonth, startYear);
            EndDate = _calc.GetEndDate(StartDate);
            
            if (!HasAccount()) return false;
            
            return true;
        }

        public override bool GetRawData()
        {
            SetProperties();

            Globals.BeginDate = StartDate;
            Globals.EndDate = EndDate;
            
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: false, buildCarWhere: false, buildHotelWhere: false, buildUdidWhere: false, buildDateWhere: true, inMemory: false, isRoutingBidirectional: false,legDit: false, ignoreTravel: false)) return false;
            BuildWhere.AddSecurityChecks();
            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }

            var sql = _creator.Create((DateType)Convert.ToInt32(Globals.GetParmValue(WhereCriteria.DATERANGE)), BuildWhere.WhereClauseFull);
            RawDataList = RetrieveRawData<RawData>(sql, IsReservationReport, false).ToList();

            if (!DataExists(RawDataList)) return false;
            if (!IsUnderOfflineThreshold(RawDataList)) return false;

            PerformCurrencyConversion(RawDataList);
            
            return true;
        }

        public override bool ProcessData()
        {
            var groupedRecs = _processor.GroupData(RawDataList);

            if (!DataExists(groupedRecs.ToList())) return false;
            
            Globals.ReportInformation.RecordCount = groupedRecs.Select(s => s.RecordCount).DefaultIfEmpty().Sum();

            var months = _calc.FillMonthArray(StartDate);

            FinalDataList = _processor.FillAmounts(groupedRecs, months).ToList();
            
            FinalDataList = _processor.FillFinalData(FinalDataList, ClientStore.ClientQueryDb, Globals.Agency, clientFunctions, _calc.IsGroupByParentAccount(Globals)).ToList();

            FinalDataList = _processor.SortData(FinalDataList, Globals.GetParmValue(WhereCriteria.SORTBY)).ToList();
            
            return true;
        }

        public override bool GenerateReport()
        {
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                    ExportHelper.ListToXlsx(FinalDataList, _calc.GetExportFields().ToList(), Globals);
                    break;
                case DestinationSwitch.Csv:
                    ExportHelper.ConvertToCsv(FinalDataList, _calc.GetExportFields().ToList(), Globals);
                    break;
                default:
                    var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

                    ReportSource = new ReportDocument();
                    ReportSource.Load(rptFilePath);

                    ReportSource.SetDataSource(FinalDataList);
                    CrystalFunctions.SetupCrystalReport(ReportSource, Globals);

                    ReportSource.SetParameterValue("cColHead1", ColumnHeader);
                    ReportSource.SetParameterValue("dBegDate", StartDate.ToShortDateString());
                    CrystalFunctions.CreatePdf(ReportSource, Globals);
                    break;
            }
            
            return true;
        }
    }
   
}
