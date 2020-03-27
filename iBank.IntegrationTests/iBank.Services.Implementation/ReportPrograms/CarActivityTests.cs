using System.Linq;
using Domain.Helper;
using iBank.IntegrationTests.TestTools;
using iBank.Services.Implementation.ReportPrograms.CarActivity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.IntegrationTests.iBank.Services.Implementation.ReportPrograms
{
    /// <summary>
    /// Note: This set of tests covers four reports. 
    /// Car Activity
    /// Analysis by City - Car
    /// Analysis by Vendor - Car
    /// Advanced Bookings - Car
    /// </summary>
    [TestClass]
    public class CarActivityTests : BaseUnitTest
    {
        [TestMethod]
        public void GenerateReportTooMuchData()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("DT:2015,1,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,10,30", "ENDDATE");
            ManipulateReportHandoffRecords("1", "PREPOST");
            InsertReportHandoff();

            //run the report
            var rpt = (CarActivity)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(2, rptInfo.ReturnCode, "Return code failed.");
            var containsNoDataMsg = rptInfo.ErrorMessage.Contains(ReportGlobals.ReportMessages.RptMsg_BigVolume);
            Assert.AreEqual(true, containsNoDataMsg, "Error message failed.");

            ClearReportHandoff();
        }

        #region Reservation
        [TestMethod]
        public void GenerateReportReservationDepartureDate()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("DT:2015,8,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,10,30", "ENDDATE");
            ManipulateReportHandoffRecords("1","PREPOST");
            InsertReportHandoff();

            //run the report
            var rpt = (CarActivity)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            var totalDays = rpt.FinalDataList.Sum(s => s.Days);
            var totalCost = rpt.FinalDataList.Sum(s => s.Abookrat * s.Days);

            Assert.AreEqual(43, totalDays);
            Assert.AreEqual(1684.84m, totalCost);

            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportReservationInvoiceDate()
        {
            GenerateReportHandoff(DateType.InvoiceDate);
            ManipulateReportHandoffRecords("DT:2015,6,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,10,30", "ENDDATE");//TODO
            ManipulateReportHandoffRecords("1", "PREPOST");
            InsertReportHandoff();

            //run the report
            var rpt = (CarActivity)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            var totalDays = rpt.FinalDataList.Sum(s => s.Days);
            var totalCost = rpt.FinalDataList.Sum(s => s.Abookrat * s.Days);

            Assert.AreEqual(2, totalDays);
            Assert.AreEqual(96.00m, totalCost);

            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportReservationOnTheRoadDate()
        {
            GenerateReportHandoff(DateType.OnTheRoadDatesCarRental);
            ManipulateReportHandoffRecords("DT:2015,6,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,10,30", "ENDDATE");
            ManipulateReportHandoffRecords("1", "PREPOST");
            InsertReportHandoff();

            //run the report
            var rpt = (CarActivity)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            var totalDays = rpt.FinalDataList.Sum(s => s.Days);
            var totalCost = rpt.FinalDataList.Sum(s => s.Abookrat * s.Days);

            Assert.AreEqual(95, totalDays);
            Assert.AreEqual(4025.35m, totalCost);

            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportReservationRentalDate()
        {
            GenerateReportHandoff(DateType.CarRentalDate);
            ManipulateReportHandoffRecords("1", "PREPOST");
            ManipulateReportHandoffRecords("DT:2015,8,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,10,30", "ENDDATE");
            InsertReportHandoff();

            //run the report
            var rpt = (CarActivity)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            var totalDays = rpt.FinalDataList.Sum(s => s.Days);
            var totalCost = rpt.FinalDataList.Sum(s => s.Abookrat * s.Days);

            Assert.AreEqual(43, totalDays);
            Assert.AreEqual(1684.84m, totalCost);

            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportReservationBookedDate()
        {
            GenerateReportHandoff(DateType.BookedDate);
            ManipulateReportHandoffRecords("DT:2015,1,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,1,1", "ENDDATE");
            ManipulateReportHandoffRecords("1", "PREPOST");
            InsertReportHandoff();

            //run the report
            var rpt = (CarActivity)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            var totalDays = rpt.FinalDataList.Sum(s => s.Days);
            var totalCost = rpt.FinalDataList.Sum(s => s.Abookrat * s.Days);

            Assert.AreEqual(86, totalDays);
            Assert.AreEqual(4068.03m, totalCost);

            ClearReportHandoff();
        }
        #endregion  

        #region Backoffice
        [TestMethod]
        public void GenerateReportBackOfficeDepartureDate()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            InsertReportHandoff();

            //run the report
            var rpt = (CarActivity)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            var totalDays = rpt.FinalDataList.Sum(s => s.Days);
            var totalCost = rpt.FinalDataList.Sum(s => s.Abookrat*s.Days);

            Assert.AreEqual(449, totalDays);
            Assert.AreEqual(12777.20m, totalCost);

            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportBackOfficeInvoiceDate()
        {
            GenerateReportHandoff(DateType.InvoiceDate);
            InsertReportHandoff();

            //run the report
            var rpt = (CarActivity)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            var totalDays = rpt.FinalDataList.Sum(s => s.Days);
            var totalCost = rpt.FinalDataList.Sum(s => s.Abookrat * s.Days);

            Assert.AreEqual(324, totalDays);
            Assert.AreEqual(9358.06m, totalCost);

            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportBackOfficeOnTheRoadDate()
        {
            GenerateReportHandoff(DateType.OnTheRoadDatesCarRental);
            InsertReportHandoff();

            //run the report
            var rpt = (CarActivity)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            var totalDays = rpt.FinalDataList.Sum(s => s.Days);
            var totalCost = rpt.FinalDataList.Sum(s => s.Abookrat * s.Days);

            Assert.AreEqual(449, totalDays);
            Assert.AreEqual(12777.20m, totalCost);

            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportBackOfficeRentalDate()
        {
            GenerateReportHandoff(DateType.CarRentalDate);
            InsertReportHandoff();

            //run the report
            var rpt = (CarActivity)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            var totalDays = rpt.FinalDataList.Sum(s => s.Days);
            var totalCost = rpt.FinalDataList.Sum(s => s.Abookrat * s.Days);

            Assert.AreEqual(449, totalDays);
            Assert.AreEqual(12777.20m, totalCost);

            ClearReportHandoff();
        }
        #endregion  
        public void GenerateReportHandoff(DateType dateType)
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2016,1,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2016,10,30", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "USD", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = "2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "76", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibCarActivity", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3383748", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1591", ParmInOut = "IN", LangCode = "" });

            ManipulateReportHandoffRecords(((int)dateType).ToString(), "DATERANGE");

        }
    }
}
