using CODE.Framework.Core.Utilities;
using CODE.Framework.Core.Utilities.Extensions;
using CrystalDecisions.CrystalReports.Engine;
using Domain.Helper;
using Domain.Models.ReportPrograms.TransactionSummary;
using Domain.Models.TransactionSummary;
using Domain.Orm.iBankClientQueries;
using Domain.Orm.iBankMastersQueries;
using Domain.Orm.iBankMastersQueries.Other;
using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities;
using iBank.Services.Implementation.Shared;

namespace iBank.Services.Implementation.ReportPrograms.TransactionSummary
{
    public class TransactionSummary : ReportRunner<RawData,FinalData>
    {
        private string _begMonth;
        private string _endMonth;
        private int _begYear;
        private int _endYear;
        private int _begMonthNumber;
        private int _endMonthNumber;
        private string _dateDesc;
        private bool _logGen1;
        TransactionSummaryData transactionSummaryData = new TransactionSummaryData();

        public TransactionSummary()
        {
            CrystalReportName = "ibTransactSum1";
        }

        public override bool InitialChecks()
        {
            if (!IsGoodCombo()) return false;

            if (!IsOnlineReport()) return false;

            return true;
        }

        public override bool GetRawData()
        {
            var server = Globals.AgencyInformation.ServerName;
            var db = Globals.AgencyInformation.DatabaseName;
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(new iBankClientQueryable(server, db), Globals.Agency);

            //We just want the trip data
            if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: false,buildCarWhere: false,buildHotelWhere: false, buildUdidWhere: false, buildDateWhere: false,inMemory: false, isRoutingBidirectional: false,legDit: false, ignoreTravel: false)) return false;

            var whereTrip = string.Empty;
            if (!string.IsNullOrEmpty(BuildWhere.WhereClauseTrip))
                whereTrip = " and " + BuildWhere.WhereClauseTrip.Replace("T1.", "T2.");

            _begMonth = Globals.GetParmValue(WhereCriteria.STARTMONTH);

            var temp = 0;
            _begMonth = int.TryParse(_begMonth, out temp) ? temp.MonthNameFromNumber() : _begMonth;
            _begMonthNumber = _begMonth.MonthNumberFromName();
            _begYear = Globals.GetParmValue(WhereCriteria.STARTYEAR).TryIntParse(-1);

            _endMonth = Globals.GetParmValue(WhereCriteria.ENDMONTH);

            temp = 0;
            _endMonth = int.TryParse(_endMonth, out temp) ? temp.MonthNameFromNumber() : _endMonth;
            _endMonthNumber = _endMonth.MonthNumberFromName();
            _endYear = Globals.GetParmValue(WhereCriteria.ENDYEAR).TryIntParse(-1);

            if(!_begMonthNumber.IsBetween(1,12) || !_endMonthNumber.IsBetween(1,12) || !_begYear.IsBetween(1998,2040) || !_endYear.IsBetween(1998, 2040))
            {
                Globals.ReportInformation.ReturnCode = 2;
                Globals.ReportInformation.ErrorMessage = "You need to specify valid Starting and Ending months and years.";
                return false;
            }

            _dateDesc = _begYear == _endYear 
                ? "For Time Frame from " + _begMonth + " to " + _endMonth + ", " + _endYear 
                : "For Time Frame from " + _begMonth + ", " + _begYear + " to " + _endMonth + ", " + _endYear;

            var sqlScript = new TransactionSummarySqlCreator().CreateScript(_begYear, _endYear, _begMonthNumber, _endMonthNumber, whereTrip, Globals.Agency);

            sqlScript.WhereClause += string.IsNullOrWhiteSpace(BuildWhere.WhereClauseAdvanced) ? "" : " AND " + BuildWhere.WhereClauseAdvanced;
            var fullSql = SqlProcessor.ProcessSql(sqlScript.FieldList, false, sqlScript.FromClause, sqlScript.WhereClause, string.Empty, Globals);
            RawDataList = new OpenMasterQuery<RawData>(MasterStore.MastersQueryDb, fullSql, BuildWhere.Parameters).ExecuteQuery();

            return true;
        }

        public override bool ProcessData()
        {
            _logGen1 = Globals.IsParmValueOn(WhereCriteria.CBBREAKBYSOURCE);

            var begYearMonth = (_begYear * 100) + _begMonthNumber;
            var endYearMonth = (_endYear * 100) + _endMonthNumber;
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);
            bool cbsShowClientDetail = Globals.IsParmValueOn(WhereCriteria.CBSHOWCLIENTDETAIL);
            if (cbsShowClientDetail) CrystalReportName = "ibTransactSum2";

            FinalDataList = new TransactionSummaryFinalDataCalculator().GetFinalDataListFromRawData(RawDataList, cbsShowClientDetail,
                _logGen1, clientFunctions, getAllMasterAccountsQuery, Globals, MasterStore);

            FinalDataList = transactionSummaryData.SortFinalData(FinalDataList, _logGen1, begYearMonth, endYearMonth);

            return true;
        }

        public override bool GenerateReport()
        {
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    var exportFieldList = transactionSummaryData.GetExportFields(Globals);

                    if (Globals.OutputFormat == DestinationSwitch.Csv)
                    {
                        ExportHelper.ConvertToCsv(FinalDataList, exportFieldList, Globals);
                    }
                    else
                    {
                        ExportHelper.ListToXlsx(FinalDataList, exportFieldList, Globals);
                    }
                    break;
                default:
                    var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

                    ReportSource = new ReportDocument();
                    ReportSource.Load(rptFilePath);

                    ReportSource.SetDataSource(FinalDataList);
                    
                    CrystalFunctions.SetupCrystalReport(ReportSource, Globals);

                    ReportSource.SetParameterValue("cDateDesc", _dateDesc);
                    ReportSource.SetParameterValue("cColHead1", Globals.Agency.EqualsIgnoreCase("AXI") ? "AuthCount" : "PCM");
                    ReportSource.SetParameterValue("lLogGen1", _logGen1);
                    ReportSource.SetParameterValue("cAcctName", "Site: " + Globals.AgencyInformation.AgencyName);

                    CrystalFunctions.CreatePdf(ReportSource, Globals);
                    break;
            }

            return true;
        }
    }
}