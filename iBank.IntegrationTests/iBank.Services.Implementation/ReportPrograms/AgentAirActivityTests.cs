using System.Linq;
using Domain.Helper;
using iBank.IntegrationTests.TestTools;
using iBank.Services.Implementation.ReportPrograms.AgentAirActivity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.IntegrationTests.iBank.Services.Implementation.ReportPrograms
{
    [TestClass]
    public class AgentAirActivityTests : BaseUnitTest
    {

        [TestMethod]
        public void NoDataReport()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("1200", "INACCT");

            InsertReportHandoff();

            var rpt = (AgentAirActivity)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(2, rptInfo.ReturnCode, "Return code failed.");
            var containsNoDataMsg = rptInfo.ErrorMessage.Contains(ReportGlobals.ReportMessages.RptMsg_NoData);
            Assert.AreEqual(true, containsNoDataMsg, "Error message failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeDepartureDate()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            InsertReportHandoff();

            var rpt = (AgentAirActivity)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual(500, rpt.FinalDataList.Count, "Count is incorrect");
            Assert.AreEqual(117770.66m, rpt.FinalDataList.Sum(s => s.Airchg), "Total Fare is incorrect");
            Assert.AreEqual(0m, rpt.FinalDataList.Sum(s => s.Svcfee), "Total Service Fee is incorrect");
            Assert.AreEqual(114.60m, rpt.FinalDataList.Sum(s => s.Acommisn), "Total Commission is incorrect");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeInvoiceDate()
        {
            GenerateReportHandoff(DateType.InvoiceDate);
            InsertReportHandoff();

            var rpt = (AgentAirActivity)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            Assert.AreEqual(479, rpt.FinalDataList.Count, "Count is incorrect");
            Assert.AreEqual(140491.80m, rpt.FinalDataList.Sum(s => s.Airchg), "Total Fare is incorrect");
            Assert.AreEqual(0m, rpt.FinalDataList.Sum(s => s.Svcfee), "Total Service Fee is incorrect");
            Assert.AreEqual(116.93m, rpt.FinalDataList.Sum(s => s.Acommisn), "Total Commission is incorrect");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationDepartureDate()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("1","PREPOST");
            ManipulateReportHandoffRecords("DT:2015,1,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,1,5", "ENDDATE");
            InsertReportHandoff();

            var rpt = (AgentAirActivity)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(203, rpt.FinalDataList.Count, "Count is incorrect");
            Assert.AreEqual(54994.42m, rpt.FinalDataList.Sum(s => s.Airchg), "Total Fare is incorrect");
            Assert.AreEqual(0m, rpt.FinalDataList.Sum(s => s.Svcfee), "Total Service Fee is incorrect");
            Assert.AreEqual(0m, rpt.FinalDataList.Sum(s => s.Acommisn), "Total Commission is incorrect");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationInvoiceDate()
        {
            GenerateReportHandoff(DateType.InvoiceDate);
            ManipulateReportHandoffRecords("1", "PREPOST");
            ManipulateReportHandoffRecords("DT:2015,1,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,1,5", "ENDDATE");
            InsertReportHandoff();

            var rpt = (AgentAirActivity)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(2747, rpt.FinalDataList.Count, "Count is incorrect");
            Assert.AreEqual(551596.82m, rpt.FinalDataList.Sum(s => s.Airchg), "Total Fare is incorrect");
            Assert.AreEqual(0m, rpt.FinalDataList.Sum(s => s.Svcfee), "Total Service Fee is incorrect");
            Assert.AreEqual(0m, rpt.FinalDataList.Sum(s => s.Acommisn), "Total Commission is incorrect");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationBookedDate()
        {
            GenerateReportHandoff(DateType.BookedDate);
            ManipulateReportHandoffRecords("1", "PREPOST");
            ManipulateReportHandoffRecords("DT:2015,1,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,1,5", "ENDDATE");
            InsertReportHandoff();

            var rpt = (AgentAirActivity)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            //TODO: Double check these values when report server is back online
            Assert.AreEqual(3905, rpt.FinalDataList.Count, "Count is incorrect");
            Assert.AreEqual(996793.53m, rpt.FinalDataList.Sum(s => s.Airchg), "Total Fare is incorrect");
            Assert.AreEqual(0m, rpt.FinalDataList.Sum(s => s.Svcfee), "Total Service Fee is incorrect");
            Assert.AreEqual(0m, rpt.FinalDataList.Sum(s => s.Acommisn), "Total Commission is incorrect");

            ClearReportHandoff();
        }

        public void GenerateReportHandoff(DateType dateType)
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2016,1,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2016,1,31", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "USD", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = "2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "130", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibAgentAirActivity", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RBAPPLYTOLEGORSEG", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3401753", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "PENDING", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1591", ParmInOut = "IN", LangCode = "" });

            ManipulateReportHandoffRecords(((int)dateType).ToString(), "DATERANGE");
        }
    }
}
