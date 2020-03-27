using CODE.Framework.Core.Utilities;
using CrystalDecisions.CrystalReports.Engine;
using Domain.Helper;
using Domain.Models;
using iBank.Services.Implementation.Shared;
using System.Collections.Generic;
using System.Linq;
using Domain.Models.ReportPrograms.AccountSummary;
using Domain.Orm.iBankClientQueries;
using iBank.Repository.SQL.Repository;
using Domain;

namespace iBank.Services.Implementation.ReportPrograms.AccountSummary
{
    public class AccountSummary : ReportRunner<RawData, FinalData>
    {
        private string _groupBy;
        private string _colHead;
        private string _cDateDesc1;
        private string _cDateDesc2;
        private string _fieldNameToExclude;

        private AccountSummaryRawDataCalculator accSumRawDataCalc = new AccountSummaryRawDataCalculator();

        public AccountSummary() { }

        public override bool InitialChecks()
        {
            //This report can only be run on historical data 
            Globals.SetParmValue(WhereCriteria.PREPOST, "2");

            if (!IsOnlineReport()) return false;

            if (!IsDateRangeValid()) return false;
            
            //don't run online without an acct. 
            if (!HasAccount()) return false;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool GetRawData()
        {
            SetProperties();

            //build the date and trip queries
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);
            if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: false, buildCarWhere: false, buildHotelWhere: false, buildUdidWhere: false, buildDateWhere: true, inMemory: false, isRoutingBidirectional: false, legDit: false, ignoreTravel: false)) return false;
            BuildWhere.AddSecurityChecks();
            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }
            
            var sqlScript = new SqlScript();
            sqlScript.FromClause = "hibtrips T1";
            sqlScript.KeyWhereClause = " valcarr <> 'ZZ' AND valcarr <> '$$'" + string.Format("AND NOT({0} is null) AND ", _fieldNameToExclude);
            sqlScript.WhereClause = sqlScript.KeyWhereClause + BuildWhere.WhereClauseFull;
            sqlScript.FieldList = "*";

            var rawCurrentYearData = RetrieveRawData<RawData>(sqlScript, GlobalCalc.IsReservationReport(), false).ToList();
            rawCurrentYearData = accSumRawDataCalc.GetCurrentYearData(rawCurrentYearData);

            //Get prior year data
            var beginDate = Globals.BeginDate.Value;
            var endDate = Globals.EndDate.Value;
            Globals.BeginDate = beginDate.AddYears(-1);
            Globals.EndDate = endDate.AddYears(-1);
            _cDateDesc1 = Globals.BuildDateDesc();

            if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, true, false, false, false, false, true, false, false, false, false)) return false;
            BuildWhere.AddSecurityChecks();
            BuildWhere.AddAdvancedClauses();

            RawDataList = RetrieveRawData<RawData>(sqlScript, GlobalCalc.IsReservationReport(), false).ToList();
            RawDataList = accSumRawDataCalc.GetPreviousYearData(RawDataList);

            //Combine the two years of data
            RawDataList.AddRange(rawCurrentYearData);
            PerformCurrencyConversion(RawDataList);
            return true;
        }

        public override bool ProcessData()
        {
            List<IGrouping<string, RawData>> groupedRecs = accSumRawDataCalc.GetGroupRecsBasedOnGroupBy(_groupBy, RawDataList);
            _colHead = accSumRawDataCalc.GetColumnHeaderBaseOnGroupBy(_groupBy);

            if (!DataExists(groupedRecs)) return false;

            var server = Globals.AgencyInformation.ServerName;
            var db = Globals.AgencyInformation.DatabaseName;
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(new iBankClientQueryable(server, db), Globals.Agency);
            var getAllParentAccountsQuery = new GetAllParentAccountsQuery(new iBankClientQueryable(server, db));
            var outputType = Globals.GetParmValue(WhereCriteria.OUTPUTTYPE);
            var accSummaryFinalDataCalculator = new AccountSummaryFinalDataCalculator();

            FinalDataList = accSummaryFinalDataCalculator.GetFinalDataFromRawData(getAllMasterAccountsQuery, getAllParentAccountsQuery, 
                                RawDataList, outputType, _groupBy, groupedRecs, clientFunctions, Globals, _colHead);

            //"regroup" if groupby is 2 (parent account). 
            if (_groupBy == "2")
            {
                FinalDataList = accSummaryFinalDataCalculator.RegroupParentAccount(FinalDataList);
            }

            //SORTBY == 2 --> sort by Variance, else sort by the grouped column
            FinalDataList = Globals.GetParmValue(WhereCriteria.SORTBY) == "2" 
                                ? FinalDataList.OrderByDescending(s => s.VarAmt).ThenBy(s => s.AcctDesc).ToList() 
                                : FinalDataList.OrderBy(s => s.AcctDesc).ToList();

            return true;
        }

        public override bool GenerateReport()
        {
            var exportFields = new AccountSummaryData().GetExportFields();
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:                
                    ExportHelper.ListToXlsx(FinalDataList, exportFields, Globals);
                    break;
                case DestinationSwitch.Csv:
                    ExportHelper.ConvertToCsv(FinalDataList, exportFields, Globals);
                    break;
                default:
                    var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

                    ReportSource = new ReportDocument();
                    ReportSource.Load(rptFilePath);

                    ReportSource.SetDataSource(FinalDataList);
                    CrystalFunctions.SetupCrystalReport(ReportSource, Globals);

                    ReportSource.SetParameterValue("cColHead1", _colHead);
                    ReportSource.SetParameterValue("cDateDesc1", _cDateDesc1);
                    ReportSource.SetParameterValue("cDateDesc2", _cDateDesc2);

                    CrystalFunctions.CreatePdf(ReportSource, Globals);
                    break;
            }

            return true;
        }
        private void SetProperties()
        {
            _groupBy = Globals.GetParmValue(WhereCriteria.GROUPBY);
            CrystalReportName = accSumRawDataCalc.GetCrystalReportName(_groupBy);

            _cDateDesc2 = Globals.BuildDateDesc();
            _cDateDesc1 = string.Empty;

            _fieldNameToExclude = accSumRawDataCalc.GetFieldNameBaseOnGroupBy(_groupBy);
            _colHead = accSumRawDataCalc.GetColumnHeaderBaseOnGroupBy(_groupBy);
        }
    }
    
}
