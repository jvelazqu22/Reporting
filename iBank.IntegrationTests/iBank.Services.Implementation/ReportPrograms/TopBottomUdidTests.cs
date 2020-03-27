using System.Linq;
using Domain.Helper;
using iBank.IntegrationTests.TestTools;
using iBank.Services.Implementation.ReportPrograms.TopBottomUdids;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.IntegrationTests.iBank.Services.Implementation.ReportPrograms
{
    [TestClass]
    public class TopBottomUdidTests : BaseUnitTest
    {

        [TestMethod]
        public void BackOfficeDepartureDate()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            InsertReportHandoff();

            var rpt = (TopBottomUdids)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            var firstRec = rpt.FinalDataList.FirstOrDefault();
            if (firstRec == null)
            {
                Assert.Fail("No records in final data list");
            }
            Assert.AreEqual(9, rpt.FinalDataList.Count, "Count is incorrect");

            Assert.AreEqual(79, firstRec.UdidCount, "Udid Count is incorrect");
            Assert.AreEqual("CHOC-NA", firstRec.UdidText, "Udid Text is incorrect");

            Assert.AreEqual(231, rpt.FinalDataList.Sum(s => s.UdidCount), "Total Listed Udid Count");

            Assert.AreEqual(231, rpt.RawDataList.Sum(s => s.UdidCount), "Report Total Udid Count");


           
            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeDepartureDateWithUdidTexts()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("CHOC-NA|PET-NA", "UDIDTEXT");
            InsertReportHandoff();

            var rpt = (TopBottomUdids)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            var firstRec = rpt.FinalDataList.FirstOrDefault();
            if (firstRec == null)
            {
                Assert.Fail("No records in final data list");
            }
            Assert.AreEqual(2, rpt.FinalDataList.Count, "Count is incorrect");

            Assert.AreEqual(79, firstRec.UdidCount, "Udid Count is incorrect");
            Assert.AreEqual("CHOC-NA", firstRec.UdidText, "Udid Text is incorrect");

            Assert.AreEqual(134, rpt.FinalDataList.Sum(s => s.UdidCount), "Total Listed Udid Count");

            Assert.AreEqual(134, rpt.RawDataList.Sum(s => s.UdidCount), "Report Total Udid Count");



            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeInvoiceDate()
        {
            GenerateReportHandoff(DateType.InvoiceDate);
            InsertReportHandoff();

            var rpt = (TopBottomUdids)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            var firstRec = rpt.FinalDataList.FirstOrDefault();
            if (firstRec == null)
            {
                Assert.Fail("No records in final data list");
            }
            Assert.AreEqual(9, rpt.FinalDataList.Count, "Count is incorrect");

            Assert.AreEqual(68, firstRec.UdidCount, "Udid Count is incorrect");
            Assert.AreEqual("PET-NA", firstRec.UdidText, "Udid Text is incorrect");

            Assert.AreEqual(228, rpt.FinalDataList.Sum(s => s.UdidCount), "Total Listed Udid Count");

            Assert.AreEqual(228, rpt.RawDataList.Sum(s => s.UdidCount), "Report Total Udid Count");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationDepartureDate()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("1","PREPOST");
            ManipulateReportHandoffRecords("DT:2015,1,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,1,31", "ENDDATE");
            InsertReportHandoff();

            var rpt = (TopBottomUdids)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            var firstRec = rpt.FinalDataList.FirstOrDefault();
            if (firstRec == null)
            {
                Assert.Fail("No records in final data list");
            }
            Assert.AreEqual(10, rpt.FinalDataList.Count, "Count is incorrect");

            Assert.AreEqual(2957, firstRec.UdidCount, "Udid Count is incorrect");
            Assert.AreEqual("GENERAL", firstRec.UdidText, "Udid Text is incorrect");

            Assert.AreEqual(6693, rpt.FinalDataList.Sum(s => s.UdidCount), "Total Listed Udid Count");

            Assert.AreEqual(9648, rpt.RawDataList.Sum(s => s.UdidCount), "Report Total Udid Count");



            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationInvoiceDate()
        {
            GenerateReportHandoff(DateType.InvoiceDate);
            ManipulateReportHandoffRecords("1", "PREPOST");
            ManipulateReportHandoffRecords("DT:2015,1,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,1,31", "ENDDATE");
            InsertReportHandoff();

            var rpt = (TopBottomUdids)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            var firstRec = rpt.FinalDataList.FirstOrDefault();
            if (firstRec == null)
            {
                Assert.Fail("No records in final data list");
            }
            Assert.AreEqual(10, rpt.FinalDataList.Count, "Count is incorrect");

            Assert.AreEqual(4856, firstRec.UdidCount, "Udid Count is incorrect");
            Assert.AreEqual("GENERAL", firstRec.UdidText, "Udid Text is incorrect");

            Assert.AreEqual(11566, rpt.FinalDataList.Sum(s => s.UdidCount), "Total Listed Udid Count");

            Assert.AreEqual(17245, rpt.RawDataList.Sum(s => s.UdidCount), "Report Total Udid Count");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationBookedDate()
        {
            GenerateReportHandoff(DateType.BookedDate);
            ManipulateReportHandoffRecords("1", "PREPOST");
            ManipulateReportHandoffRecords("DT:2015,1,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,1,31", "ENDDATE");
            InsertReportHandoff();

            var rpt = (TopBottomUdids)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            var firstRec = rpt.FinalDataList.FirstOrDefault();
            if (firstRec == null)
            {
                Assert.Fail("No records in final data list");
            }
            Assert.AreEqual(10, rpt.FinalDataList.Count, "Count is incorrect");

            Assert.AreEqual(5064, firstRec.UdidCount, "Udid Count is incorrect");
            Assert.AreEqual("GENERAL", firstRec.UdidText, "Udid Text is incorrect");

            Assert.AreEqual(12513, rpt.FinalDataList.Sum(s => s.UdidCount), "Total Listed Udid Count");

            Assert.AreEqual(18789, rpt.RawDataList.Sum(s => s.UdidCount), "Report Total Udid Count");

            ClearReportHandoff();
        }

        public void GenerateReportHandoff(DateType dateType)
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2016,1,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2016,1,31", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "HOWMANY", ParmValue = "10", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "USD", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = "2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "58", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibTopUdids", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RBSORTDESCASC", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3400135", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "SORTBY", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDIDNBR", ParmValue = "10", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDIDTEXT", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1591", ParmInOut = "IN", LangCode = "" });

            ManipulateReportHandoffRecords(((int)dateType).ToString(), "DATERANGE");

        }
    }
}
