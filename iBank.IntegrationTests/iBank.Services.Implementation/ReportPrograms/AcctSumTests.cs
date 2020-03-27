using System.Linq;

using Domain.Helper;

using iBank.IntegrationTests.TestTools;
using iBank.Services.Implementation.ReportPrograms;
using iBank.Services.Implementation.Shared;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using iBank.Services.Implementation.ReportPrograms.AccountSummary;

namespace iBank.IntegrationTests.iBank.Services.Implementation.ReportPrograms
{
    /*  Header
        Back Office Only Report

        Items to test:
            Date Range Type
                Departure Date
                Invoice Date
            SortBy
                Column Grouped By
                Variance

            No Data
            Too Much Data
               
        Standard Parameters:
            Dates: 4/1/2009 - 2/23/2016
            Accts: 1100, 1188, 1200
        
        No Data Params:
            Date Range: 2/25/16 - 2/26/16
            Date Range Type: Departure Date
            Accounts: 1100

        Too Much Data Params:
            Date Range: 8/1/15 - 2/26/16
            Date Range Type: Departure Date
            Accounts: All


        Report Id: ea2-99E5C176-F78B-2764-5C944ADEF5A34181_54_47643.keystonecf1
            
        Create a report with the desired criteria through iBank. Generate a set of reporthandoff objects using this sql script:

        select 'ReportHandoff.Add(new ReportHandoffInformation{ParmName = "' + rtrim(ltrim(Parmname)) + '",ParmValue = "'+rtrim(ltrim(parmvalue))+'",ParmInOut = "' + rtrim(ltrim(parminout))+'", LangCode = "' + rtrim(ltrim(Langcode)) + '"});' 
        from reporthandoff where reportid = '<REPORT ID GOES HERE>' and parminout = 'IN' order by parmname
    */

    [TestClass]
    public class AcctSumTests : BaseUnitTest
    {
        private enum GroupBy
        {
            Account = 1,
            ParentAcct = 2,
            Pseudocity = 3,
            Branch = 4,
            AgentId = 5
        }

        private enum SortBy
        {
            GroupByColumn = 1,
            Variance = 2
        }

