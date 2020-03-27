using System.Linq;
using Domain.Helper;
using iBank.IntegrationTests.TestTools;
using iBank.Server.Utilities;
using iBank.Services.Implementation.ReportPrograms.TripChanges.TripChangesAir;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.IntegrationTests.iBank.Services.Implementation.ReportPrograms
{
    [TestClass]
    public class TripChangeTests : BaseUnitTest
    {
        /*
     *  Header:
     *  Trip Changes - Air
     *  
     *  Items to Test:
     *      No Data
     *      Bad date range
     *      Too much data
     *      
     *      Date Range Type
     *          Departure Date
     *          Invoice Date
     *          Booked Date
     *      Origin
     *      Destination
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
            var rpt = (TripChange)RunReport();
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
            GenerateCustomHandoffRecords(beginDate: "DT:2015,3,1");

            InsertReportHandoff();

            //run the report
            var rpt = (TripChange)RunReport();
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
            var rpt = (TripChange)RunReport();
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
            var rpt = (TripChange)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there are right number of FinalData records. 
            Assert.AreEqual(483, rpt.FinalDataList.Count, "Final data records failed.");

            //Check for right number of records with changes
            Assert.AreEqual(423, rpt.FinalDataList.Count(s => !string.IsNullOrEmpty(s.Changedesc.Trim())), "Total records with changes failed.");

            //Check for right number of records with RecType == A
            Assert.AreEqual(60, rpt.FinalDataList.Count(s => s.Rectype == "A"), "Total records with RecType == A failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeInvoiceDateType()
        {
            GenerateCustomHandoffRecords(dateType: DateType.InvoiceDate, beginDate: "DT:2015,1,1", endDate: "DT:2015,1,5");

            InsertReportHandoff();

            //Run the report
            var rpt = (TripChange)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there are right number of FinalData records. 
            Assert.AreEqual(1050, rpt.FinalDataList.Count, "Final data records failed.");

            //Check for right number of records with changes
            Assert.AreEqual(855, rpt.FinalDataList.Count(s => !string.IsNullOrEmpty(s.Changedesc.Trim())), "Total records with changes failed.");

            //Check for right number of records with RecType == A
            Assert.AreEqual(195, rpt.FinalDataList.Count(s => s.Rectype == "A"), "Total records with RecType == A failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeBookDateType()
        {
            GenerateCustomHandoffRecords(dateType: DateType.BookedDate, beginDate: "DT:2015,1,1", endDate: "DT:2015,1,5");

            InsertReportHandoff();

            //Run the report
            var rpt = (TripChange)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there are right number of FinalData records. 
            Assert.AreEqual(1705, rpt.FinalDataList.Count, "Final data records failed.");

            //Check for right number of records with changes
            Assert.AreEqual(1422, rpt.FinalDataList.Count(s => !string.IsNullOrEmpty(s.Changedesc.Trim())), "Total records with changes failed.");

            //Check for right number of records with RecType == A
            Assert.AreEqual(283, rpt.FinalDataList.Count(s => s.Rectype == "A"), "Total records with RecType == A failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeOrigin()
        {
            GenerateCustomHandoffRecords(originCity: "NYC");

            InsertReportHandoff();

            //Run the report
            var rpt = (TripChange)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there are right number of FinalData records. 
            Assert.AreEqual(101, rpt.FinalDataList.Count, "Final data records failed.");

            //Check for right number of records with changes
            Assert.AreEqual(93, rpt.FinalDataList.Count(s => !string.IsNullOrEmpty(s.Changedesc.Trim())), "Total records with changes failed.");

            //Check for right number of records with RecType == A
            Assert.AreEqual(8, rpt.FinalDataList.Count(s => s.Rectype == "A"), "Total records with RecType == A failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeDestination()
        {
            GenerateCustomHandoffRecords(destinationCity: "ATL");

            InsertReportHandoff();

            //Run the report
            var rpt = (TripChange)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there are right number of FinalData records. 
            Assert.AreEqual(40, rpt.FinalDataList.Count, "Final data records failed.");

            //Check for right number of records with changes
            Assert.AreEqual(32, rpt.FinalDataList.Count(s => !string.IsNullOrEmpty(s.Changedesc.Trim())), "Total records with changes failed.");

            //Check for right number of records with RecType == A
            Assert.AreEqual(8, rpt.FinalDataList.Count(s => s.Rectype == "A"), "Total records with RecType == A failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeTripChanges()
        {
            GenerateCustomHandoffRecords(changeStampBeginDate: "DT:2015,1,20 T:0:0", changeStampEndDate: "DT:2015,1,21 T:0:0");

            InsertReportHandoff();

            //Run the report
            var rpt = (TripChange)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there are right number of FinalData records. 
            Assert.AreEqual(33, rpt.FinalDataList.Count, "Final data records failed.");

            //Check for right number of records with changes
            Assert.AreEqual(23, rpt.FinalDataList.Count(s => !string.IsNullOrEmpty(s.Changedesc.Trim())), "Total records with changes failed.");

            //Check for right number of records with RecType == A
            Assert.AreEqual(10, rpt.FinalDataList.Count(s => s.Rectype == "A"), "Total records with RecType == A failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeTripCancelled()
        {
            GenerateCustomHandoffRecords(tripCancelled: false);

            InsertReportHandoff();

            //Run the report
            var rpt = (TripChange)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there are right number of FinalData records. 
            Assert.AreEqual(483, rpt.FinalDataList.Count, "Final data records failed.");

            //Check for right number of records with changes
            Assert.AreEqual(423, rpt.FinalDataList.Count(s => !string.IsNullOrEmpty(s.Changedesc.Trim())), "Total records with changes failed.");

            //Check for right number of records with RecType == A
            Assert.AreEqual(60, rpt.FinalDataList.Count(s => s.Rectype == "A"), "Total records with RecType == A failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeTypeOfChange()
        {
            GenerateCustomHandoffRecords(changeCode: "220");

            InsertReportHandoff();

            //Run the report
            var rpt = (TripChange)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there are right number of FinalData records. 
            Assert.AreEqual(69, rpt.FinalDataList.Count, "Final data records failed.");

            //Check for right number of records with changes
            Assert.AreEqual(27, rpt.FinalDataList.Count(s => !string.IsNullOrEmpty(s.Changedesc.Trim())), "Total records with changes failed.");

            //Check for right number of records with RecType == A
            Assert.AreEqual(42, rpt.FinalDataList.Count(s => s.Rectype == "A"), "Total records with RecType == A failed.");

            ClearReportHandoff();
        }

        #endregion

        #region Generate Report Handoff Records Helpers

        private void GenerateCustomHandoffRecords(DateType dateType = DateType.DepartureDate, string accnt = "", string changeStampBeginDate = "", string changeStampEndDate = "", bool? tripCancelled = null, string changeCode = "", string beginDate = "", string endDate = "",
            string originCity = "", string destinationCity = "", bool consolidateChanges = false)
        {

            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2015,5,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BRANCH", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CANCELCODE", ParmValue = "X", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CARDNUM", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CANCELCODE", ParmValue = tripCancelled.HasValue ? tripCancelled.HasTrueValue() ? "Y" : "N" : "", ParmInOut = "IN", LangCode = "" });
            if (consolidateChanges)
                ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CBCONSOLIDATECHNGES", ParmValue = "ON", ParmInOut = "IN", LangCode = "" });
            if (!string.IsNullOrEmpty(changeStampBeginDate))
                ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CHANGESTAMP", ParmValue = changeStampBeginDate, ParmInOut = "IN", LangCode = "" });
            if (!string.IsNullOrEmpty(changeStampEndDate))
                ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CHANGESTAMP2", ParmValue = changeStampEndDate, ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "5", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DOMINTL", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2015,5,5", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "FIRSTDEST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
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
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "182", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibTripChange", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RECLOC", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3382560", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TICKET", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
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
                ManipulateReportHandoffRecords(originCity, "INMETROORGS");

            if (!string.IsNullOrEmpty(destinationCity))
                ManipulateReportHandoffRecords(destinationCity, "INMETRODESTS");

            if (!string.IsNullOrEmpty(changeCode))
                ManipulateReportHandoffRecords(changeCode, "INCHANGECODE");

        }


        #endregion
    }
}
