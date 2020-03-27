using System.Linq;

using Domain.Helper;

using iBank.IntegrationTests.TestTools;
using iBank.Services.Implementation.ReportPrograms;
using iBank.Services.Implementation.ReportPrograms.ServiceFeeDetailByTrip;
using iBank.Services.Implementation.Shared;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.IntegrationTests.iBank.Services.Implementation.ReportPrograms
{
    /*
     *  Header:
     *  Service Fee Detail By Trip Report
     *  
     *  Items to Test:
     *      No Data
     *      Bad date range
     *      Too much data
     *      
     *      Back Office
     *          Date Range Type
     *              Departure
     *              Invoice
     *          Invoice/Credit
     *              Invoice only
     *              Credit only
     *          Currency Conversion
     *          Summary Page only
     *          
     *      Reservation
     *          Date Range Type
     *              Departure
     *              Invoice
     *              Booked Date
     *          Domestic only
     *          International only
     *          Air only
     *          Rail only
     *          Currency Conversion
     *          Summary Page only
     *          
     *          
     * */
    [TestClass]
    public class SvcFeeDetTripTests : BaseUnitTest
    {
        #region Test Methods

        [TestMethod]
        public void NoData()
        {
            GenerateCustomHandoffRecords(beginDate: "DT:2016,3,1", endDate: "DT:2016,3,2", accnt: "1200");

            InsertReportHandoff();

            //run the report
            var rpt = (SvcFeeDetTrip)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(2, rptInfo.ReturnCode, "Return code failed.");
            var containsNoDataMsg = rptInfo.ErrorMessage.Contains(ReportGlobals.ReportMessages.RptMsg_NoData);
            Assert.AreEqual(true, containsNoDataMsg, "Error message failed.");

            ClearReportHandoff();
        }

        //TODO: DEMOCA01 datasource doesnt have enough data
        //[TestMethod]
        //public void TooMuchData()
        //{
        //    GenerateCustomHandoffRecords(beginDate: "DT:2016,3,1", endDate: "DT:2016,3,2", accnt: "1200");

        //    InsertReportHandoff();

        //    //run the report
        //    var rpt = (SvcFeeDetTrip)RunReport();
        //    var rptInfo = rpt.Globals.ReportInformation;

        //    //check for validity
        //    Assert.AreEqual(2, rptInfo.ReturnCode, "Return code failed.");
        //    var containsNoDataMsg = rptInfo.ErrorMessage.Contains(ReportGlobals.ReportMessages.RptMsg_BigVolume);
        //    Assert.AreEqual(true, containsNoDataMsg, "Error message failed.");

        //    ClearReportHandoff();
        //}

        [TestMethod]
        public void BadDateRange()
        {
            GenerateCustomHandoffRecords(beginDate: "DT:2016,3,1", endDate: "DT:2016,2,2");

            InsertReportHandoff();

            //run the report
            var rpt = (SvcFeeDetTrip)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(2, rptInfo.ReturnCode, "Return code failed.");
            var containsBadDateRangeMsg = rptInfo.ErrorMessage.Contains(ReportGlobals.ReportMessages.RptMsg_DateRange);
            Assert.AreEqual(true, containsBadDateRangeMsg, "Error message failed.");

            ClearReportHandoff();
        }

       
        [TestMethod]
        public void DepartureDateType()
        {
            GenerateCustomHandoffRecords(beginDate: "DT:2016,1,1", endDate: "DT:2016,6,30", accnt: "9000933");

            InsertReportHandoff();

            //Run the report
            var rpt = (SvcFeeDetTrip)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(4, rpt.FinalDataList.Count, "Final data records failed.");

            //Check for correct fee
            var fee = rpt.FinalDataList.Sum(s => s.Svcfee);
            Assert.AreEqual(0, fee, "Fee failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void InvoiceDateType()
        {
            GenerateCustomHandoffRecords(dateType: DateType.InvoiceDate, beginDate: "DT:2016,1,1", endDate: "DT:2016,6,30", accnt: "9000933");

            InsertReportHandoff();

            //Run the report
            var rpt = (SvcFeeDetTrip)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(3, rpt.FinalDataList.Count, "Final data records failed.");

            //Check for correct fee
            var fee = rpt.FinalDataList.Sum(s => s.Svcfee);
            Assert.AreEqual(0, fee, "Fee failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void InvoicesOnly()
        {
            GenerateCustomHandoffRecords(beginDate: "DT:2016,1,1", endDate: "DT:2016,6,30", invoicesCredits: "INVOICES");

            InsertReportHandoff();

            //Run the report
            var rpt = (SvcFeeDetTrip)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(75, rpt.FinalDataList.Count, "Final data records failed.");

            //Check for correct fee
            var fee = rpt.FinalDataList.Sum(s => s.Svcfee);
            Assert.AreEqual(0, fee, "Fee failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void IncludeVoids()
        {
            GenerateCustomHandoffRecords(beginDate: "DT:2016,1,1", endDate: "DT:2016,6,30", includeVoids: true, accnt: "9000933");

            InsertReportHandoff();

            //Run the report
            var rpt = (SvcFeeDetTrip)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(5, rpt.FinalDataList.Count, "Final data records failed.");

            //Check for correct fee
            var fee = rpt.FinalDataList.Sum(s => s.Svcfee);
            Assert.AreEqual(0, fee, "Fee failed.");

            ClearReportHandoff();
        }

        #endregion

        #region Generate Report Handoff Records Helpers

        private void GenerateCustomHandoffRecords(DateType dateType = DateType.DepartureDate, string accnt = "", string invoicesCredits = "", string currencyType = "", bool includeVoids = false,
            string beginDate = "", string endDate = "")
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2016,3,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BRANCH", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CARDNUM", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CBINCLVOIDS", ParmValue = includeVoids ? "ON" : "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DOMINTL", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2016,3,31", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INACCT", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK1", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK3", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVALCARR", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVCRED", ParmValue = "ALL", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVOICE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "USD", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSFIRST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSLAST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "140", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibSvcFeeDetTrip", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RECLOC", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3379932", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TICKET", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1593", ParmInOut = "IN", LangCode = "" });

            ManipulateReportHandoffRecords(((int)dateType).ToString(), "DATERANGE");

            if (!string.IsNullOrEmpty(invoicesCredits))
                ManipulateReportHandoffRecords(invoicesCredits, "INVCRED");

            if (!string.IsNullOrEmpty(currencyType))
                ManipulateReportHandoffRecords(currencyType, "MONEYTYPE");

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
