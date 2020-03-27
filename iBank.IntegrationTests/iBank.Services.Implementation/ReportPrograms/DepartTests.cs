using System.Linq;

using Domain.Helper;

using iBank.IntegrationTests.TestTools;
using iBank.Services.Implementation.ReportPrograms;
using iBank.Services.Implementation.ReportPrograms.Departures;
using iBank.Services.Implementation.Shared;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.IntegrationTests.iBank.Services.Implementation.ReportPrograms
{
    [TestClass]
    public class DepartTests : BaseUnitTest
    {
        #region Region - Header

        /*
            Reservation & Back Office Report

            Items To Test:
                Date Range Type 
                    Departure Date
                    Invoice Date
                    Booked Date (Reservation Only)
                Sort By:
                    Departure Time -- default in tests
                    Passenger Name
                Use Connecting Legs and Include All Legs (Yes/No) -- No is default in tests
                No Data
                Too Much Data

            Reservation:
                Standard Params:
                    Dates: 12/1/15 - 12/31/15
                    Accounts: 1100, 1188, 1200

                    Alt Dates for Booked Date: 1/1/15 - 1/2/15

            Back Office:
                Standard Params:
                    Dates: 1/1/12 - 3/1/16
                    Accounts: 1100, 1188, 1200

            No Data Params:
                Dates: 3/1/16 - 3/216
                Accounts: 1100, 1188, 1200
                Date Range Type: Departure Date
                Reservation

            Too Much Data Params:
                Dates: 3/1/11 - 3/2/16
                Accounts: All
                Date Range Type: Departure Date
                Reservation

            Create a report with the desired criteria through iBank. Generate a set of reporthandoff objects using this sql script:

            select 'ReportHandoff.Add(new ReportHandoffInformation{ParmName = "' + rtrim(ltrim(Parmname)) + '",ParmValue = "'+rtrim(ltrim(parmvalue))+'",ParmInOut = "' + rtrim(ltrim(parminout))+'", LangCode = "' + rtrim(ltrim(Langcode)) + '"});' 
            from reporthandoff where reportid = '<REPORT ID GOES HERE>' and parminout = 'IN' order by parmname
        */

        #endregion

        #region Region - Enums

        private enum SortBy
        {
            DepartureTime = 1,
            PassengerName = 2
        }

        #endregion

        #region Region - General

        [TestMethod]
        public void NoDataReport()
        {
            GenerateNoDataReportHandoffRecords();

            InsertReportHandoff();

            var rpt = (Depart)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(2, rptInfo.ReturnCode, "Return code failed.");
            var containsNoDataMsg = rptInfo.ErrorMessage.Contains(ReportGlobals.ReportMessages.RptMsg_NoData);
            Assert.AreEqual(true, containsNoDataMsg, "Error message failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void TooMuchDataReport()
        {
            GenerateTooMuchDataReportHandoffRecords();

            InsertReportHandoff();

            var rpt = (Depart)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(2, rptInfo.ReturnCode, "Return code failed.");
            var containsNoDataMsg = rptInfo.ErrorMessage.Contains(ReportGlobals.ReportMessages.RptMsg_BigVolume);
            Assert.AreEqual(true, containsNoDataMsg, "Error message failed.");

            ClearReportHandoff();
        }

        private void GenerateNoDataReportHandoffRecords()
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2016,3,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BRANCH", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CARDNUM", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "4", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DOMINTL", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2016,3,2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "FIRSTDEST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "FIRSTORIGIN", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INACCT", ParmValue = "1100,1188,1200", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INAIRLINE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK1", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK3", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INDESTCOUNTRY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INDESTREGION", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INDESTS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INMETRODESTS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INMETROORGS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INORGS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INORIGCOUNTRY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INORIGREGION", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVALCARR", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVCRED", ParmValue = "ALL", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVOICE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MODE", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "GBP", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSFIRST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSLAST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "6", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibdepart", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RECLOC", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3373785", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "SORTBY", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TICKET", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1595", ParmInOut = "IN", LangCode = "" });
        }

        private void GenerateTooMuchDataReportHandoffRecords()
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2011,3,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BRANCH", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CARDNUM", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "4", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DOMINTL", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2016,3,2", ParmInOut = "IN", LangCode = "" });
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
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INMETRODESTS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INMETROORGS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INORGS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INORIGCOUNTRY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INORIGREGION", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVALCARR", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVCRED", ParmValue = "ALL", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVOICE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MODE", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "GBP", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSFIRST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSLAST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "6", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibdepart", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RECLOC", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3373786", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "SORTBY", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TICKET", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1595", ParmInOut = "IN", LangCode = "" });
        }

        #endregion

        #region Region - Reservation

        [TestMethod]
        public void ReservationReportDepartureDate()
        {
            GenerateReservationReportHandoffRecords();

            InsertReportHandoff();

            var rpt = (Depart)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Error code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //check total number records
            var expectedTotalRecords = 8;
            var actualTotalRecords = rpt.FinalDataList.Count;
            Assert.AreEqual(expectedTotalRecords, actualTotalRecords, "Total records failed.");

            //check total number of departure cities
            var expectedDepartCities = 5;
            var actualDepartCities = rpt.FinalDataList.Select(x => x.Origin).Distinct().Count();
            Assert.AreEqual(expectedDepartCities, actualDepartCities, "Number departure cities failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationReportInvoiceDate()
        {
            GenerateReservationReportHandoffRecords(DateType.InvoiceDate);

            InsertReportHandoff();

            var rpt = (Depart)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Error code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //check total number records
            var expectedTotalRecords = 2;
            var actualTotalRecords = rpt.FinalDataList.Count;
            Assert.AreEqual(expectedTotalRecords, actualTotalRecords, "Total records failed.");

            //check total number of departure cities
            var expectedDepartCities = 2;
            var actualDepartCities = rpt.FinalDataList.Select(x => x.Origin).Distinct().Count();
            Assert.AreEqual(expectedDepartCities, actualDepartCities, "Number departure cities failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationReportBookedDate()
        {
            var begDate = "DT:2015,1,1";
            var endDate = "DT:2015,1,2";
            GenerateReservationReportHandoffRecords(DateType.BookedDate, SortBy.DepartureTime, false, false, begDate, endDate);

            InsertReportHandoff();

            var rpt = (Depart)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Error code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //check total number records
            var expectedTotalRecords = 61;
            var actualTotalRecords = rpt.FinalDataList.Count;
            Assert.AreEqual(expectedTotalRecords, actualTotalRecords, "Total records failed.");

            //check total number of departure cities
            var expectedDepartCities = 26;
            var actualDepartCities = rpt.FinalDataList.Select(x => x.Origin).Distinct().Count();
            Assert.AreEqual(expectedDepartCities, actualDepartCities, "Number departure cities failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationReportSortByDepartureTime()
        {
            GenerateReservationReportHandoffRecords();

            InsertReportHandoff();

            var rpt = (Depart)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Error code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");
            
            var maxCity = rpt.FinalDataList.GroupBy(y => y.OrgDesc).OrderByDescending(z => z.Count()).First().Key.Trim();
            //expected maxCity is "WASHINGTON-RR,DC"

            //find the first last name of the sorted passengers in same origin on same day
            var actualFirstPassLastName = rpt.FinalDataList.Where(x => x.OrgDesc == maxCity).OrderBy(x => x.DepTime).First().PassLast.Trim();
            var expectedFirstPassengerLastName = "CHESHIRE1";
            Assert.AreEqual(expectedFirstPassengerLastName, actualFirstPassLastName, "First passenger in sort by failed.");

            //find the first last name of the sorted passegers in same origin on same day
            var actualLastPassLastName = rpt.FinalDataList.Where(x => x.OrgDesc == maxCity).OrderBy(x => x.DepTime).First().PassLast.Trim();
            var expectedLastPassengerLastName = "ICARIUS";
            Assert.AreEqual(expectedLastPassengerLastName, actualLastPassLastName, "Last passenger in sort by failed.");

            //check record count
            var expectedTotalRecords = 8;
            var actualTotalRecords = rpt.FinalDataList.Count;
            Assert.AreEqual(expectedTotalRecords, actualTotalRecords, "Total records failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationReportSortByPassengerName()
        {
            GenerateReservationReportHandoffRecords(DateType.DepartureDate, SortBy.PassengerName);

            InsertReportHandoff();

            var rpt = (Depart)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Error code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            var maxCity = rpt.FinalDataList.GroupBy(y => y.OrgDesc).OrderByDescending(z => z.Count()).First().Key.Trim();
            //expected maxCity is "WASHINGTON-RR,DC"
            
            //find the first last name of the sorted passengers in same origin on same day
            var actualFirstPassLastName = rpt.FinalDataList.Where(x => x.OrgDesc == maxCity).OrderBy(x => x.PassLast)
                                                                                            .ThenBy(x => x.PassFrst).First().PassLast.Trim();
            var expectedFirstPassengerLastName = "CHESHIRE1";
            Assert.AreEqual(expectedFirstPassengerLastName, actualFirstPassLastName, "First passenger in sort by failed.");

            //find the first last name of the sorted passegers in same origin on same day
            var actualLastPassLastName = rpt.FinalDataList.Where(x => x.OrgDesc == maxCity).OrderBy(x => x.PassLast)
                                                                                           .ThenBy(x => x.PassFrst).First().PassLast.Trim();
            var expectedLastPassengerLastName = "ICARIUS";
            Assert.AreEqual(expectedLastPassengerLastName, actualLastPassLastName, "Last passenger in sort by failed.");

            //check record count
            var expectedTotalRecords = 8;
            var actualTotalRecords = rpt.FinalDataList.Count;
            Assert.AreEqual(expectedTotalRecords, actualTotalRecords, "Total records failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationReportUseConnectingLegsIncludeAllLegs()
        {
            GenerateReservationReportHandoffRecords(DateType.DepartureDate, SortBy.DepartureTime, true, true);

            InsertReportHandoff();

            var rpt = (Depart)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Error code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //count the total records
            var expectedTotalRecords = 24;
            var actualTotalRecords = rpt.FinalDataList.Count;
            Assert.AreEqual(expectedTotalRecords, actualTotalRecords, "Total records failed.");

            //count the total passengers
            var expectedTotalPassengers = 6;
            var actualTotalPassengers = rpt.FinalDataList.Select(x => x.RecKey).Distinct().Count();
            Assert.AreEqual(expectedTotalPassengers, actualTotalPassengers, "Total passengers failed.");

            //count the total origins
            var expectedTotalOrigins = 5;
            var actualTotalOrigins = rpt.FinalDataList.Select(x => x.Origin).Distinct().Count();
            Assert.AreEqual(expectedTotalOrigins, actualTotalOrigins, "Total origins failed.");

            ClearReportHandoff();
        }

        private void GenerateReservationReportHandoffRecords(DateType dateType = DateType.DepartureDate, SortBy sortBy = SortBy.DepartureTime,
            bool useConnectingLegs = false, bool useAllLegs = false, string begDate = "", string endDate = "")
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2015,12,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BRANCH", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CARDNUM", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "4", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DOMINTL", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2015,12,31", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "FIRSTDEST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "FIRSTORIGIN", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INACCT", ParmValue = "1100,1188,1200", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INAIRLINE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK1", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK3", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INDESTCOUNTRY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INDESTREGION", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INDESTS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INMETRODESTS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INMETROORGS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INORGS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INORIGCOUNTRY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INORIGREGION", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVALCARR", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVCRED", ParmValue = "ALL", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVOICE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MODE", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "GBP", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSFIRST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSLAST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "6", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibdepart", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RECLOC", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3373758", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "SORTBY", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TICKET", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1595", ParmInOut = "IN", LangCode = "" });

            if (dateType != DateType.DepartureDate) ManipulateReportHandoffRecords(((int)dateType).ToString(), "DATERANGE");
            if (sortBy != SortBy.DepartureTime) ManipulateReportHandoffRecords(((int)sortBy).ToString(), "SORTBY");
            if (useConnectingLegs) ManipulateReportHandoffRecords("ON", "CBUSECONNECTLEGS");
            if (useAllLegs) ManipulateReportHandoffRecords("ON", "CBINCLUDEALLLEGS");
            if (!string.IsNullOrEmpty(begDate)) ManipulateReportHandoffRecords(begDate, "BEGDATE");
            if (!string.IsNullOrEmpty(endDate)) ManipulateReportHandoffRecords(endDate, "ENDDATE");
        }

        #endregion

        #region Region - Back Office

        [TestMethod]
        public void BackOfficeReportDepartureDate()
        {
            GenerateBackOfficeReportHandoffRecords();

            InsertReportHandoff();

            var rpt = (Depart)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Error code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //check total number records
            var expectedTotalRecords = 10;
            var actualTotalRecords = rpt.FinalDataList.Count;
            Assert.AreEqual(expectedTotalRecords, actualTotalRecords, "Total records failed.");

            //check total number of departure cities
            var expectedDepartCities = 5;
            var actualDepartCities = rpt.FinalDataList.Select(x => x.Origin).Distinct().Count();
            Assert.AreEqual(expectedDepartCities, actualDepartCities, "Number departure cities failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeReportInvoiceDate()
        {
            GenerateBackOfficeReportHandoffRecords(DateType.InvoiceDate);

            InsertReportHandoff();

            var rpt = (Depart)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Error code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //check total number records
            var expectedTotalRecords = 8;
            var actualTotalRecords = rpt.FinalDataList.Count;
            Assert.AreEqual(expectedTotalRecords, actualTotalRecords, "Total records failed.");

            //check total number of departure cities
            var expectedDepartCities = 5;
            var actualDepartCities = rpt.FinalDataList.Select(x => x.Origin).Distinct().Count();
            Assert.AreEqual(expectedDepartCities, actualDepartCities, "Number departure cities failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeReportSortByDepartureTime()
        {
            GenerateBackOfficeReportHandoffRecords();

            InsertReportHandoff();

            var rpt = (Depart)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Error code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //expected maxCity is "LONDON-HEATHROW, UK"
            var maxCity = rpt.FinalDataList.GroupBy(y => y.OrgDesc).OrderByDescending(z => z.Count()).ThenBy(x => x).First().Key.Trim();
            
            //find the first last name of the sorted passengers in same origin on same day
            var actualFirstPassLastName = rpt.FinalDataList.Where(x => x.OrgDesc == maxCity).OrderBy(x => x.DepTime).First().PassLast.Trim();
            var expectedFirstPassengerLastName = "SQUIRREL1";
            Assert.AreEqual(expectedFirstPassengerLastName, actualFirstPassLastName, "First passenger in sort by failed.");

            //find the first last name of the sorted passegers in same origin on same day
            var actualLastPassLastName = rpt.FinalDataList.Where(x => x.OrgDesc == maxCity).OrderBy(x => x.DepTime).First().PassLast.Trim();
            var expectedLastPassengerLastName = "SQUIRREL1";
            Assert.AreEqual(expectedLastPassengerLastName, actualLastPassLastName, "Last passenger in sort by failed.");

            //check record count
            var expectedTotalRecords = 8;
            var actualTotalRecords = rpt.FinalDataList.Count;
            Assert.AreEqual(expectedTotalRecords, actualTotalRecords, "Total records failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeReportSortByPassengerName()
        {
            GenerateBackOfficeReportHandoffRecords(DateType.DepartureDate, SortBy.PassengerName);

            InsertReportHandoff();

            var rpt = (Depart)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Error code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //expected maxCity is "LONDON-HEATHROW, UK"
            var maxCity = rpt.FinalDataList.GroupBy(y => y.OrgDesc).OrderByDescending(z => z.Count()).ThenBy(x => x).First().Key.Trim();

            //find the first last name of the sorted passengers in same origin on same day
            var actualFirstPassLastName = rpt.FinalDataList.Where(x => x.OrgDesc == maxCity).OrderBy(x => x.DepTime).First().PassLast.Trim();
            var expectedFirstPassengerLastName = "SQUIRREL1";
            Assert.AreEqual(expectedFirstPassengerLastName, actualFirstPassLastName, "First passenger in sort by failed.");

            //find the first last name of the sorted passegers in same origin on same day
            var actualLastPassLastName = rpt.FinalDataList.Where(x => x.OrgDesc == maxCity).OrderBy(x => x.DepTime).First().PassLast.Trim();
            var expectedLastPassengerLastName = "SQUIRREL1";
            Assert.AreEqual(expectedLastPassengerLastName, actualLastPassLastName, "Last passenger in sort by failed.");

            //check record count
            var expectedTotalRecords = 8;
            var actualTotalRecords = rpt.FinalDataList.Count;
            Assert.AreEqual(expectedTotalRecords, actualTotalRecords, "Total records failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeReportUseConnectingLegsFalse()
        {
            GenerateBackOfficeReportHandoffRecords(DateType.DepartureDate, SortBy.DepartureTime, true, true);

            InsertReportHandoff();

            var rpt = (Depart)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Error code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //count the total records
            var expectedTotalRecords = 20;
            var actualTotalRecords = rpt.FinalDataList.Count;
            Assert.AreEqual(expectedTotalRecords, actualTotalRecords, "Total records failed.");

            //count the total passengers
            var expectedTotalPassengers = 10;
            var actualTotalPassengers = rpt.FinalDataList.Select(x => x.RecKey).Distinct().Count();
            Assert.AreEqual(expectedTotalPassengers, actualTotalPassengers, "Total passengers failed.");

            //count the total origins
            var expectedTotalOrigins = 5;
            var actualTotalOrigins = rpt.FinalDataList.Select(x => x.Origin).Distinct().Count();
            Assert.AreEqual(expectedTotalOrigins, actualTotalOrigins, "Total origins failed.");

            ClearReportHandoff();
        }

        private void GenerateBackOfficeReportHandoffRecords(DateType dateType = DateType.DepartureDate, SortBy sortBy = SortBy.DepartureTime,
            bool useConnectingLegs = false, bool useAllLegs = false)
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2012,1,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BRANCH", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CARDNUM", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "4", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DOMINTL", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2016,3,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "FIRSTDEST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "FIRSTORIGIN", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INACCT", ParmValue = "1100,1188,1200", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INAIRLINE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK1", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK3", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INDESTCOUNTRY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INDESTREGION", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INDESTS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INMETRODESTS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INMETROORGS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INORGS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INORIGCOUNTRY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INORIGREGION", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVALCARR", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVCRED", ParmValue = "ALL", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVOICE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MODE", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "GBP", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSFIRST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSLAST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = "2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "6", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibdepart", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RECLOC", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3373793", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "SORTBY", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TICKET", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1595", ParmInOut = "IN", LangCode = "" });

            if (dateType != DateType.DepartureDate) ManipulateReportHandoffRecords(((int)dateType).ToString(), "DATERANGE");
            if (sortBy != SortBy.DepartureTime) ManipulateReportHandoffRecords(((int)sortBy).ToString(), "SORTBY");
            if (useConnectingLegs) ManipulateReportHandoffRecords("ON", "CBUSECONNECTLEGS");
            if (useAllLegs) ManipulateReportHandoffRecords("ON", "CBINCLUDEALLLEGS");
        }

        #endregion
    }
}