        [TestMethod]
        public void GenerateReportNoData()
        {
            GenerateReportHandoffRecordsNoData();

            InsertReportHandoff();

            //run the report
            var rpt = (AccountSummary)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(2, rptInfo.ReturnCode, "Return code failed.");
            var containsNoDataMsg = rptInfo.ErrorMessage.Contains(ReportGlobals.ReportMessages.RptMsg_NoData);
            Assert.AreEqual(true, containsNoDataMsg, "Error message failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportTooMuchData()
        {
            GenerateReportHandoffRecordsTooMuchData();

            InsertReportHandoff();

            //run the report
            var rpt = (AccountSummary)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(2, rptInfo.ReturnCode, "Return code failed.");
            var containsNoDataMsg = rptInfo.ErrorMessage.Contains(ReportGlobals.ReportMessages.RptMsg_BigVolume);
            Assert.AreEqual(true, containsNoDataMsg, "Error message failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportDepartureDateSortByAccount()
        {
            GenerateHandoffRecords(GroupBy.Account, DateType.DepartureDate, SortBy.GroupByColumn);

            InsertReportHandoff();

            //run the report
            var rpt = (AccountSummary)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //check for right number of records
            Assert.AreEqual(2, rpt.FinalDataList.Count, "Number of records failed.");
            
            //check for order
            var actualFirstAcct = rpt.FinalDataList[0].Acct;
            var expectedFirstAcct = "1188";
            Assert.AreEqual(expectedFirstAcct, actualFirstAcct, "Ordering failed.");

            var actualLastAcct = rpt.FinalDataList.Last().Acct;
            var expectedLastAcct = "1200";
            Assert.AreEqual(expectedLastAcct, actualLastAcct, "Ordering failed.");

            //check for total previous year tickets
            var actualPrevYrTikTotal = rpt.FinalDataList.Sum(x => x.PyTrips);
            var expectedPrevYrTikTotal = 8;
            Assert.AreEqual(expectedPrevYrTikTotal, actualPrevYrTikTotal,
                "Tix total failed.");


            ClearReportHandoff();
        }
        
        [TestMethod]
        public void GenerateReportDepartureDateSortByParentAccount()
        {
            GenerateHandoffRecords(GroupBy.ParentAcct, DateType.DepartureDate, SortBy.GroupByColumn);

            InsertReportHandoff();

            //run the report
            var rpt = (AccountSummary)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //check for right number of records
            Assert.AreEqual(2, rpt.FinalDataList.Count, "Number of records failed.");

            //check for order
            var actualFirstParentAcct = rpt.FinalDataList[0].AcctDesc.Trim();
            var expectedFirstParentAcct = "DEMO ACCOUNT # 1200";
            Assert.AreEqual(expectedFirstParentAcct, actualFirstParentAcct, "Ordering failed.");

            var actualLastParentAcct = rpt.FinalDataList.Last().AcctDesc.Trim();
            var expectedLastParentAcct = "test";
            Assert.AreEqual(expectedLastParentAcct, actualLastParentAcct, "Ordering failed.");

            //check for total previous year tickets
            var actualPrevYrTikTotal = rpt.FinalDataList.Sum(x => x.PyTrips);
            var expectedPrevYrTikTotal = 8;
            Assert.AreEqual(expectedPrevYrTikTotal, actualPrevYrTikTotal,
                "Tix total failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportDepartureDateSortByPseudocity()
        {
            GenerateHandoffRecords(GroupBy.Pseudocity, DateType.DepartureDate, SortBy.GroupByColumn);

            InsertReportHandoff();

            //run the report
            var rpt = (AccountSummary)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //check for right number of records
            Assert.AreEqual(1, rpt.FinalDataList.Count, "Number of records failed.");

            //check for order
            var actualFirstPseudocity = rpt.FinalDataList[0].AcctDesc.Trim();
            var expectedFirstPseudocity = "252D";
            Assert.AreEqual(expectedFirstPseudocity, actualFirstPseudocity, "Ordering failed.");

            var actualLastPseudocity = rpt.FinalDataList.Last().AcctDesc.Trim();
            var expectedLastPseudocity = "252D";
            Assert.AreEqual(expectedLastPseudocity, actualLastPseudocity, "Ordering failed.");

            //check for total tickets
            var actualPrevYrTikTotal = rpt.FinalDataList.Sum(x => x.PyTrips);
            var expectedPrevYrTikTotal = 8;
            Assert.AreEqual(expectedPrevYrTikTotal, actualPrevYrTikTotal,
                "Tix total failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportDepartureDateSortByBranch()
        {
            GenerateHandoffRecords(GroupBy.Branch, DateType.DepartureDate, SortBy.GroupByColumn);

            InsertReportHandoff();

            //run the report
            var rpt = (AccountSummary)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //check for right number of records
            Assert.AreEqual(1, rpt.FinalDataList.Count, "Number of records failed.");

            //check for order
            var actualFirstBranch = rpt.FinalDataList[0].AcctDesc.Trim();
            var expectedFirstBranch= "0031";
            Assert.AreEqual(expectedFirstBranch, actualFirstBranch, "Ordering failed.");

            var actualLastBranch = rpt.FinalDataList.Last().AcctDesc.Trim();
            var expectedLastBranch = "0031";
            Assert.AreEqual(expectedLastBranch, actualLastBranch, "Ordering failed.");

            //check for total tickets
            var actualPrevYrTikTotal = rpt.FinalDataList.Sum(x => x.PyTrips);
            var expectedPrevYrTikTotal = 8;
            Assert.AreEqual(expectedPrevYrTikTotal, actualPrevYrTikTotal,
                "Tix total failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportDepartureDateSortByAgentId()
        {
            GenerateHandoffRecords(GroupBy.AgentId, DateType.DepartureDate, SortBy.GroupByColumn);

            InsertReportHandoff();

            //run the report
            var rpt = (AccountSummary)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //check for right number of records
            Assert.AreEqual(5, rpt.FinalDataList.Count, "Number of records failed.");

            //check for order
            var actualFirstAgentId = rpt.FinalDataList[0].AcctDesc.Trim();
            var expectedFirstAgentId = "AP";
            Assert.AreEqual(expectedFirstAgentId, actualFirstAgentId, "Ordering failed.");

            var actualLastAgentId = rpt.FinalDataList.Last().AcctDesc.Trim();
            var expectedLastAgentId = "SI";
            Assert.AreEqual(expectedLastAgentId, actualLastAgentId, "Ordering failed.");

            //check for total tickets
            var actualPrevYrTikTotal = rpt.FinalDataList.Sum(x => x.PyTrips);
            var expectedPrevYrTikTotal = 8;
            Assert.AreEqual(expectedPrevYrTikTotal, actualPrevYrTikTotal,
                "Tix total failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportDepartureDateSortByVarianceGroupByAgentId()
        {
            GenerateHandoffRecords(GroupBy.AgentId, DateType.DepartureDate, SortBy.Variance);

            InsertReportHandoff();

            //run the report
            var rpt = (AccountSummary)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //check for right number of records
            Assert.AreEqual(5, rpt.FinalDataList.Count, "Number of records failed.");

            //check for order
            var actualFirstAgentId = rpt.FinalDataList[0].AcctDesc.Trim();
            var expectedFirstAgentId = "AP";
            Assert.AreEqual(expectedFirstAgentId, actualFirstAgentId, "Ordering failed.");

            var actualLastAgentId = rpt.FinalDataList.Last().AcctDesc.Trim();
            var expectedLastAgentId = "SI";
            Assert.AreEqual(expectedLastAgentId, actualLastAgentId, "Ordering failed.");

            //check for total tickets
            var actualPrevYrTikTotal = rpt.FinalDataList.Sum(x => x.PyTrips);
            var expectedPrevYrTikTotal = 8;
            Assert.AreEqual(expectedPrevYrTikTotal, actualPrevYrTikTotal,
                "Tix total failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportInvoiceDatesSortByAccount()
        {
            GenerateHandoffRecords(GroupBy.Account, DateType.InvoiceDate, SortBy.GroupByColumn);

            InsertReportHandoff();

            //run the report
            var rpt = (AccountSummary)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //check for right number of records
            Assert.AreEqual(2, rpt.FinalDataList.Count, "Number of records failed.");

            //check for order
            var actualFirstAcct = rpt.FinalDataList[0].AcctDesc.Trim();
            var expectedFirstAcct = "DEMO ACCOUNT # 1188";
            Assert.AreEqual(expectedFirstAcct, actualFirstAcct, "Ordering failed.");

            var actualLastAcct = rpt.FinalDataList.Last().AcctDesc.Trim();
            var expectedLastAcct = "DEMO ACCOUNT # 1200";
            Assert.AreEqual(expectedLastAcct, actualLastAcct, "Ordering failed.");

            //check for total tickets
            var actualPrevYrTikTotal = rpt.FinalDataList.Sum(x => x.PyTrips);
            var expectedPrevYrTikTotal = 8;
            Assert.AreEqual(expectedPrevYrTikTotal, actualPrevYrTikTotal,
                "Tix total failed.");

            ClearReportHandoff();
        }

        private void GenerateHandoffRecords(GroupBy groupBy, DateType dateRangeType, SortBy sortBy)
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2009,4,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BRANCH", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CARDNUM", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DOMINTL", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2016,2,23", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "GROUPBY", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INACCT", ParmValue = "1100,1188,1200", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK1", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK3", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INPARENTACCT", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVALCARR", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVCRED", ParmValue = "ALL", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVOICE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "GBP", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSFIRST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSLAST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "132", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibAcctSum1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RECLOC", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3373229", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "SORTBY", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TICKET", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1595", ParmInOut = "IN", LangCode = "" });

            ManipulateReportHandoffRecords(((int)groupBy).ToString(), "GROUPBY");
            ManipulateReportHandoffRecords(((int)dateRangeType).ToString(), "DATERANGE");
            ManipulateReportHandoffRecords(((int)sortBy).ToString(), "SORTBY");

        }

        private void GenerateReportHandoffRecordsNoData()
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2016,2,25", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BRANCH", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CARDNUM", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DOMINTL", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2016,2,26", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "GROUPBY", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INACCT", ParmValue = "1100", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK1", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK3", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INPARENTACCT", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVALCARR", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVCRED", ParmValue = "ALL", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVOICE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "GBP", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSFIRST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSLAST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "132", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibAcctSum1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RECLOC", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3373439", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "SORTBY", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TICKET", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1595", ParmInOut = "IN", LangCode = "" });
        }

        private void GenerateReportHandoffRecordsTooMuchData()
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2015,8,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BRANCH", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CARDNUM", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DOMINTL", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2016,2,26", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "GROUPBY", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INACCT", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK1", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK3", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INPARENTACCT", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVALCARR", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVCRED", ParmValue = "ALL", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVOICE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "GBP", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSFIRST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSLAST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "132", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibAcctSum1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RECLOC", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3373440", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "SORTBY", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TICKET", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1595", ParmInOut = "IN", LangCode = "" });
        }
    }
}