using System.Linq;

using iBank.IntegrationTests.TestTools;
using iBank.Services.Implementation.ReportPrograms.ServiceFeeDetailByTransaction;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.IntegrationTests.iBank.Services.Implementation.ReportPrograms
{
    /*
     *  Header:
     *  Service Fee Detail By Transaction Report
     *  
     *  Items to Test:
     *      No Data
     *      Bad date range
     *      Too much data (not enough data for DEMOCA01 data source)
     *      
     *      Invoice/Credit
     *              Invoice only
     *              Credit only
     *      Include Voids
     *      Currency Conversion
     *      Group By
     *         Svc Fee Desciption
     *         Passenger Name
     *         Transaction Date 
     *     
     *          
     *          
     * */
    [TestClass]
    public class SvcFeeDetTranTests : BaseUnitTest
    {
        #region Test Methods

        [TestMethod]
        public void NoData()
        {
            GenerateCustomHandoffRecords(accnt: "1200", beginDate: "DT:2016,6,1", endDate: "DT:2016,6,2");

            InsertReportHandoff();

            //run the report
            var rpt = (SvcFeeDetTran)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(2, rptInfo.ReturnCode, "Return code failed.");
            var containsNoDataMsg = rptInfo.ErrorMessage.Contains(ReportGlobals.ReportMessages.RptMsg_NoData);
            Assert.AreEqual(true, containsNoDataMsg, "Error message failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BadDateRange()
        {
            GenerateCustomHandoffRecords(beginDate: "DT:2016,6,1", endDate: "DT:2016,5,1");

            InsertReportHandoff();

            //run the report
            var rpt = (SvcFeeDetTran)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(2, rptInfo.ReturnCode, "Return code failed.");
            var containsBadDateRangeMsg = rptInfo.ErrorMessage.Contains(ReportGlobals.ReportMessages.RptMsg_DateRange);
            Assert.AreEqual(true, containsBadDateRangeMsg, "Error message failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void InvoicesOnly()
        {
            GenerateCustomHandoffRecords(beginDate: "DT:2016,2,1", invoicesCredits: "INVOICES");

            InsertReportHandoff();

            //Run the report
            var rpt = (SvcFeeDetTran)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(33, rpt.FinalDataList.Count, "Final data records failed.");

            //Check for correct fee
            var savings = rpt.FinalDataList.Sum(s => s.Svcfee);
            Assert.AreEqual(0, savings, "Fee failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void CreditsOnly()
        {
            GenerateCustomHandoffRecords(beginDate: "DT:2016,1,1", invoicesCredits: "CREDITS");

            InsertReportHandoff();

            //Run the report
            var rpt = (SvcFeeDetTran)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(3, rpt.FinalDataList.Count, "Final data records failed.");

            //Check for correct fee
            var savings = rpt.FinalDataList.Sum(s => s.Svcfee);
            Assert.AreEqual(0, savings, "Fee failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void IncludeVoids()
        {
            GenerateCustomHandoffRecords(beginDate: "DT:2016,1,1", accnt: "9000933", includeVoids: true);

            InsertReportHandoff();

            //Run the report
            var rpt = (SvcFeeDetTran)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(4, rpt.FinalDataList.Count, "Final data records failed.");

            //Check for correct fee
            var savings = rpt.FinalDataList.Sum(s => s.Svcfee);
            Assert.AreEqual(0, savings, "Fee failed.");

            ClearReportHandoff();
        }

        /*TODO: Currently DEMOCA01 has no data
        [TestMethod]
        public void CurrencyConversion()
        {
            GenerateCustomHandoffRecords(beginDate: "DT:2016,1,1", invoicesCredits: "CREDITS");

            InsertReportHandoff();

            //Run the report
            var rpt = (SvcFeeDetTran)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(3, rpt.FinalDataList.Count, "Final data records failed.");

            //Check for correct savings
            var savings = rpt.FinalDataList.Sum(s => s.Svcfee);
            Assert.AreEqual((decimal)0, savings, "Savings failed.");

            ClearReportHandoff();
        }
        */

        [TestMethod]
        public void GroupByServiceFee()
        {
            GenerateCustomHandoffRecords(beginDate: "DT:2016,2,1");

            InsertReportHandoff();

            //Run the report
            var rpt = (SvcFeeDetTran)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(33, rpt.FinalDataList.Count, "Final data records failed.");

            //Check for correct invoice
            var invoice = rpt.FinalDataList.Last().Invoice.Trim();
            Assert.AreEqual("33499055", invoice, "Group by failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void GroupByPassengerName()
        {
            GenerateCustomHandoffRecords(beginDate: "DT:2016,2,1", groupBy: "2", accnt: "8999855");

            InsertReportHandoff();

            //Run the report
            var rpt = (SvcFeeDetTran)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(19, rpt.FinalDataList.Count, "Final data records failed.");

            //Check for correct invoice
            var invoice = rpt.FinalDataList.First().Invoice.Trim();
            Assert.AreEqual("33501150", invoice, "Group by passenger name failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void GroupByTransactionDate()
        {
            GenerateCustomHandoffRecords(beginDate: "DT:2016,2,1", groupBy: "3", accnt: "8999855");

            InsertReportHandoff();

            //Run the report
            var rpt = (SvcFeeDetTran)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(19, rpt.FinalDataList.Count, "Final data records failed.");

            //Check for correct invoice
            var invoice = rpt.FinalDataList.Last().Invoice.Trim();
            Assert.AreEqual("33502762", invoice, "Group by invoice date failed.");

            ClearReportHandoff();
        }

        #endregion

        #region Generate Report Handoff Records Helpers

        private void GenerateCustomHandoffRecords(string accnt = "", string invoicesCredits = "", string currencyType = "", string groupBy = "",
            string beginDate = "", string endDate = "", bool includeVoids = false)
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2016,6,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CBINCLVOIDS", ParmValue = includeVoids ? "ON" : "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "8", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2016,6,30", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "GROUPBY", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INACCT", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVCRED", ParmValue = "ALL", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "USD", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "142", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibSvcFeeDetTran", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3380113", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1593", ParmInOut = "IN", LangCode = "" });

            if (!string.IsNullOrEmpty(invoicesCredits))
                ManipulateReportHandoffRecords(invoicesCredits, "INVCRED");

            if (!string.IsNullOrEmpty(currencyType))
                ManipulateReportHandoffRecords(currencyType, "MONEYTYPE");

            if (!string.IsNullOrEmpty(groupBy))
                ManipulateReportHandoffRecords(groupBy, "GROUPBY");

            if (!string.IsNullOrEmpty(accnt))
                ManipulateReportHandoffRecords(accnt, "INACCT");

            if (!string.IsNullOrEmpty(beginDate))
                ManipulateReportHandoffRecords(beginDate, "BEGDATE");

            if (!string.IsNullOrEmpty(endDate))
                ManipulateReportHandoffRecords(endDate, "ENDDATE");

        }


        #endregion
    }
}
