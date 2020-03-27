using System;
using System.Linq;

using Domain.Helper;

using iBank.IntegrationTests.TestTools;
using iBank.Services.Implementation;
using iBank.Services.Implementation.ReportPrograms;
using iBank.Services.Implementation.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using iBank.Server.Utilities;
using iBank.Services.Implementation.ReportPrograms.TripChanges.TripChangesMeetAndGreet;

namespace iBank.IntegrationTests.iBank.Services.Implementation.ReportPrograms
{
    [TestClass]
    public class MeetGreetTests : BaseUnitTest
    {
        /*
     *  Header:
     *  Trip Changes - Meet and Greet Report
     *  
     *  Items to Test:
     *      No Data
     *      Bad date range
     *      Too much data
     *      
     *      Date Range Type
     *          Arrival
     *          Booked Date
     *      Suppress Detail Change Info
     *      Use Airpot Codes, Not City Names
     *      Use Connecting Legs
     *      UDID search
     *      Trip Changes
     *      Trip Cancelled
     *      Type of Change
     *          
     *          
     * */

        #region General Test Methods

        [TestMethod]
        public void NoData()
        {
            GenerateCustomHandoffRecords(accnt: "1200", endDate: "DT:2015,5,2");

            InsertReportHandoff();

            //run the report
            var rpt = (MeetGreet)RunReport();
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
            GenerateCustomHandoffRecords(beginDate: "DT:2015,1,1", endDate: "DT:2015,6,30");

            InsertReportHandoff();

            //run the report
            var rpt = (MeetGreet)RunReport();
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
            var rpt = (MeetGreet)RunReport();
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
        public void BackOfficeArrivalDateType()
        {
            GenerateCustomHandoffRecords();

            InsertReportHandoff();

            //Run the report
            var rpt = (MeetGreet)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there are right number of FinalData records. 
            Assert.AreEqual(311, rpt.FinalDataList.Count, "Final data records failed.");

            //Check for right number of records for a particular destination
            var totalDest = rpt.FinalDataList.Count(s => s.Destinat.Trim().ToUpper() == "AUA");
            Assert.AreEqual(10, totalDest, "Total records for particular destination failed");


            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeBookDateType()
        {
            GenerateCustomHandoffRecords(dateType: DateType.BookedDate, beginDate: "DT:2015,1,1", accnt: "1200");

            InsertReportHandoff();

            //Run the report
            var rpt = (MeetGreet)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there are right number of FinalData records. 
            Assert.AreEqual(894, rpt.FinalDataList.Count, "Final data records failed.");

            //Check for right number of records for a particular destination
            var totalDest = rpt.FinalDataList.Count(s => s.Destinat.Trim().ToUpper() == "ORD");
            Assert.AreEqual(121, totalDest, "Total records for particular destination failed");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeSuppressDetailChangeInfo()
        {
            GenerateCustomHandoffRecords(beginDate: "DT:2015,3,1", endDate: "DT:2015,3,13", accnt: "1188", suppressChangeDetails: true);

            InsertReportHandoff();

            //Run the report
            var rpt = (MeetGreet)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there are right number of FinalData records. 
            Assert.AreEqual(91, rpt.FinalDataList.Count, "Final data records failed.");

            //Check for right number of records for a particular destination
            var totalDest = rpt.FinalDataList.Count(s => s.Destinat.Trim().ToUpper() == "EWR");
            Assert.AreEqual(19, totalDest, "Total records for particular destination failed");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeAirportCodes()
        {
            GenerateCustomHandoffRecords(useAirportCodes: true);

            InsertReportHandoff();

            //Run the report
            var rpt = (MeetGreet)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there are right number of FinalData records. 
            Assert.AreEqual(311, rpt.FinalDataList.Count, "Final data records failed.");

            //Check if destination description is an airport code
            Assert.AreEqual("ALB", rpt.FinalDataList.First().Destdesc.Trim(), "Destination description failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeUseConnectingLegs()
        {
            GenerateCustomHandoffRecords(originCity: "ATL", useConnectinLegs: true, useAirportCodes: true, beginDate: "DT:2015,4,1", endDate: "DT:2015,5,1");

            InsertReportHandoff();

            //Run the report
            var rpt = (MeetGreet)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there are right number of FinalData records. 
            Assert.AreEqual(77, rpt.FinalDataList.Count, "Final data records failed.");

            //Check that all the records have ATL as their original origin 
            Assert.AreEqual(true, rpt.FinalDataList.All(s => s.Origdesc.Trim() == "ATL"), "Final data records failed.");

            ClearReportHandoff();
        }


        [TestMethod]
        public void BackOfficeUDID1Search()
        {
            GenerateCustomHandoffRecords(udid1: 1);

            InsertReportHandoff();

            //Run the report
            var rpt = (MeetGreet)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there are right number of FinalData records. 
            Assert.AreEqual(311, rpt.FinalDataList.Count, "Final data records failed.");

            //Check total number of records that have a udid text. 
            Assert.AreEqual(216, rpt.FinalDataList.Count(s => !string.IsNullOrEmpty(s.Udidtext1.Trim())), "Non-empty UDID1 Text failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeUDID2Search()
        {
            GenerateCustomHandoffRecords(udid1: 1, udid2: 5);

            InsertReportHandoff();

            //Run the report
            var rpt = (MeetGreet)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there are right number of FinalData records. 
            Assert.AreEqual(311, rpt.FinalDataList.Count, "Final data records failed.");

            //Check total number of records that have a udid text. 
            Assert.AreEqual(292, rpt.FinalDataList.Count(s => !string.IsNullOrEmpty(s.Udidtext2.Trim())), "Non-empty UDID2 Text failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeTripChanges()
        {
            GenerateCustomHandoffRecords(changeStampBeginDate: "DT:2015,1,20 T:0:0", changeStampEndDate: "DT:2015,1,21 T:0:0");

            InsertReportHandoff();

            //Run the report
            var rpt = (MeetGreet)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there are right number of FinalData records. 
            Assert.AreEqual(163, rpt.FinalDataList.Count, "Final data records failed.");

            //Check total number of records that have changes. 
            Assert.AreEqual(2, rpt.FinalDataList.Count(s => s.Changstamp != DateTime.MinValue), "Trip changes failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeTripCancelled()
        {
            GenerateCustomHandoffRecords(tripCancelled: false);

            InsertReportHandoff();

            //Run the report
            var rpt = (MeetGreet)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there are right number of FinalData records. 
            Assert.AreEqual(311, rpt.FinalDataList.Count, "Final data records failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeTypeOfChange()
        {
            GenerateCustomHandoffRecords(changeCode: "220");

            InsertReportHandoff();

            //Run the report
            var rpt = (MeetGreet)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there are right number of FinalData records. 
            Assert.AreEqual(165, rpt.FinalDataList.Count, "Final data records failed.");

            //Check that there are right number of records with change description
            Assert.AreEqual(8, rpt.FinalDataList.Count(s => !string.IsNullOrEmpty(s.Changedesc.Trim())), "Non-empty change description records failed.");

            ClearReportHandoff();
        }

        #endregion

        #region Generate Report Handoff Records Helpers

        private void GenerateCustomHandoffRecords(DateType dateType = DateType.RoutingArrivalDate, string accnt = "", bool suppressChangeDetails = false, bool useAirportCodes = false, bool useConnectinLegs = false,
            int? udid1 = null, int? udid2 = null, string changeStampBeginDate = "", string changeStampEndDate = "", bool? tripCancelled = null, string changeCode = "", string beginDate = "", string endDate = "", 
            string originCity = "", string destinationCity = "", string sortyBy = "1")
        {

            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2015,5,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BRANCH", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CANCELCODE", ParmValue = "X", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CARDNUM", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CANCELCODE",  ParmValue = tripCancelled.HasValue ?  tripCancelled.HasTrueValue() ? "Y" : "N" : "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CBINCLSUBTOTSBYFLT", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CBUSECONNECTLEGS", ParmValue = useConnectinLegs ? "ON" : "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CBSUPPDETCHANGE", ParmValue = suppressChangeDetails ? "ON" : "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CBUSEAIRPORTCODES", ParmValue = useAirportCodes ? "ON" : "", ParmInOut = "IN", LangCode = "" });
            if (!string.IsNullOrEmpty(changeStampBeginDate))
                ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CHANGESTAMP", ParmValue = changeStampBeginDate, ParmInOut = "IN", LangCode = "" });
            if (!string.IsNullOrEmpty(changeStampEndDate))
                ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CHANGESTAMP2", ParmValue = changeStampEndDate, ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "5", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DOMINTL", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2015,5,3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "FIRSTORIGIN", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INACCT", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INAIRLINE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK1", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK3", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INCHANGECODE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INDESTS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INMETROORGS", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INORIGCOUNTRY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INORIGREGION", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVALCARR", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVOICE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MODE", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "USD", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSFIRST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSLAST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "180", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibMeetGreet", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RECLOC", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3382140", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "SORTBY", ParmValue = sortyBy, ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TICKET", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });

            if (udid1.HasValue)
            {
                ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UdidOnRpt1", ParmValue = udid1.ToString(), ParmInOut = "IN", LangCode = "" });
            }
            if (udid2.HasValue)
            {
                ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UdidOnRpt2", ParmValue = udid2.ToString(), ParmInOut = "IN", LangCode = "" });
            }

            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1593", ParmInOut = "IN", LangCode = "" });


            ManipulateReportHandoffRecords(((int)dateType).ToString(), "DATERANGE");

            if (!string.IsNullOrEmpty(accnt))
                ManipulateReportHandoffRecords(accnt, "INACCT");

            if (!string.IsNullOrEmpty(beginDate))
                ManipulateReportHandoffRecords(beginDate, "BEGDATE");

            if (!string.IsNullOrEmpty(endDate))
                ManipulateReportHandoffRecords(endDate, "ENDDATE");

            if (!string.IsNullOrEmpty(originCity))
                ManipulateReportHandoffRecords(originCity, "INORGS");

            if (!string.IsNullOrEmpty(destinationCity))
                ManipulateReportHandoffRecords(destinationCity, "INMETRODESTS");

            if (!string.IsNullOrEmpty(changeCode))
                ManipulateReportHandoffRecords(changeCode, "INCHANGECODE");

        }


        #endregion
    }
}
