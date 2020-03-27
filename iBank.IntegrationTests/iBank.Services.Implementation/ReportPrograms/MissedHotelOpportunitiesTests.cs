using System.Linq;

using Domain.Helper;

using iBank.IntegrationTests.TestTools;
using iBank.Server.Utilities;
using iBank.Services.Implementation;
using iBank.Services.Implementation.ReportPrograms.MissedHotelOpportunities;
using iBank.Services.Implementation.Shared;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.IntegrationTests.iBank.Services.Implementation.ReportPrograms
{
    /*
     *  Header:
     *  Missed Hotel Opportunities Report
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
     *              On-the-road
     *          Exclude Car-only Bookings
     *          Include trips where hotels are present
     *          Exclude Car-only Bookings and Include trips where hotels are present
     *          Trip Duration
     *          Group By Home Country
     *          Apply Origin/Destination Criteria
     *              Leg level
     *              Seg level
     *          
     *      Reservation
     *          Date Range Type
     *              Departure
     *              Invoice
     *              Booked Date
     *              on-the-road
     *          Exclude Car-only Bookings
     *          Include trips where hotels are present
     *          Exclude Car-only Bookings and Include trips where hotels are present
     *          Trip Duration
     *          Group By Home Country
     *          Apply Origin/Destination Criteria
     *              Leg level
     *              Seg level
     *          
     *          
     * */
    [TestClass]
    public class MissedHotelOpportunitiesTests : BaseUnitTest
    {
        #region General Test Methods

        [TestMethod]
        public void NoData()
        {
            GenerateCustomHandoffRecords(account: "1200");

            InsertReportHandoff();

            //run the report
            var rpt = (MissedHotel)RunReport();
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
            GenerateCustomHandoffRecords(beginDate: "DT:2014,1,1");

            InsertReportHandoff();

            //run the report
            var rpt = (MissedHotel)RunReport();
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
            var rpt = (MissedHotel)RunReport();
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
            var rpt = (MissedHotel)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(113, rpt.FinalDataList.Count, "Final data records failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeInvoiceDateType()
        {
            GenerateCustomHandoffRecords(dateType: DateType.InvoiceDate);

            InsertReportHandoff();

            //Run the report
            var rpt = (MissedHotel)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(120, rpt.FinalDataList.Count, "Final data records failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeOnTheRoadDateType()
        {
            GenerateCustomHandoffRecords(dateType: DateType.OnTheRoadDatesSpecial);

            InsertReportHandoff();

            //Run the report
            var rpt = (MissedHotel)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(114, rpt.FinalDataList.Count, "Final data records failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeExcludeCarOnly()
        {
            GenerateCustomHandoffRecords(excludeCarOnly: true, beginDate: "DT:2015,6,1");

            InsertReportHandoff();

            //Run the report
            var rpt = (MissedHotel)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(812, rpt.FinalDataList.Count, "Final data records failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeIncludeTripsWithHotels()
        {
            GenerateCustomHandoffRecords(includeTripsWithHotel: true, beginDate: "DT:2015,6,1");

            InsertReportHandoff();

            //Run the report
            var rpt = (MissedHotel)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(1444, rpt.FinalDataList.Count, "Final data records failed.");

            //Check the hotels without booking
            var hotelsWithoutBookings = rpt.FinalDataList.Count(s => s.Hotelbkd == "No ");
            Assert.AreEqual(855, hotelsWithoutBookings, "Trips without hotel bookings failed");


            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeExcludeCarAndIncludeTripsWithHotels()
        {
            GenerateCustomHandoffRecords(excludeCarOnly: true, includeTripsWithHotel: true, beginDate: "DT:2015,6,1");

            InsertReportHandoff();

            //Run the report
            var rpt = (MissedHotel)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(1401, rpt.FinalDataList.Count, "Final data records failed.");

            //Check the hotels without booking
            var hotelsWithoutBookings = rpt.FinalDataList.Count(s => s.Hotelbkd == "No ");
            Assert.AreEqual(812, hotelsWithoutBookings, "Trips without hotel bookings failed");


            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeTripDuration()
        {
            GenerateCustomHandoffRecords(tripDuration: 5, beginDate: "DT:2015,1,1");

            InsertReportHandoff();

            //Run the report
            var rpt = (MissedHotel)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(229, rpt.FinalDataList.Count, "Final data records failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeGroupByHomeCountry()
        {
            GenerateCustomHandoffRecords(groupByHomeCountry: true, tripDuration: 5, beginDate: "DT:2015,1,1");

            InsertReportHandoff();

            //Run the report
            var rpt = (MissedHotel)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(229, rpt.FinalDataList.Count, "Final data records failed.");

            //Check the country name
            Assert.AreEqual(229, rpt.FinalDataList.Count(s => s.Homectry.EqualsIgnoreCase("CANADA")), "Group by country failed");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeApplyToLegsOrigin()
        {
            GenerateCustomHandoffRecords(beginDate: "DT:2015,1,1", applyAtLegLevel: true, originCity: "ATL");

            InsertReportHandoff();

            //Run the report
            var rpt = (MissedHotel)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(10, rpt.FinalDataList.Count, "Final data records failed.");

            //Check the origin name
            Assert.AreEqual(10, rpt.FinalDataList.Count(s => s.Itinerary.ToUpper().Contains("ATL")), "Origin and apply to leg level failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeApplyToLegsDestination()
        {
            GenerateCustomHandoffRecords(beginDate: "DT:2015,1,1", applyAtLegLevel: true, destinationCity: "SFO");

            InsertReportHandoff();

            //Run the report
            var rpt = (MissedHotel)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(9, rpt.FinalDataList.Count, "Final data records failed.");

            //Check the origin name
            Assert.AreEqual(9, rpt.FinalDataList.Count(s => s.Itinerary.ToUpper().Contains("SFO")), "Destination and apply to leg level failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeApplyToSegmentsOrigin()
        {
            GenerateCustomHandoffRecords(beginDate: "DT:2015,1,1", applyAtLegLevel: false, originCity: "SEA");

            InsertReportHandoff();

            //Run the report
            var rpt = (MissedHotel)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(1, rpt.FinalDataList.Count, "Final data records failed.");

            //Check the origin name
            Assert.AreEqual(1, rpt.FinalDataList.Count(s => s.Itinerary.ToUpper().Contains("SEA")), "Origin and apply to segment level failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeApplyToSegmentsDestination()
        {
            GenerateCustomHandoffRecords(beginDate: "DT:2015,1,1", applyAtLegLevel: false, destinationCity: "DFW");

            InsertReportHandoff();

            //Run the report
            var rpt = (MissedHotel)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(4, rpt.FinalDataList.Count, "Final data records failed.");

            //Check the origin name
            Assert.AreEqual(4, rpt.FinalDataList.Count(s => s.Itinerary.ToUpper().Contains("DFW")), "Destination and apply to segment level failed.");

            ClearReportHandoff();
        }

        #endregion

        #region Reservation Test Methods

        [TestMethod]
        public void ReservationDepartureDateType()
        {
            GenerateCustomHandoffRecords(isBackOffice: false, beginDate: "DT:2015,1,1", account: "1200");

            InsertReportHandoff();

            //Run the report
            var rpt = (MissedHotel)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(145, rpt.FinalDataList.Count, "Final data records failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationInvoiceDateType()
        {
            GenerateCustomHandoffRecords(isBackOffice: false, dateType: DateType.InvoiceDate, beginDate: "DT:2015,1,1", account: "1200");

            InsertReportHandoff();

            //Run the report
            var rpt = (MissedHotel)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(130, rpt.FinalDataList.Count, "Final data records failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationOnTheRoadDateType()
        {
            GenerateCustomHandoffRecords(isBackOffice: false, dateType: DateType.OnTheRoadDatesSpecial, beginDate: "DT:2015,1,1", account: "1200");

            InsertReportHandoff();

            //Run the report
            var rpt = (MissedHotel)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(145, rpt.FinalDataList.Count, "Final data records failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservatioBookedDateType()
        {
            GenerateCustomHandoffRecords(isBackOffice: false, dateType: DateType.BookedDate, beginDate: "DT:2015,1,1", account: "1200");

            InsertReportHandoff();

            //Run the report
            var rpt = (MissedHotel)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(145, rpt.FinalDataList.Count, "Final data records failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationExcludeCarOnly()
        {
            GenerateCustomHandoffRecords(isBackOffice: false, excludeCarOnly: true, beginDate: "DT:2015,1,1", endDate: "DT:2015,1,5");

            InsertReportHandoff();

            //Run the report
            var rpt = (MissedHotel)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(28, rpt.FinalDataList.Count, "Final data records failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationIncludeTripsWithHotels()
        {
            GenerateCustomHandoffRecords(isBackOffice: false, includeTripsWithHotel: true, beginDate: "DT:2015,1,1", endDate: "DT:2015,1,5");

            InsertReportHandoff();

            //Run the report
            var rpt = (MissedHotel)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(222, rpt.FinalDataList.Count, "Final data records failed.");

            //Check the hotels without booking
            var hotelsWithoutBookings = rpt.FinalDataList.Count(s => s.Hotelbkd == "No ");
            Assert.AreEqual(75, hotelsWithoutBookings, "Trips without hotel bookings failed");


            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationExcludeCarAndIncludeTripsWithHotels()
        {
            GenerateCustomHandoffRecords(isBackOffice: false, excludeCarOnly: true, includeTripsWithHotel: true, beginDate: "DT:2015,1,1", endDate: "DT:2015,1,5");

            InsertReportHandoff();

            //Run the report
            var rpt = (MissedHotel)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(175, rpt.FinalDataList.Count, "Final data records failed.");

            //Check the hotels without booking
            var hotelsWithoutBookings = rpt.FinalDataList.Count(s => s.Hotelbkd == "No ");
            Assert.AreEqual(28, hotelsWithoutBookings, "Trips without hotel bookings failed");


            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationTripDuration()
        {
            GenerateCustomHandoffRecords(isBackOffice: false, tripDuration: 5, beginDate: "DT:2015,1,1", endDate: "DT:2015,1,5");

            InsertReportHandoff();

            //Run the report
            var rpt = (MissedHotel)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(17, rpt.FinalDataList.Count, "Final data records failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationGroupByHomeCountry()
        {
            GenerateCustomHandoffRecords(isBackOffice: false, groupByHomeCountry: true, tripDuration: 5, beginDate: "DT:2015,1,1", endDate: "DT:2015,1,5");

            InsertReportHandoff();

            //Run the report
            var rpt = (MissedHotel)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(17, rpt.FinalDataList.Count, "Final data records failed.");

            //Check the country name
            Assert.AreEqual(17, rpt.FinalDataList.Count(s => s.Homectry.EqualsIgnoreCase("CANADA")), "Group by country failed");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationApplyToLegsOrigin()
        {
            GenerateCustomHandoffRecords(isBackOffice: false, beginDate: "DT:2015,1,1", endDate: "DT:2015,1,5", applyAtLegLevel: true, originCity: "ATL");

            InsertReportHandoff();

            //Run the report
            var rpt = (MissedHotel)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(5, rpt.FinalDataList.Count, "Final data records failed.");

            //Check the origin name
            Assert.AreEqual(5, rpt.FinalDataList.Count(s => s.Itinerary.ToUpper().Contains("ATL")), "Origin and apply to leg level failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationApplyToLegsDestination()
        {
            GenerateCustomHandoffRecords(isBackOffice: false, beginDate: "DT:2015,1,1", endDate: "DT:2015,1,5", applyAtLegLevel: true, destinationCity: "SFO");

            InsertReportHandoff();

            //Run the report
            var rpt = (MissedHotel)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(3, rpt.FinalDataList.Count, "Final data records failed.");

            //Check the origin name
            Assert.AreEqual(3, rpt.FinalDataList.Count(s => s.Itinerary.ToUpper().Contains("SFO")), "Destination and apply to leg level failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationApplyToSegmentsOrigin()
        {
            GenerateCustomHandoffRecords(isBackOffice: false, beginDate: "DT:2015,1,1", endDate: "DT:2015,1,5", applyAtLegLevel: false, originCity: "SEA");

            InsertReportHandoff();

            //Run the report
            var rpt = (MissedHotel)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(1, rpt.FinalDataList.Count, "Final data records failed.");

            //Check the origin name
            Assert.AreEqual(1, rpt.FinalDataList.Count(s => s.Itinerary.ToUpper().Contains("SEA")), "Origin and apply to segment level failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationApplyToSegmentsDestination()
        {
            GenerateCustomHandoffRecords(isBackOffice: false, beginDate: "DT:2015,1,1", endDate: "DT:2015,1,5", applyAtLegLevel: false, destinationCity: "LAX");

            InsertReportHandoff();

            //Run the report
            var rpt = (MissedHotel)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there is the right number of FinalData records. 
            Assert.AreEqual(1, rpt.FinalDataList.Count, "Final data records failed.");

            //Check the origin name
            Assert.AreEqual(1, rpt.FinalDataList.Count(s => s.Itinerary.ToUpper().Contains("LAX")), "Destination and apply to segment level failed.");

            ClearReportHandoff();
        }

        #endregion

        #region Generate Report Handoff Records Helpers

        private void GenerateCustomHandoffRecords(bool isBackOffice = true, bool excludeCarOnly = false, bool includeTripsWithHotel = false, bool groupByHomeCountry = false, bool breakByAgentId = false, int tripDuration = 1, DateType dateType = DateType.DepartureDate, string account = "",
            string beginDate = "", string endDate = "", bool applyAtLegLevel = true, string originCity = "", string destinationCity = "")
        {

            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2016,1,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BRANCH", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CARDNUM", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CBBRKBYAGENTID", ParmValue = breakByAgentId ? "ON" : "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CBEXCLUDECARONLY", ParmValue = excludeCarOnly ? "ON" : "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CBINCLTRIPSWITHHOTELS", ParmValue = includeTripsWithHotel ? "ON" : "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CBGRPBYHOMECTRY", ParmValue = groupByHomeCountry ? "ON" : "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DOMINTL", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2016,1,30", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "FIRSTDEST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "FIRSTORIGIN", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INACCT", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INAIRLINE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK1", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK3", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INDESTCOUNTRY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INDESTREGION", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INDESTS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INHOMECTRY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INMETRODESTS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INMETROORGS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INORGS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INORIGCOUNTRY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INORIGREGION", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVALCARR", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVCRED", ParmValue = "ALL", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVOICE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MODE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "USD", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "NBRDAYSDURATION", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSFIRST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSLAST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = isBackOffice ? "2" : "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "88", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibMissedHotel", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RBAPPLYTOLEGORSEG", ParmValue = applyAtLegLevel ? "1" : "2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RECLOC", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3380593", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TICKET", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1593", ParmInOut = "IN", LangCode = "" });

            ManipulateReportHandoffRecords(((int)dateType).ToString(), "DATERANGE");

            ManipulateReportHandoffRecords(tripDuration.ToString(), "NBRDAYSDURATION");

            if (!string.IsNullOrEmpty(originCity))
                ManipulateReportHandoffRecords(originCity, "INORGS");

            if (!string.IsNullOrEmpty(destinationCity))
                ManipulateReportHandoffRecords(destinationCity, "INDESTS");

            if (!string.IsNullOrEmpty(account))
                ManipulateReportHandoffRecords(account, "INACCT");

            if (!string.IsNullOrEmpty(beginDate))
                ManipulateReportHandoffRecords(beginDate, "BEGDATE");

            if (!string.IsNullOrEmpty(endDate))
                ManipulateReportHandoffRecords(endDate, "ENDDATE");

        }


        #endregion
    }
}
