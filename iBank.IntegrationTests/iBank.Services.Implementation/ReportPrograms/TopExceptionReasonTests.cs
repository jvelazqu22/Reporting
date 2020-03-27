using System.Linq;
using Domain.Helper;
using iBank.IntegrationTests.TestTools;
using iBank.Services.Implementation.ReportPrograms.TopBottomExceptionReason;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.IntegrationTests.iBank.Services.Implementation.ReportPrograms
{
    [TestClass]
    public class TopExceptionReasonTests : BaseUnitTest
    {

        [TestMethod]
        public void NoDataReport()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("1200", "INACCT");

            InsertReportHandoff();

            var rpt = (TopExceptions)RunReport();

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

            var rpt = (TopExceptions)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(8, rpt.FinalDataList.Count, "Count is incorrect");
            Assert.AreEqual(188, rpt.FinalDataList.Sum(s => s.NumOccurs), "Total Stays is incorrect");
            Assert.AreEqual(23458.86m, rpt.FinalDataList.Sum(s => s.LostAmt), "Total Cost is incorrect");

            Assert.AreEqual(191, rpt.TotCount, "Report Total Stays is incorrect");
            Assert.AreEqual(19461.30m, rpt.TotLost, "Report Total Cost is incorrect");
            

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeInvoiceDate()
        {
            GenerateReportHandoff(DateType.InvoiceDate);
            InsertReportHandoff();

            var rpt = (TopExceptions)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(8, rpt.FinalDataList.Count, "Count is incorrect");
            Assert.AreEqual(188, rpt.FinalDataList.Sum(s => s.NumOccurs), "Total Stays is incorrect");
            Assert.AreEqual(23458.86m, rpt.FinalDataList.Sum(s => s.LostAmt), "Total Cost is incorrect");

            Assert.AreEqual(191, rpt.TotCount, "Report Total Stays is incorrect");
            Assert.AreEqual(19461.30m, rpt.TotLost, "Report Total Cost is incorrect");


            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationDepartureDate()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("1", "PREPOST");
            ManipulateReportHandoffRecords("1200", "INACCT");
            ManipulateReportHandoffRecords("10", "HOWMANY");
            ManipulateReportHandoffRecords("DT:2015,1,30", "ENDDATE");
            InsertReportHandoff();

            var rpt = (TopExceptions)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(10, rpt.FinalDataList.Count, "Count is incorrect");
            Assert.AreEqual(59, rpt.FinalDataList.Sum(s => s.NumOccurs), "Total Stays is incorrect");
            Assert.AreEqual(63018.82m, rpt.FinalDataList.Sum(s => s.LostAmt), "Total Cost is incorrect");

            Assert.AreEqual(69, rpt.TotCount, "Report Total Stays is incorrect");
            Assert.AreEqual(63103.43m, rpt.TotLost, "Report Total Cost is incorrect");


            ClearReportHandoff();
        }


        [TestMethod]
        public void ReservationInvoiceDate()
        {
            GenerateReportHandoff(DateType.InvoiceDate);
            ManipulateReportHandoffRecords("1", "PREPOST");
            ManipulateReportHandoffRecords("1200", "INACCT");
            ManipulateReportHandoffRecords("10", "HOWMANY");
            ManipulateReportHandoffRecords("DT:2015,1,30", "ENDDATE");
            InsertReportHandoff();

            var rpt = (TopExceptions)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(10, rpt.FinalDataList.Count, "Count is incorrect");
            Assert.AreEqual(84, rpt.FinalDataList.Sum(s => s.NumOccurs), "Total Stays is incorrect");
            Assert.AreEqual(65864.95m, rpt.FinalDataList.Sum(s => s.LostAmt), "Total Cost is incorrect");

            Assert.AreEqual(113, rpt.TotCount, "Report Total Stays is incorrect");
            Assert.AreEqual(66465.87m, rpt.TotLost, "Report Total Cost is incorrect");


            ClearReportHandoff();
        }


        [TestMethod]
        public void ReservationBookedDate()
        {
            GenerateReportHandoff(DateType.BookedDate);
            ManipulateReportHandoffRecords("1", "PREPOST");
            ManipulateReportHandoffRecords("1200", "INACCT");
            ManipulateReportHandoffRecords("10", "HOWMANY");
            ManipulateReportHandoffRecords("DT:2015,1,30", "ENDDATE");
            InsertReportHandoff();

            var rpt = (TopExceptions)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(10, rpt.FinalDataList.Count, "Count is incorrect");
            Assert.AreEqual(146, rpt.FinalDataList.Sum(s => s.NumOccurs), "Total Stays is incorrect");
            Assert.AreEqual(252128.04m, rpt.FinalDataList.Sum(s => s.LostAmt), "Total Cost is incorrect");

            Assert.AreEqual(185, rpt.TotCount, "Report Total Stays is incorrect");
            Assert.AreEqual(257412.54m, rpt.TotLost, "Report Total Cost is incorrect");


            ClearReportHandoff();
        }



        public void GenerateReportHandoff(DateType dateType)
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2015,1,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2016,12,30", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "HOWMANY", ParmValue = "8", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "USD", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = "2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "60", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibTopExceptions", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RBSORTDESCASC", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3394142", ParmInOut = "IN", LangCode = "" });
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
