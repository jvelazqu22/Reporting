using System.Linq;

using Domain.Helper;

using iBank.IntegrationTests.TestTools;
using iBank.Services.Implementation.ReportPrograms.NegotiatedSavings;
using iBank.Services.Implementation.Shared;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.IntegrationTests.iBank.Services.Implementation.ReportPrograms
{
    /*
     *  Header:
     *  Negotiated Savings Report
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
     *          Currency Conversion
     *          
     *      Reservation
     *          Date Range Type
     *              Departure
     *              Invoice
     *              Booked Date
     *          Currency Conversion
     *          
     *          
     * */
    [TestClass]
    public class NegoSaveTests : BaseUnitTest
    {
        #region General Test Methods

        [TestMethod]
        public void NoData()
        {
            GenerateCustomHandoffRecords(beginDate: "DT:2016,3,1", endDate: "DT:2016,3,5", accnt: "1200");

            InsertReportHandoff();

            //run the report
            var rpt = (NegoSave)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(2, rptInfo.ReturnCode, "Return code failed.");
            var containsNoDataMsg = rptInfo.ErrorMessage.Contains(ReportGlobals.ReportMessages.RptMsg_NoData);
            Assert.AreEqual(true, containsNoDataMsg, "Error message failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void TooMuchData()
        {
            GenerateCustomHandoffRecords();

            InsertReportHandoff();

            //run the report
            var rpt = (NegoSave)RunReport();

            //artificially lower the record limit to trip the conditional
            rpt.Globals.RecordLimit = 5;
            rpt.RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(2, rptInfo.ReturnCode, "Return code failed.");
            var containsNoDataMsg = rptInfo.ErrorMessage.Contains(ReportGlobals.ReportMessages.RptMsg_BigVolume);
            Assert.AreEqual(true, containsNoDataMsg, "Error message failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BadDateRange()
        {
            GenerateCustomHandoffRecords(beginDate: "DT:2016,2,1", endDate: "DT:2016,1,1");

            InsertReportHandoff();

            //run the report
            var rpt = (NegoSave)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(2, rptInfo.ReturnCode, "Return code failed.");
            var containsBadDateRangeMsg = rptInfo.ErrorMessage.Contains(ReportGlobals.ReportMessages.RptMsg_DateRange);
            Assert.AreEqual(true, containsBadDateRangeMsg, "Error message failed.");

            ClearReportHandoff();
        }

        #endregion

        #region BackOffice Test Methods

        [TestMethod]
        public void BackOfficeDepartureDateType()
        {
            GenerateCustomHandoffRecords(beginDate: "DT:2015,1,1", endDate: "DT:2015,12,1");

            InsertReportHandoff();

            //Run the report
            var rpt = (NegoSave)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(32, rpt.FinalDataList.Count, "Final data records failed.");

            //Check for correct savings
            var savings = rpt.FinalDataList.Select(s => new {s.Reckey, s.Negosvgs}).Distinct().Sum(s => s.Negosvgs);
            Assert.AreEqual((decimal)-347.91, savings, "Savings failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeInvoiceDateType()
        {
            GenerateCustomHandoffRecords(dateType: DateType.InvoiceDate, accnt: "9000521", beginDate: "DT:2015,1,1", endDate: "DT:2015,12,1");

            InsertReportHandoff();

            //Run the report
            var rpt = (NegoSave)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(4, rpt.FinalDataList.Count, "Final data records failed.");

            //Check for correct savings
            var savings = rpt.FinalDataList.Select(s => new { s.Reckey, s.Negosvgs }).Distinct().Sum(s => s.Negosvgs);
            Assert.AreEqual((decimal)240.03, savings, "Savings failed.");

            ClearReportHandoff();
        }

     

        [TestMethod]
        public void BackOfficeCurrencyConversion()
        {
            GenerateCustomHandoffRecords(currencyType: "GBP", dateType: DateType.InvoiceDate, beginDate: "DT:2015,1,1", endDate: "DT:2015,12,1");

            InsertReportHandoff();

            //Run the report
            var rpt = (NegoSave)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(34, rpt.FinalDataList.Count, "Final data records failed.");

            //Check for correct savings
            var savings = rpt.FinalDataList.Select(s => new { s.Reckey, s.Negosvgs }).Distinct().Sum(s => s.Negosvgs);
            Assert.AreEqual((decimal)-228.00, savings, "Savings failed.");

            ClearReportHandoff();
        }


        #endregion

        #region Reservation Test Methods

        [TestMethod]
        public void ReservationDepartureDateType()
        {
            GenerateCustomHandoffRecords(false, beginDate: "DT:2015,1,1", endDate: "DT:2015,2,1", accnt: "1200");

            InsertReportHandoff();

            //Run the report
            var rpt = (NegoSave)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(23, rpt.FinalDataList.Count, "Final data records failed.");

            //Check for correct savings
            var savings = rpt.FinalDataList.Select(s => new { s.Reckey, s.Negosvgs }).Distinct().Sum(s => s.Negosvgs);
            Assert.AreEqual((decimal)14475.85, savings, "Savings failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationInvoiceDateType()
        {
            GenerateCustomHandoffRecords(false, dateType: DateType.InvoiceDate, beginDate: "DT:2015,1,1", endDate: "DT:2015,2,1", accnt: "1200");

            InsertReportHandoff();

            //Run the report
            var rpt = (NegoSave)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(43, rpt.FinalDataList.Count, "Final data records failed.");

            //Check for correct savings
            var savings = rpt.FinalDataList.Select(s => new { s.Reckey, s.Negosvgs }).Distinct().Sum(s => s.Negosvgs);
            Assert.AreEqual((decimal)15819.13, savings, "Savings failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationBookedDateType()
        {
            GenerateCustomHandoffRecords(false, dateType: DateType.BookedDate, beginDate: "DT:2015,1,1", endDate: "DT:2015,2,1", accnt: "1200");

            InsertReportHandoff();

            //Run the report
            var rpt = (NegoSave)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(57, rpt.FinalDataList.Count, "Final data records failed.");

            //Check for correct savings
            var savings = rpt.FinalDataList.Select(s => new { s.Reckey, s.Negosvgs }).Distinct().Sum(s => s.Negosvgs);
            Assert.AreEqual((decimal)29361.58, savings, "Savings failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationCurrencyConversion()
        {
            GenerateCustomHandoffRecords(false, currencyType: "GBP", accnt: "1200", beginDate: "DT:2015,1,1", endDate: "DT:2015,2,1");

            InsertReportHandoff();

            //Run the report
            var rpt = (NegoSave)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(23, rpt.FinalDataList.Count, "Final data records failed.");

            //Check for correct savings
            var savings = rpt.FinalDataList.Select(s => new { s.Reckey, s.Negosvgs }).Distinct().Sum(s => s.Negosvgs);
            Assert.AreEqual((decimal)9628.91, savings, "Savings failed.");

            ClearReportHandoff();
        }

        #endregion

        #region Generate Report Handoff Records Helpers

        private void GenerateCustomHandoffRecords(bool isBackOffice = true, DateType dateType = DateType.DepartureDate, string accnt = "", string invoicesCredits = "", string currencyType = "",
            string beginDate = "", string endDate = "", string domesticInternational = "")
        {

            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2015,2,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BRANCH", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CARDNUM", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DOMINTL", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2016,3,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INACCT", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INAIRLINE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK1", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK3", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INDESTS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INORGS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVALCARR", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVOICE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MODE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "USD", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSFIRST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSLAST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = isBackOffice ? "2" : "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "162", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibNegoSave", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RECLOC", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3380272", ParmInOut = "IN", LangCode = "" });
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

            if (!string.IsNullOrEmpty(domesticInternational))
                ManipulateReportHandoffRecords(domesticInternational, "DOMINTL");

        }


        #endregion
    }
}
