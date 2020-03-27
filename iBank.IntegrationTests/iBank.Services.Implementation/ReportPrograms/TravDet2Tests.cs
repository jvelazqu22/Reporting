using System.Linq;
using Domain.Helper;
using iBank.IntegrationTests.TestTools;
using iBank.Services.Implementation.ReportPrograms.TravelDetail;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.IntegrationTests.iBank.Services.Implementation.ReportPrograms
{
    [TestClass]
    public class TravDet2Tests : BaseUnitTest
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
            //Reservation
            GenerateReportHandoffRecordsTooMuchData(true);
            InsertReportHandoff();
            var rpt = (TravDet2)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;
            Assert.AreEqual(2, rptInfo.ReturnCode, "Return code failed.");
            var containsTooMuchDataMsg =
                rptInfo.ErrorMessage.Contains(ReportGlobals.ReportMessages.RptMsg_BigVolume);
            Assert.AreEqual(true, containsTooMuchDataMsg, "Error message failed.");
            ClearReportHandoff();

            //Reservation
            InsertReportHandoff();
            GenerateReportHandoffRecordsTooMuchData(false);
            rpt = (TravDet2)RunReport();
            rptInfo = rpt.Globals.ReportInformation;
            Assert.AreEqual(2, rptInfo.ReturnCode, "Return code failed.");
            containsTooMuchDataMsg =
               rptInfo.ErrorMessage.Contains(ReportGlobals.ReportMessages.RptMsg_BigVolume);
            Assert.AreEqual(true, containsTooMuchDataMsg, "Error message failed.");
            ClearReportHandoff();
        }

        [TestMethod]
        public void NoData()
        {
            //Reservation
            GenerateReportHandoffRecordsNoData(true);
            InsertReportHandoff();
            var rpt = (TravDet2)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;
            Assert.AreEqual(2, rptInfo.ReturnCode, "Return code failed.");
            var msg =
                rptInfo.ErrorMessage.Contains(ReportGlobals.ReportMessages.RptMsg_NoData);
            Assert.AreEqual(true, msg, "Error message failed.");
            ClearReportHandoff();

            //Backoffice
            GenerateReportHandoffRecordsNoData(false);
            InsertReportHandoff();
            rpt = (TravDet2)RunReport();
            rptInfo = rpt.Globals.ReportInformation;
            Assert.AreEqual(2, rptInfo.ReturnCode, "Return code failed.");
            msg =
                rptInfo.ErrorMessage.Contains(ReportGlobals.ReportMessages.RptMsg_NoData);
            Assert.AreEqual(true, msg, "Error message failed.");
            ClearReportHandoff();
        }

        private void GenerateReportHandoffRecordsNoData(bool reservation)
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:1800,8,16", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DDAIRRAILCARHOTELOPTIONS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:1800,8,16", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "USD", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTLANGUAGE", ParmValue = "EN", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = reservation ? "1" : "2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "24", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibTravDet1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RBAPPLYTOLEGORSEG", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RBTRAVBRKOPTION", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3382814", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1592", ParmInOut = "IN", LangCode = "" });
        }


        private void GenerateReportHandoffRecordsTooMuchData(bool reservation)
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2016,8,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DDAIRRAILCARHOTELOPTIONS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2016,8,2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "USD", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTLANGUAGE", ParmValue = "EN", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = "2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "24", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibTravDet1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RBAPPLYTOLEGORSEG", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RBTRAVBRKOPTION", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3382814", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1592", ParmInOut = "IN", LangCode = "" });
        }

        #endregion General

        #region - Reservation

        #endregion - Reservation

        #region Region - Back Office
        [TestMethod]
        public void BackOfficeHotelOnly()
        {
            GenerateBackOfficeHandoffRecordsHotelOnly(DateType.DepartureDate, "2");
            InsertReportHandoff();

            var rpt = (TravDet2)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded.
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            Assert.AreEqual(6, rpt.TravDetShared.Cars.Count, "Car data records failed.");
            Assert.AreEqual(43, rpt.TravDetShared.RawDataList.Count, "Raw data records failed.");
            Assert.AreEqual(53, rpt.TravDetShared.Segments.Count, "Segment data records failed.");
            Assert.AreEqual(69, rpt.TravDetShared.Legs.Count, "Leg data records failed.");
            Assert.AreEqual(16, rpt.TravDetShared.Hotels.Count, "Hotel data records failed.");
            Assert.AreEqual(11, rpt.FinalDataList.Count, "Final data records failed.");
            Assert.AreEqual(1400.43M, rpt.FinalDataList.Sum(s => s.HotelCost), "Total hotel cost failed.");
            ClearReportHandoff();
        }
        [TestMethod]
        public void ReservationHotelOnly()
        {
            GenerateBackOfficeHandoffRecordsHotelOnly(DateType.DepartureDate, "1");
            InsertReportHandoff();

            var rpt = (TravDet2)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded.
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            Assert.AreEqual(7, rpt.TravDetShared.Cars.Count, "Car data records failed.");
            Assert.AreEqual(35, rpt.TravDetShared.RawDataList.Count, "Raw data records failed.");
            Assert.AreEqual(40, rpt.TravDetShared.Segments.Count, "Segment data records failed.");
            Assert.AreEqual(58, rpt.TravDetShared.Legs.Count, "Leg data records failed.");
            Assert.AreEqual(18, rpt.TravDetShared.Hotels.Count, "Hotel data records failed.");
            Assert.AreEqual(13, rpt.FinalDataList.Count, "Final data records failed.");
            var sum = rpt.FinalDataList.Sum(s => s.HotelCost);
            Assert.AreEqual(1734.53M, sum , "Total hotel cost failed.");
            ClearReportHandoff();
        }

        private void GenerateBackOfficeHandoffRecordsHotelOnly(DateType dateRangeType, string prePost)
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2016,1,3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DDAIRRAILCARHOTELOPTIONS", ParmValue = "Hotel Only", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2016,5,4", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INACCT", ParmValue = "1102", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTLANGUAGE", ParmValue = "EN", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PICKRECNUM", ParmValue = "13170", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = prePost, ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "24", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibTravDet1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RBAPPLYTOLEGORSEG", ParmValue = "2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RBTRAVBRKOPTION", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3383939", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "Trip Detail Exceptions: Hotel Only", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1592", ParmInOut = "IN", LangCode = "" });
            ManipulateReportHandoffRecords(((int)dateRangeType).ToString(), "DATERANGE");
        }


        [TestMethod]
        public void BackOfficeApplyOrigDestAtLeg()
        {
            GenerateBackOfficeHandoffRecordsApplyOrigDestAtLeg(DateType.DepartureDate);
            InsertReportHandoff();

            var rpt = (TravDet2)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded.
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            Assert.AreEqual(2, rpt.TravDetShared.Cars.Count, "Car data records failed.");
            Assert.AreEqual(6, rpt.TravDetShared.RawDataList.Count, "Raw data records failed.");
            Assert.AreEqual(0, rpt.TravDetShared.Segments.Count, "Segment data records failed.");
            Assert.AreEqual(8, rpt.TravDetShared.Legs.Count, "Leg data records failed.");
            Assert.AreEqual(3, rpt.TravDetShared.Hotels.Count, "Hotel data records failed.");
            Assert.AreEqual(8, rpt.FinalDataList.Count, "Final data records failed.");

            ClearReportHandoff();
        }


        [TestMethod]
        public void BackOfficeApplyOrigDestAtSeg()
        {
            GenerateBackOfficeHandoffRecordsApplyOrigDestAtSeg(DateType.DepartureDate);
            InsertReportHandoff();

            var rpt = (TravDet2)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded.
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            Assert.AreEqual(2, rpt.TravDetShared.Cars.Count, "Car data records failed.");
            Assert.AreEqual(6, rpt.TravDetShared.RawDataList.Count, "Raw data records failed.");
            Assert.AreEqual(2, rpt.TravDetShared.Segments.Count, "Segment data records failed.");
            Assert.AreEqual(8, rpt.TravDetShared.Legs.Count, "Leg data records failed.");
            Assert.AreEqual(3, rpt.TravDetShared.Hotels.Count, "Hotel data records failed.");
            Assert.AreEqual(4, rpt.FinalDataList.Count, "Final data records failed.");

            ClearReportHandoff();
        }

        private void GenerateBackOfficeHandoffRecordsApplyOrigDestAtSeg(DateType dateRangeType)
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2016,1,3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DDAIRRAILCARHOTELOPTIONS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2016,5,4", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INACCT", ParmValue = "1102", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK1", ParmValue = "HORSECARE", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INDESTS", ParmValue = "ORD", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "USD", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTLANGUAGE", ParmValue = "EN", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PICKRECNUM", ParmValue = "13170", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = "2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "24", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibTravDet1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RBAPPLYTOLEGORSEG", ParmValue = "2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RBTRAVBRKOPTION", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3383934", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "Apply orig/dest @ seg Horsecare", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1592", ParmInOut = "IN", LangCode = "" });
            ManipulateReportHandoffRecords(((int)dateRangeType).ToString(), "DATERANGE");
        }

        private void GenerateBackOfficeHandoffRecordsApplyOrigDestAtLeg(DateType dateRangeType)
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2016,1,3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DDAIRRAILCARHOTELOPTIONS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2016,5,4", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INACCT", ParmValue = "1102", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK1", ParmValue = "HORSECARE", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INDESTS", ParmValue = "ORD", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "USD", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTLANGUAGE", ParmValue = "EN", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PICKRECNUM", ParmValue = "13335", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = "2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "24", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibTravDet1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RBAPPLYTOLEGORSEG", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RBTRAVBRKOPTION", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3383933", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "Apply dest/orig @ leg Horsecare", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1592", ParmInOut = "IN", LangCode = "" });
            ManipulateReportHandoffRecords(((int)dateRangeType).ToString(), "DATERANGE");
        }

        #endregion Region - Back Office

    }
}