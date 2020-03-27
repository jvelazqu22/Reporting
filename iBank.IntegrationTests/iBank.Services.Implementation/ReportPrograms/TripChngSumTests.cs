using System.Linq;

using Domain.Helper;

using iBank.IntegrationTests.TestTools;
using iBank.Server.Utilities;
using iBank.Services.Implementation.ReportPrograms.TripChanges.TripChangesTypeSummary;
using iBank.Services.Implementation.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.IntegrationTests.iBank.Services.Implementation.ReportPrograms
{
    [TestClass]
    public class TripChngSumTests : BaseUnitTest
    {
        /*
        *  Header:
        *  Trip Changes - Type Summary Report
        *  
        *  Items to Test:
        *      No Data
        *      Bad date range
        *      
        *      Date Range Type
        *          Departure Date
        *          Invoice Date
        *          Booked Date
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
            var rpt = (TripChngSum)RunReport();
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
            GenerateCustomHandoffRecords(beginDate: "DT:2016,2,1", endDate: "DT:2016,1,1");

            InsertReportHandoff();

            //run the report
            var rpt = (TripChngSum)RunReport();
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
            var rpt = (TripChngSum)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there are right number of total trips. 
            Assert.AreEqual(165, rpt.TotalCount, "Total trips failed.");

            //Check that there are right number of trips with changes. 
            Assert.AreEqual(32, rpt.TotalCount3, "Total trips with changes failed.");

            //Check for right number of changes
            var totalChanges = rpt.FinalDataList.Sum(s => s.Numchngs);
            Assert.AreEqual(625, totalChanges, "Total changes failed");

            //Check for right number of trip/traveler info changes
            var tripChanges = rpt.SubReportData.FirstOrDefault(s => s.GrpDesc.StartsWith("Trip"));
            Assert.AreEqual(455, tripChanges.Numchngs, "Total trip changes failed");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeInvoiceDateType()
        {
            GenerateCustomHandoffRecords(dateType: DateType.InvoiceDate, beginDate: "DT:2015,1,1", endDate: "DT:2015,1,5");

            InsertReportHandoff();

            //Run the report
            var rpt = (TripChngSum)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there are right number of total trips. 
            Assert.AreEqual(1609, rpt.TotalCount, "Total trips failed.");

            //Check that there are right number of trips with changes. 
            Assert.AreEqual(90, rpt.TotalCount3, "Total trips with changes failed.");

            //Check for right number of changes
            var totalChanges = rpt.FinalDataList.Sum(s => s.Numchngs);
            Assert.AreEqual(1079, totalChanges, "Total changes failed");

            //Check for right number of trip/traveler info changes
            var tripChanges = rpt.SubReportData.FirstOrDefault(s => s.GrpDesc.StartsWith("Trip"));
            Assert.AreEqual(822, tripChanges.Numchngs, "Total trip changes failed");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeBookDateType()
        {
            GenerateCustomHandoffRecords(dateType: DateType.BookedDate, beginDate: "DT:2015,1,1", endDate: "DT:2015,1,5");

            InsertReportHandoff();

            //Run the report
            var rpt = (TripChngSum)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there are right number of total trips. 
            Assert.AreEqual(2081, rpt.TotalCount, "Total trips failed.");

            //Check that there are right number of trips with changes. 
            Assert.AreEqual(124, rpt.TotalCount3, "Total trips with changes failed.");

            //Check for right number of changes
            var totalChanges = rpt.FinalDataList.Sum(s => s.Numchngs);
            Assert.AreEqual(1819, totalChanges, "Total changes failed");

            //Check for right number of trip/traveler info changes
            var tripChanges = rpt.SubReportData.FirstOrDefault(s => s.GrpDesc.StartsWith("Trip"));
            Assert.AreEqual(1273, tripChanges.Numchngs, "Total trip changes failed");

            ClearReportHandoff();
        }

       
        [TestMethod]
        public void BackOfficeTripChanges()
        {
            GenerateCustomHandoffRecords(changeStampBeginDate: "DT:2015,4,1 T:0:0", changeStampEndDate: "DT:2015,4,30 T:0:0");

            InsertReportHandoff();

            //Run the report
            var rpt = (TripChngSum)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there are right number of total trips. 
            Assert.AreEqual(165, rpt.TotalCount, "Total trips failed.");

            //Check that there are right number of trips with changes. 
            Assert.AreEqual(6, rpt.TotalCount3, "Total trips with changes failed.");

            //Check for right number of changes
            var totalChanges = rpt.FinalDataList.Sum(s => s.Numchngs);
            Assert.AreEqual(16, totalChanges, "Total changes failed");

            //Check for right number of trip/traveler info changes
            var tripChanges = rpt.SubReportData.FirstOrDefault(s => s.GrpDesc.StartsWith("Trip"));
            Assert.AreEqual(14, tripChanges.Numchngs, "Total trip changes failed");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeTripCancelled()
        {
            GenerateCustomHandoffRecords(tripCancelled: false);

            InsertReportHandoff();

            //Run the report
            var rpt = (TripChngSum)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there are right number of total trips. 
            Assert.AreEqual(165, rpt.TotalCount, "Total trips failed.");

            //Check that there are right number of trips with changes. 
            Assert.AreEqual(32, rpt.TotalCount3, "Total trips with changes failed.");

            //Check for right number of changes
            var totalChanges = rpt.FinalDataList.Sum(s => s.Numchngs);
            Assert.AreEqual(625, totalChanges, "Total changes failed");

            //Check for right number of trip/traveler info changes
            var tripChanges = rpt.SubReportData.FirstOrDefault(s => s.GrpDesc.StartsWith("Trip"));
            Assert.AreEqual(455, tripChanges.Numchngs, "Total trip changes failed");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeTypeOfChange()
        {
            GenerateCustomHandoffRecords(changeCode: "112");

            InsertReportHandoff();

            //Run the report
            var rpt = (TripChngSum)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //If the test is for a valid report, check to see that the report succeeded. 
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual("", rptInfo.ErrorMessage, "Error message failed.");

            //Check that there are right number of total trips. 
            Assert.AreEqual(165, rpt.TotalCount, "Total trips failed.");

            //Check that there are right number of trips with changes. 
            Assert.AreEqual(32, rpt.TotalCount3, "Total trips with changes failed.");

            //Check for right number of changes
            var totalChanges = rpt.FinalDataList.Sum(s => s.Numchngs);
            Assert.AreEqual(202, totalChanges, "Total changes failed");

            //Check for right number of trip/traveler info changes
            var tripChanges = rpt.SubReportData.FirstOrDefault(s => s.GrpDesc.StartsWith("Trip"));
            Assert.AreEqual(202, tripChanges.Numchngs, "Total trip changes failed");
        }

        #endregion

        #region Generate Report Handoff Records Helpers

        private void GenerateCustomHandoffRecords(DateType dateType = DateType.DepartureDate, string accnt = "", string changeStampBeginDate = "", string changeStampEndDate = "", bool? tripCancelled = null, string changeCode = "", string beginDate = "", string endDate = "")
        {

            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2015,5,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BRANCH", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CANCELCODE", ParmValue = tripCancelled.HasValue ? tripCancelled.HasTrueValue() ? "Y" : "N" : "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CARDNUM", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DOMINTL", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2015,5,30", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INACCT", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK1", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INBREAK3", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INCHANGECODE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVALCARR", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INVOICE", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "USD", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSFIRST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PASSLAST", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "184", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibTripChngSum", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PSEUDOCITY", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RECLOC", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3383006", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TICKET", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1593", ParmInOut = "IN", LangCode = "" });

            if (!string.IsNullOrEmpty(changeStampBeginDate))
                ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CHANGESTAMP", ParmValue = changeStampBeginDate, ParmInOut = "IN", LangCode = "" });
            if (!string.IsNullOrEmpty(changeStampEndDate))
                ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CHANGESTAMP2", ParmValue = changeStampEndDate, ParmInOut = "IN", LangCode = "" });

            ManipulateReportHandoffRecords(((int)dateType).ToString(), "DATERANGE");

            if (!string.IsNullOrEmpty(accnt))
                ManipulateReportHandoffRecords(accnt, "INACCT");

            if (!string.IsNullOrEmpty(beginDate))
                ManipulateReportHandoffRecords(beginDate, "BEGDATE");

            if (!string.IsNullOrEmpty(endDate))
                ManipulateReportHandoffRecords(endDate, "ENDDATE");

            if (!string.IsNullOrEmpty(changeCode))
                ManipulateReportHandoffRecords(changeCode, "INCHANGECODE");

        }

        #endregion
    }
}
