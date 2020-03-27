using System.Linq;
using Domain.Helper;
using iBank.IntegrationTests.TestTools;
using iBank.Services.Implementation.ReportPrograms.AgentSummary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.IntegrationTests.iBank.Services.Implementation.ReportPrograms
{
    [TestClass]
    public class AgentSummaryTests : BaseUnitTest
    {
        [TestMethod]
        public void BackOfficeDepartureDate()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            InsertReportHandoff();

            var rpt = (AgentSummary)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(8, rpt.FinalDataList.Count, "Count is incorrect");

            Assert.AreEqual(229,rpt.FinalDataList.Sum(s => s.Transacts));
            Assert.AreEqual(229, rpt.FinalDataList.Sum(s => s.Tickets));
            Assert.AreEqual(0, rpt.FinalDataList.Sum(s => s.Refunds));
            Assert.AreEqual(229, rpt.FinalDataList.Sum(s => s.Net_trips));
            Assert.AreEqual(114.60m, rpt.FinalDataList.Sum(s => s.Commission));
            Assert.AreEqual(117770.66m, rpt.FinalDataList.Sum(s => s.Invoiceamt));
            Assert.AreEqual(0m, rpt.FinalDataList.Sum(s => s.Creditamt));
            Assert.AreEqual(117770.66m, rpt.FinalDataList.Sum(s => s.Netvolume));

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeInvoiceDate()
        {
            GenerateReportHandoff(DateType.InvoiceDate);
            InsertReportHandoff();

            var rpt = (AgentSummary)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(8, rpt.FinalDataList.Count, "Count is incorrect");

            Assert.AreEqual(226, rpt.FinalDataList.Sum(s => s.Transacts));
            Assert.AreEqual(226, rpt.FinalDataList.Sum(s => s.Tickets));
            Assert.AreEqual(0, rpt.FinalDataList.Sum(s => s.Refunds));
            Assert.AreEqual(226, rpt.FinalDataList.Sum(s => s.Net_trips));
            Assert.AreEqual(116.93m, rpt.FinalDataList.Sum(s => s.Commission));
            Assert.AreEqual(140491.80m, rpt.FinalDataList.Sum(s => s.Invoiceamt));
            Assert.AreEqual(0m, rpt.FinalDataList.Sum(s => s.Creditamt));
            Assert.AreEqual(140491.80m, rpt.FinalDataList.Sum(s => s.Netvolume));

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeBookedDate()
        {
            GenerateReportHandoff(DateType.BookedDate);
            InsertReportHandoff();

            var rpt = (AgentSummary)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(8, rpt.FinalDataList.Count, "Count is incorrect");

            Assert.AreEqual(211, rpt.FinalDataList.Sum(s => s.Transacts));
            Assert.AreEqual(211, rpt.FinalDataList.Sum(s => s.Tickets));
            Assert.AreEqual(0, rpt.FinalDataList.Sum(s => s.Refunds));
            Assert.AreEqual(211, rpt.FinalDataList.Sum(s => s.Net_trips));
            Assert.AreEqual(107.96m, rpt.FinalDataList.Sum(s => s.Commission));
            Assert.AreEqual(133568.99m, rpt.FinalDataList.Sum(s => s.Invoiceamt));
            Assert.AreEqual(0m, rpt.FinalDataList.Sum(s => s.Creditamt));
            Assert.AreEqual(133568.99m, rpt.FinalDataList.Sum(s => s.Netvolume));

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
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "128", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibAgentSummary", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3402356", ParmInOut = "IN", LangCode = "" });
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
