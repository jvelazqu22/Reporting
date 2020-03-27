using Domain.Helper;
using iBank.IntegrationTests.TestTools;
using iBank.Services.Implementation.ReportPrograms.TravelDetail;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.IntegrationTests.iBank.Services.Implementation.ReportPrograms
{
    [TestClass]
    public class TravDet1Tests : BaseUnitTest
    {
        #region Region - Header Info

        /*
           Reservation & Back Office Report

           Items to test:
               Date Range Type
                    Departure Date
                    Invoice Date
                    On the road dates
                    Booked Date - Reservation Report Only

               Number of passengers
               No data
               Too Much Data

           Standard Parameters Back Office:
               Dates: 1/1/16 - 2/27/16
               Accounts: All

           Standard Parameters Reservation:
               Accounts: All

               Departure Date: 7/1/15 - 2/27/16
               Invoice Date: 4/1/15 - 2/27/16
               On The Road Dates: 6/1/15 - 2/27/16
               Booked Date: 1/1/15 - 1/31/15
                            Account: 1100, 1188, 1200

           No Data Params:
                Dates: 2/25/16 - 2/26/16
                Date Range Type: Departure Date
                Accounts: 1100
                Back Office

           Too Much Data Params:
                Dates: 2/25/12 - 2/26/16
                Date Range Type: Departure Date
                Accounts: all
                Back Office

           Report Id:

           Create a report with the desired criteria through iBank. Generate a set of reporthandoff objects using this sql script:

           select 'ReportHandoff.Add(new ReportHandoffInformation{ParmName = "' + rtrim(ltrim(Parmname)) + '",ParmValue = "'+rtrim(ltrim(parmvalue))+'",ParmInOut = "' + rtrim(ltrim(parminout))+'", LangCode = "' + rtrim(ltrim(Langcode)) + '"});'
           from reporthandoff where reportid = '<REPORT ID GOES HERE>' and parminout = 'IN' order by parmname

           select 'ReportHandoff.Add(new ReportHandoffInformation{ParmName = "' + rtrim(ltrim(Parmname)) + '",ParmValue = "'+rtrim(ltrim(parmvalue))+'",ParmInOut = "' + rtrim(ltrim(parminout))+'", LangCode = "' + rtrim(ltrim(Langcode)) + '"});' 
           from reporthandoff where reportid = (select top 1 reportid from reporthandoff where usernumber = 1592 order by datecreated desc) and parminout = 'IN' order by parmname

       */

        #endregion Region - Header Info

        #region General
        [TestMethod]
        public void TooMuchData()
        {
            GenerateReportHandoffRecordsTooMuchData();
            InsertReportHandoff();
            var rpt = (TravDet1)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;
            Assert.AreEqual(2, rptInfo.ReturnCode, "Return code failed.");
            var containsTooMuchDataMsg = rptInfo.ErrorMessage.Contains(ReportGlobals.ReportMessages.RptMsg_BigVolume);
            Assert.AreEqual(true, containsTooMuchDataMsg, "Error message failed.");
            ClearReportHandoff();
        }

        [TestMethod]
        public void NoData()
        {
            GenerateReportHandoffRecordsNoData();

            InsertReportHandoff();

            var rpt = (TravDet1)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(2, rptInfo.ReturnCode, "Return code failed.");
            var containsNoDataMsg = rptInfo.ErrorMessage.Contains(ReportGlobals.ReportMessages.RptMsg_NoData);
            Assert.AreEqual(true, containsNoDataMsg, "Error message failed.");

            ClearReportHandoff();
        }
        [TestMethod]
        private void GenerateReportHandoffRecordsNoData()
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2016,2,26", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DDAIRRAILCARHOTELOPTIONS", ParmValue = "All Records", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2016,2,26", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INACCT", ParmValue = "1100", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "USD", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = "2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "23", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibTravDet1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RBAPPLYTOLEGORSEG", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3378582", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1592", ParmInOut = "IN", LangCode = "" });
        }

        [TestMethod]
        private void GenerateReportHandoffRecordsTooMuchData()
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2016,2,15", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DDAIRRAILCARHOTELOPTIONS", ParmValue = "All Records", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2016,2,26", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "USD", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = "2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "23", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibTravDet1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RBAPPLYTOLEGORSEG", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3378581", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1592", ParmInOut = "IN", LangCode = "" });
        }

        #endregion General

        #region - Reservation

        [TestMethod]
        public void ReservationReportBookedDate()
        {
            GenerateReservationHandoffRecords(DateType.BookedDate);

            //Inserts the records into the database under the Test User id.
            InsertReportHandoff();

            //run the report
            var rpt = (TravDet1)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded.
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            Assert.AreEqual(13, rpt.TravDetShared.Cars.Count, "Car data records failed.");
            Assert.AreEqual(41, rpt.TravDetShared.RawDataList.Count, "Raw data records failed.");
            Assert.AreEqual(76, rpt.TravDetShared.Segments.Count, "Segment data records failed.");
            Assert.AreEqual(89, rpt.TravDetShared.Legs.Count, "Leg data records failed.");
            Assert.AreEqual(27, rpt.TravDetShared.Hotels.Count, "Hotel data records failed.");
            Assert.AreEqual(170, rpt.FinalDataList.Count, "Final data records failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationReportDepartureDate()
        {
            GenerateReservationHandoffRecords(DateType.DepartureDate);

            //Inserts the records into the database under the Test User id.
            InsertReportHandoff();

            //run the report
            var rpt = (TravDet1)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded.
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            Assert.AreEqual(11, rpt.TravDetShared.Cars.Count, "Car data records failed.");
            Assert.AreEqual(26, rpt.TravDetShared.RawDataList.Count, "Raw data records failed.");
            Assert.AreEqual(61, rpt.TravDetShared.Segments.Count, "Segment data records failed.");
            Assert.AreEqual(67, rpt.TravDetShared.Legs.Count, "Leg data records failed.");
            Assert.AreEqual(26, rpt.TravDetShared.Hotels.Count, "Hotel data records failed.");
            Assert.AreEqual(130, rpt.FinalDataList.Count, "Final data records failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationReportInvoiceDateNoCar()
        {
            GenerateReservationHandoffRecords(DateType.InvoiceDate,airRailCarHotelOptions: "NO CAR");

            //Inserts the records into the database under the Test User id.
            InsertReportHandoff();

            //run the report
            var rpt = (TravDet1)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded.
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            Assert.AreEqual(9, rpt.TravDetShared.Cars.Count, "Car data records failed.");
            Assert.AreEqual(30, rpt.TravDetShared.RawDataList.Count, "Raw data records failed.");
            Assert.AreEqual(55, rpt.TravDetShared.Segments.Count, "Segment data records failed.");
            Assert.AreEqual(61, rpt.TravDetShared.Legs.Count, "Leg data records failed.");
            Assert.AreEqual(24, rpt.TravDetShared.Hotels.Count, "Hotel data records failed.");
            Assert.AreEqual(80, rpt.FinalDataList.Count, "Final data records failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationReportOnTheRoadDate()
        {
            GenerateReservationHandoffRecords(DateType.OnTheRoadDatesSpecial);

            //Inserts the records into the database under the Test User id.
            InsertReportHandoff();

            //run the report
            var rpt = (TravDet1)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded.
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            Assert.AreEqual(40, rpt.TravDetShared.Cars.Count, "Car data records failed.");
            Assert.AreEqual(117, rpt.TravDetShared.RawDataList.Count, "Raw data records failed.");
            Assert.AreEqual(196, rpt.TravDetShared.Segments.Count, "Segment data records failed.");
            Assert.AreEqual(218, rpt.TravDetShared.Legs.Count, "Leg data records failed.");
            Assert.AreEqual(103, rpt.TravDetShared.Hotels.Count, "Hotel data records failed.");
            Assert.AreEqual(478, rpt.FinalDataList.Count, "Final data records failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationReportOnTheRoadHotelOnlyDate()
        {
            GenerateReservationHandoffRecords(DateType.OnTheRoadDatesSpecial, airRailCarHotelOptions: "HOTEL ONLY");

            //Inserts the records into the database under the Test User id.
            InsertReportHandoff();

            //run the report
            var rpt = (TravDet1)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded.
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            Assert.AreEqual(40, rpt.TravDetShared.Cars.Count, "Car data records failed.");
            Assert.AreEqual(117, rpt.TravDetShared.RawDataList.Count, "Raw data records failed.");
            Assert.AreEqual(196, rpt.TravDetShared.Segments.Count, "Segment data records failed.");
            Assert.AreEqual(218, rpt.TravDetShared.Legs.Count, "Leg data records failed.");
            Assert.AreEqual(103, rpt.TravDetShared.Hotels.Count, "Hotel data records failed.");
            Assert.AreEqual(45, rpt.FinalDataList.Count, "Final data records failed.");

            ClearReportHandoff();
        }

        #endregion - Reservation

        #region Region - Back Office


 
        [TestMethod]
        public void BackOfficeReportDepartureDate()
        {
            GenerateBackOfficeHandoffRecords(DateType.DepartureDate);

            //Inserts the records into the database under the Test User id.
            InsertReportHandoff();

            //run the report
            var rpt = (TravDet1)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded.
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            Assert.AreEqual(3, rpt.TravDetShared.Cars.Count, "Car data records failed.");
            Assert.AreEqual(20, rpt.TravDetShared.RawDataList.Count, "Raw data records failed.");
            Assert.AreEqual(29, rpt.TravDetShared.Segments.Count, "Segment data records failed.");
            Assert.AreEqual(31, rpt.TravDetShared.Legs.Count, "Leg data records failed.");
            Assert.AreEqual(6, rpt.TravDetShared.Hotels.Count, "Hotel data records failed.");
            Assert.AreEqual(58, rpt.FinalDataList.Count, "Final data records failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeReportDepartureDateCarOnlyMethod()
        {
            GenerateBackOfficeHandoffRecords(DateType.DepartureDate, airRailCarHotelOptions: "CAR ONLY");

            //Inserts the records into the database under the Test User id.
            InsertReportHandoff();

            //run the report
            var rpt = (TravDet1)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded.
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            Assert.AreEqual(3, rpt.TravDetShared.Cars.Count, "Car data records failed.");
            Assert.AreEqual(20, rpt.TravDetShared.RawDataList.Count, "Raw data records failed.");
            Assert.AreEqual(29, rpt.TravDetShared.Segments.Count, "Segment data records failed.");
            Assert.AreEqual(31, rpt.TravDetShared.Legs.Count, "Leg data records failed.");
            Assert.AreEqual(6, rpt.TravDetShared.Hotels.Count, "Hotel data records failed.");
            Assert.AreEqual(2, rpt.FinalDataList.Count, "Final data records failed.");


            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeReportInvoiceDate()
        {
            GenerateBackOfficeHandoffRecords(DateType.InvoiceDate);

            //Inserts the records into the database under the Test User id.
            InsertReportHandoff();

            //run the report
            var rpt = (TravDet1)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded.
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            Assert.AreEqual(13, rpt.TravDetShared.Cars.Count, "Car data records failed.");
            Assert.AreEqual(64, rpt.TravDetShared.RawDataList.Count, "Raw data records failed.");
            Assert.AreEqual(78, rpt.TravDetShared.Segments.Count, "Segment data records failed.");
            Assert.AreEqual(97, rpt.TravDetShared.Legs.Count, "Leg data records failed.");
            Assert.AreEqual(35, rpt.TravDetShared.Hotels.Count, "Hotel data records failed.");
            Assert.AreEqual(204, rpt.FinalDataList.Count, "Final data records failed.");


            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeReportOnTheRoadDate()
        {
            GenerateBackOfficeHandoffRecords(DateType.OnTheRoadDatesSpecial);

            //Inserts the records into the database under the Test User id.
            InsertReportHandoff();

            //run the report
            var rpt = (TravDet1)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded.
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            Assert.AreEqual(51, rpt.TravDetShared.Cars.Count, "Car data records failed.");
            Assert.AreEqual(214, rpt.TravDetShared.RawDataList.Count, "Raw data records failed.");
            Assert.AreEqual(370, rpt.TravDetShared.Segments.Count, "Segment data records failed.");
            Assert.AreEqual(416, rpt.TravDetShared.Legs.Count, "Leg data records failed.");
            Assert.AreEqual(104, rpt.TravDetShared.Hotels.Count, "Hotel data records failed.");
            Assert.AreEqual(771, rpt.FinalDataList.Count, "Final data records failed.");



            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeReportOnTheRoadHotelOnlyDate()
        {
            GenerateBackOfficeHandoffRecords(DateType.OnTheRoadDatesSpecial, airRailCarHotelOptions: "HOTEL ONLY");

            //Inserts the records into the database under the Test User id.
            InsertReportHandoff();

            //run the report
            var rpt = (TravDet1)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded.
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            Assert.AreEqual(214, rpt.TravDetShared.RawDataList.Count, "Raw data records failed.");
            Assert.AreEqual(51, rpt.TravDetShared.Cars.Count, "Car data records failed.");
            Assert.AreEqual(416, rpt.TravDetShared.Legs.Count, "Leg data records failed.");
            Assert.AreEqual(104, rpt.TravDetShared.Hotels.Count, "Hotel data records failed.");
            Assert.AreEqual(370, rpt.TravDetShared.Segments.Count, "Segment data records failed.");
            Assert.AreEqual(52, rpt.FinalDataList.Count, "Final data records failed.");

            ClearReportHandoff();
        }

        #endregion Region - Back Office

        #region Generate

        [TestMethod]
        private void GenerateBackOfficeHandoffRecords(DateType dateRangeType, 
            string airRailCarHotelOptions = "ALL RECORDS")
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2015,7,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DDAIRRAILCARHOTELOPTIONS", ParmValue = airRailCarHotelOptions, ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2015,7,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INACCT", ParmValue = "1100,1188,1200", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PICKRECNUM", ParmValue = "13008", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = "2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "23", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibTravDet1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RBAPPLYTOLEGORSEG", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3378370", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1592", ParmInOut = "IN", LangCode = "" });

            ManipulateReportHandoffRecords(((int)dateRangeType).ToString(), "DATERANGE");
        }


        [TestMethod]
        private void GenerateReservationHandoffRecords(DateType dateRangeType, string airRailCarHotelOptions = "ALL RECORDS")
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2015,2,25", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DDAIRRAILCARHOTELOPTIONS", ParmValue = airRailCarHotelOptions, ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2015,2,26", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INACCT", ParmValue = "1200", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "EUR", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "23", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibTravDet1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RBAPPLYTOLEGORSEG", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3378596", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1592", ParmInOut = "IN", LangCode = "" });

            ManipulateReportHandoffRecords(((int)dateRangeType).ToString(), "DATERANGE");
        }
        #endregion Generate
    }
}