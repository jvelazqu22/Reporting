using System.Linq;

using Domain.Helper;

using iBank.IntegrationTests.TestTools;
using iBank.Services.Implementation.ReportPrograms.TravelerByCountry;
using iBank.Services.Implementation.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.IntegrationTests.iBank.Services.Implementation.ReportPrograms
{
    [TestClass]
    public class TravByCountryTests : BaseUnitTest
    {
        /*
     *  Header:
     *  Traveler By Country Report
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
     *          Include One Way trips
     *          Routing Criteria
     *              Origin
     *              Destination
     *          Invoice/Credits
     *          
     *      Reservation
     *          Date Range Type
     *              Departure
     *              Invoice
     *              Booked Date
     *          Include One Way trips
     *          Routing Criteria
     *              Origin
     *              Destination
     *          Invoice/Credits
     *          
     * */

        #region General Test Methods

        [TestMethod]
        public void NoData()
        {
            GenerateCustomHandoffRecords(accnt: "1200");

            InsertReportHandoff();

            //run the report
            var rpt = (TravByCountry)RunReport();
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
            GenerateCustomHandoffRecords(beginDate: "DT:2012,4,1", endDate: "DT:2016,1,1");

            InsertReportHandoff();

            //run the report
            var rpt = (TravByCountry)RunReport();
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
            var rpt = (TravByCountry)RunReport();
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
            GenerateCustomHandoffRecords();

            InsertReportHandoff();

            //Run the report
            var rpt = (TravByCountry)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(219, rpt.FinalDataList.Count, "Final data records failed.");

            //Check for correct totals
            var tickets = rpt.FinalDataList.Sum(s => s.Dispticks);
            Assert.AreEqual(338, tickets, "Total tickets failed.");

            var days = rpt.FinalDataList.Sum(s => s.Totdays);
            Assert.AreEqual(937, days, "Total tickets failed.");

            var maxStay = rpt.FinalDataList.Max(s => s.Longstay);
            Assert.AreEqual(12, maxStay, "Longest stay failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeInvoiceDateType()
        {
            GenerateCustomHandoffRecords(dateType: DateType.InvoiceDate);

            InsertReportHandoff();

            //Run the report
            var rpt = (TravByCountry)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(124, rpt.FinalDataList.Count, "Final data records failed.");

            //Check for correct totals
            var tickets = rpt.FinalDataList.Sum(s => s.Dispticks);
            Assert.AreEqual(164, tickets, "Total tickets failed.");

            var days = rpt.FinalDataList.Sum(s => s.Totdays);
            Assert.AreEqual(423, days, "Total tickets failed.");

            var maxStay = rpt.FinalDataList.Max(s => s.Longstay);
            Assert.AreEqual(12, maxStay, "Longest stay failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeIncludeOneWayTrips()
        {
            GenerateCustomHandoffRecords(includeOneWayTrips: true, beginDate: "DT:2016,4,1");

            InsertReportHandoff();

            //Run the report
            var rpt = (TravByCountry)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(13, rpt.FinalDataList.Count, "Final data records failed.");

            //Check for correct totals
            var tickets = rpt.FinalDataList.Sum(s => s.Dispticks);
            Assert.AreEqual(14, tickets, "Total tickets failed.");

            var days = rpt.FinalDataList.Sum(s => s.Totdays);
            Assert.AreEqual(38, days, "Total tickets failed.");

            var maxStay = rpt.FinalDataList.Max(s => s.Longstay);
            Assert.AreEqual(5, maxStay, "Longest stay failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeOrigin()
        {
            GenerateCustomHandoffRecords(originCity: "NYC");

            InsertReportHandoff();

            //Run the report
            var rpt = (TravByCountry)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(2, rpt.FinalDataList.Count, "Final data records failed.");

            //Check for correct totals
            var tickets = rpt.FinalDataList.Sum(s => s.Dispticks);
            Assert.AreEqual(4, tickets, "Total tickets failed.");

            var days = rpt.FinalDataList.Sum(s => s.Totdays);
            Assert.AreEqual(13, days, "Total tickets failed.");

            var maxStay = rpt.FinalDataList.Max(s => s.Longstay);
            Assert.AreEqual(5, maxStay, "Longest stay failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeDestination()
        {
            GenerateCustomHandoffRecords(destinationCity: "NYC");

            InsertReportHandoff();

            //Run the report
            var rpt = (TravByCountry)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(13, rpt.FinalDataList.Count, "Final data records failed.");

            //Check for correct totals
            var tickets = rpt.FinalDataList.Sum(s => s.Dispticks);
            Assert.AreEqual(18, tickets, "Total tickets failed.");

            var days = rpt.FinalDataList.Sum(s => s.Totdays);
            Assert.AreEqual(51, days, "Total days failed.");

            var maxStay = rpt.FinalDataList.Max(s => s.Longstay);
            Assert.AreEqual(5, maxStay, "Longest stay failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeInvoice()
        {
            GenerateCustomHandoffRecords(invoicesCredits: "INVOICES", beginDate: "DT:2016,5,1");

            InsertReportHandoff();

            //Run the report
            var rpt = (TravByCountry)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(1, rpt.FinalDataList.Count, "Final data records failed.");

            //Check for correct totals
            var tickets = rpt.FinalDataList.Sum(s => s.Dispticks);
            Assert.AreEqual(1, tickets, "Total tickets failed.");

            var days = rpt.FinalDataList.Sum(s => s.Totdays);
            Assert.AreEqual(3, days, "Total tickets failed.");

            var maxStay = rpt.FinalDataList.Max(s => s.Longstay);
            Assert.AreEqual(3, maxStay, "Longest stay failed.");

            ClearReportHandoff();
        }
        #endregion

        #region Reservation Test Methods

        [TestMethod]
        public void ReservationDepartureDateType()
        {
            GenerateCustomHandoffRecords(false, beginDate: "DT:2015,11,1");

            InsertReportHandoff();

            //Run the report
            var rpt = (TravByCountry)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(6, rpt.FinalDataList.Count, "Final data records failed.");

            //Check for correct totals
            var tickets = rpt.FinalDataList.Sum(s => s.Dispticks);
            Assert.AreEqual(10, tickets, "Total tickets failed.");

            var days = rpt.FinalDataList.Sum(s => s.Totdays);
            Assert.AreEqual(25, days, "Total tickets failed.");

            var maxStay = rpt.FinalDataList.Max(s => s.Longstay);
            Assert.AreEqual(6, maxStay, "Longest stay failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationInvoiceDateType()
        {
            GenerateCustomHandoffRecords(false, dateType: DateType.InvoiceDate, beginDate: "DT:2015,11,1");

            InsertReportHandoff();

            //Run the report
            var rpt = (TravByCountry)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(5, rpt.FinalDataList.Count, "Final data records failed.");

            //Check for correct totals
            var tickets = rpt.FinalDataList.Sum(s => s.Dispticks);
            Assert.AreEqual(8, tickets, "Total tickets failed.");

            var days = rpt.FinalDataList.Sum(s => s.Totdays);
            Assert.AreEqual(17, days, "Total tickets failed.");

            var maxStay = rpt.FinalDataList.Max(s => s.Longstay);
            Assert.AreEqual(5, maxStay, "Longest stay failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationBookedDateType()
        {
            GenerateCustomHandoffRecords(false, dateType: DateType.BookedDate, beginDate: "DT:2015,4,1");

            InsertReportHandoff();

            //Run the report
            var rpt = (TravByCountry)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(1, rpt.FinalDataList.Count, "Final data records failed.");

            //Check for correct totals
            var tickets = rpt.FinalDataList.Sum(s => s.Dispticks);
            Assert.AreEqual(2, tickets, "Total tickets failed.");

            var days = rpt.FinalDataList.Sum(s => s.Totdays);
            Assert.AreEqual(9, days, "Total tickets failed.");

            var maxStay = rpt.FinalDataList.Max(s => s.Longstay);
            Assert.AreEqual(6, maxStay, "Longest stay failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationIncludeOneWayTrips()
        {
            GenerateCustomHandoffRecords(isBackOffice: false, includeOneWayTrips: true, beginDate: "DT:2015,6,1");

            InsertReportHandoff();

            //Run the report
            var rpt = (TravByCountry)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(123, rpt.FinalDataList.Count, "Final data records failed.");

            //Check for correct totals
            var tickets = rpt.FinalDataList.Sum(s => s.Dispticks);
            Assert.AreEqual(137, tickets, "Total tickets failed.");

            var days = rpt.FinalDataList.Sum(s => s.Totdays);
            Assert.AreEqual(638, days, "Total tickets failed.");

            var maxStay = rpt.FinalDataList.Max(s => s.Longstay);
            Assert.AreEqual(17, maxStay, "Longest stay failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationOrigin()
        {
            GenerateCustomHandoffRecords(isBackOffice: false, originCity: "NYC", beginDate: "DT:2015,6,1");

            InsertReportHandoff();

            //Run the report
            var rpt = (TravByCountry)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(8, rpt.FinalDataList.Count, "Final data records failed.");

            //Check for correct totals
            var tickets = rpt.FinalDataList.Sum(s => s.Dispticks);
            Assert.AreEqual(8, tickets, "Total tickets failed.");

            var days = rpt.FinalDataList.Sum(s => s.Totdays);
            Assert.AreEqual(57, days, "Total days failed.");

            var maxStay = rpt.FinalDataList.Max(s => s.Longstay);
            Assert.AreEqual(17, maxStay, "Longest stay failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationDestination()
        {
            GenerateCustomHandoffRecords(isBackOffice: false, destinationCity: "NYC", beginDate: "DT:2015,6,1");

            InsertReportHandoff();

            //Run the report
            var rpt = (TravByCountry)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(7, rpt.FinalDataList.Count, "Final data records failed.");

            //Check for correct totals
            var tickets = rpt.FinalDataList.Sum(s => s.Dispticks);
            Assert.AreEqual(7, tickets, "Total tickets failed.");

            var days = rpt.FinalDataList.Sum(s => s.Totdays);
            Assert.AreEqual(20, days, "Total days failed.");

            var maxStay = rpt.FinalDataList.Max(s => s.Longstay);
            Assert.AreEqual(4, maxStay, "Longest stay failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationInvoice()
        {
            GenerateCustomHandoffRecords(isBackOffice: false, invoicesCredits: "INVOICES", beginDate: "DT:2015,8,1");

            InsertReportHandoff();

            //Run the report
            var rpt = (TravByCountry)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(26, rpt.FinalDataList.Count, "Final data records failed.");

            //Check for correct totals
            var tickets = rpt.FinalDataList.Sum(s => s.Dispticks);
            Assert.AreEqual(33, tickets, "Total tickets failed.");

            var days = rpt.FinalDataList.Sum(s => s.Totdays);
            Assert.AreEqual(164, days, "Total tickets failed.");

            var maxStay = rpt.FinalDataList.Max(s => s.Longstay);
            Assert.AreEqual(17, maxStay, "Longest stay failed.");

            ClearReportHandoff();
        }

        #endregion

        #region Generate Report Handoff Records Helpers

        private void GenerateCustomHandoffRecords(bool isBackOffice = true, DateType dateType = DateType.DepartureDate, string accnt = "", string invoicesCredits = "", bool includeOneWayTrips = false,
            string beginDate = "", string endDate = "", string originCity = "", string destinationCity = "")
        {

            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2016,2,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CBINCLUDEONEWAY", ParmValue = includeOneWayTrips ? "ON" : "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2016,6,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVCRED", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INMETROORGS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = isBackOffice ? "2" : "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "205", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibTravByCountry", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3381234", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1593", ParmInOut = "IN", LangCode = "" });
           

            ManipulateReportHandoffRecords(((int)dateType).ToString(), "DATERANGE");

            if (!string.IsNullOrEmpty(invoicesCredits))
                ManipulateReportHandoffRecords(invoicesCredits, "INVCRED");

            if (!string.IsNullOrEmpty(accnt))
                ManipulateReportHandoffRecords(accnt, "INACCT");

            if (!string.IsNullOrEmpty(beginDate))
                ManipulateReportHandoffRecords(beginDate, "BEGDATE");

            if (!string.IsNullOrEmpty(endDate))
                ManipulateReportHandoffRecords(endDate, "ENDDATE");

            if (!string.IsNullOrEmpty(originCity))
                ManipulateReportHandoffRecords(originCity, "INMETROORGS");

            if (!string.IsNullOrEmpty(destinationCity))
                ManipulateReportHandoffRecords(destinationCity, "INMETRODESTS");

        }


        #endregion
    }
}
