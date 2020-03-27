using System.Linq;
using Domain.Helper;
using iBank.IntegrationTests.TestTools;
using iBank.Services.Implementation.ReportPrograms.RailTopBottomCityPair;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.IntegrationTests.iBank.Services.Implementation.ReportPrograms
{
    [TestClass]
    public class TopCityPairRailTests : BaseUnitTest
    {
        [TestMethod]
        public void GenerateReportNoData()
        {
            GenerateReportHandoff(DateType.BookedDate);
            ManipulateReportHandoffRecords("1", "PREPOST");
            InsertReportHandoff();

            //run the report
            var rpt = (TopCityPairRail)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(2, rptInfo.ReturnCode, "Return code failed.");
            var containsNoDataMsg = rptInfo.ErrorMessage.Contains(ReportGlobals.ReportMessages.RptMsg_NoData);
            Assert.AreEqual(true, containsNoDataMsg, "Error message failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportBackofficeInvoiceDate()
        {
            GenerateReportHandoff(DateType.InvoiceDate);
 
            InsertReportHandoff();

            //run the report
            var rpt = (TopCityPairRail)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(8, rpt.FinalDataList.Count, "Wrong number of records.");
            Assert.AreEqual(15, rpt.FinalDataList.Sum(s => s.Segments), "Wrong number of segments.");
            Assert.AreEqual(1682.96m, rpt.FinalDataList.Sum(s => s.Cost), "Volume Booked incorrect.");
            Assert.AreEqual(1616, rpt.FinalDataList.Sum(s => s.Miles), "Miles incorrect.");

            ClearReportHandoff();
        }

       

        [TestMethod]
        public void GenerateReportBackofficeDepartureDate()
        {
            GenerateReportHandoff(DateType.DepartureDate);


            InsertReportHandoff();

            //run the report
            var rpt = (TopCityPairRail)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(7, rpt.FinalDataList.Count, "Wrong number of records.");
            Assert.AreEqual(13, rpt.FinalDataList.Sum(s => s.Segments), "Wrong number of segments.");
            Assert.AreEqual(1611.64m, rpt.FinalDataList.Sum(s => s.Cost), "Volume Booked incorrect.");
            Assert.AreEqual(1338, rpt.FinalDataList.Sum(s => s.Miles), "Miles incorrect.");


            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportBackofficeDepartureDateBothWays()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("1", "RBFLTMKTONEWAYBOTHWAYS");

            InsertReportHandoff();

            //run the report
            var rpt = (TopCityPairRail)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(5, rpt.FinalDataList.Count, "Wrong number of records.");
            Assert.AreEqual(13, rpt.FinalDataList.Sum(s => s.Segments), "Wrong number of segments.");
            Assert.AreEqual(1611.64m, rpt.FinalDataList.Sum(s => s.Cost), "Volume Booked incorrect.");
            Assert.AreEqual(1338, rpt.FinalDataList.Sum(s => s.Miles), "Miles incorrect.");


            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportBackofficeDepartureDateLimit5()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("5", "HOWMANY");

            InsertReportHandoff();

            //run the report
            var rpt = (TopCityPairRail)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(5, rpt.FinalDataList.Count, "Wrong number of records.");
            Assert.AreEqual(11, rpt.FinalDataList.Sum(s => s.Segments), "Wrong number of segments.");
            Assert.AreEqual(1396.30m, rpt.FinalDataList.Sum(s => s.Cost), "Volume Booked incorrect.");
            Assert.AreEqual(860, rpt.FinalDataList.Sum(s => s.Miles), "Miles incorrect.");


            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportReservationInvoiceDate()
        {
            GenerateReportHandoff(DateType.InvoiceDate);
            ManipulateReportHandoffRecords("1", "PREPOST");
            ManipulateReportHandoffRecords("DT:2015,1,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,1,31", "ENDDATE");
            InsertReportHandoff();

            //run the report
            var rpt = (TopCityPairRail)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(10, rpt.FinalDataList.Count, "Wrong number of records.");
            Assert.AreEqual(196, rpt.FinalDataList.Sum(s => s.Segments), "Wrong number of segments.");
            Assert.AreEqual(17874.84m, rpt.FinalDataList.Sum(s => s.Cost), "Volume Booked incorrect.");
            Assert.AreEqual(0, rpt.FinalDataList.Sum(s => s.Miles), "Miles incorrect.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportReservationDepartureDate()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("1", "PREPOST");

            InsertReportHandoff();

            //run the report
            var rpt = (TopCityPairRail)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(10, rpt.FinalDataList.Count, "Wrong number of records.");
            Assert.AreEqual(14, rpt.FinalDataList.Sum(s => s.Segments), "Wrong number of segments.");
            Assert.AreEqual(1824.52m, rpt.FinalDataList.Sum(s => s.Cost), "Volume Booked incorrect.");
            Assert.AreEqual(0, rpt.FinalDataList.Sum(s => s.Miles), "Miles incorrect.");


            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportReservationBookedDate()
        {
            GenerateReportHandoff(DateType.BookedDate);
            ManipulateReportHandoffRecords("1", "PREPOST");
            ManipulateReportHandoffRecords("DT:2015,1,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,1,31", "ENDDATE");
            InsertReportHandoff();

            //run the report
            var rpt = (TopCityPairRail)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(10, rpt.FinalDataList.Count, "Wrong number of records.");
            Assert.AreEqual(185, rpt.FinalDataList.Sum(s => s.Segments), "Wrong number of segments.");
            Assert.AreEqual(21907.15m, rpt.FinalDataList.Sum(s => s.Cost), "Volume Booked incorrect.");
            Assert.AreEqual(0, rpt.FinalDataList.Sum(s => s.Miles), "Miles incorrect.");


            ClearReportHandoff();
        }

        public void GenerateReportHandoff(DateType dateType)
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2015,3,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2015,9,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "HOWMANY", ParmValue = "10", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INACCT", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "USD", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = "2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "66", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibTopCityPairRail", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RBFLTMKTONEWAYBOTHWAYS", ParmValue = "2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RBSORTDESCASC", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3392454", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "SORTBY", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1591", ParmInOut = "IN", LangCode = "" });

            ManipulateReportHandoffRecords(((int)dateType).ToString(), "DATERANGE");

        }
    }
}
