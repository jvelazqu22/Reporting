using System.Linq;
using Domain.Helper;
using iBank.IntegrationTests.TestTools;
using iBank.Services.Implementation.ReportPrograms.ValidatingCarrier;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.IntegrationTests.iBank.Services.Implementation.ReportPrograms
{
    [TestClass]
    public class ValidatingCarrierTests : BaseUnitTest
    {
        [TestMethod]
        public void GenerateReportNoData()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("DT:2016,6,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2016,9,1", "ENDDATE");

            InsertReportHandoff();

            //run the report
            var rpt = (AgcyValCarr)RunReport();
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
            ManipulateReportHandoffRecords("DT:2015,6,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,9,1", "ENDDATE");

            InsertReportHandoff();

            //run the report
            var rpt = (AgcyValCarr)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            var transactions = rpt.FinalDataList.Sum(s => s.Transacts);
            var tickets = rpt.FinalDataList.Sum(s => s.Tickets);
            var refunds = rpt.FinalDataList.Sum(s => s.Refunds);
            var nettrips = rpt.FinalDataList.Sum(s => s.Net_trips);
            var commission = rpt.FinalDataList.Sum(s => s.Commission);
            var invoiceamt = rpt.FinalDataList.Sum(s => s.Invoiceamt);
            var creditamt = rpt.FinalDataList.Sum(s => s.Creditamt);
            var netvolume = rpt.FinalDataList.Sum(s => s.Netvolume);

            Assert.AreEqual(477, transactions, "Return code failed.");
            Assert.AreEqual(475, tickets, "Return code failed.");
            Assert.AreEqual(2, refunds, "Return code failed.");
            Assert.AreEqual(473, nettrips, "Return code failed.");
            Assert.AreEqual(782.67m, commission, "Return code failed.");
            Assert.AreEqual(317696.41m, invoiceamt, "Return code failed.");
            Assert.AreEqual(1418.51m, creditamt, "Return code failed.");
            Assert.AreEqual(316277.90m, netvolume, "Return code failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportBackofficeDepartureDate()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("DT:2015,6,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,9,1", "ENDDATE");

            InsertReportHandoff();

            //run the report
            var rpt = (AgcyValCarr)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            var transactions = rpt.FinalDataList.Sum(s => s.Transacts);
            var tickets = rpt.FinalDataList.Sum(s => s.Tickets);
            var refunds = rpt.FinalDataList.Sum(s => s.Refunds);
            var nettrips = rpt.FinalDataList.Sum(s => s.Net_trips);
            var commission = rpt.FinalDataList.Sum(s => s.Commission);
            var invoiceamt = rpt.FinalDataList.Sum(s => s.Invoiceamt);
            var creditamt = rpt.FinalDataList.Sum(s => s.Creditamt);
            var netvolume = rpt.FinalDataList.Sum(s => s.Netvolume);

            Assert.AreEqual(496, transactions, "Return code failed.");
            Assert.AreEqual(494, tickets, "Return code failed.");
            Assert.AreEqual(2, refunds, "Return code failed.");
            Assert.AreEqual(492, nettrips, "Return code failed.");
            Assert.AreEqual(838.57m, commission, "Return code failed.");
            Assert.AreEqual(321476.48m, invoiceamt, "Return code failed.");
            Assert.AreEqual(1252.19m, creditamt, "Return code failed.");
            Assert.AreEqual(320224.29m, netvolume, "Return code failed.");

            ClearReportHandoff();
        }

        public void GenerateReportHandoff(DateType dateType)
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2016,6,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2016,9,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "USD", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "126", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibAgcyValCarr", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3384712", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "SORTBY", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1591", ParmInOut = "IN", LangCode = "" });

            ManipulateReportHandoffRecords(((int)dateType).ToString(), "DATERANGE");

        }
    }
}
