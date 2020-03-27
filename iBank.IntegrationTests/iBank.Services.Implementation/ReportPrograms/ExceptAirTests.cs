using System.Linq;
using Domain.Helper;
using iBank.IntegrationTests.TestTools;
using iBank.Services.Implementation.ReportPrograms;
using iBank.Services.Implementation.ReportPrograms.ExceptAir;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.IntegrationTests.iBank.Services.Implementation.ReportPrograms
{
    [TestClass]
    public class ExceptAirTests : BaseUnitTest
    {

        [TestMethod]
        public void GenerateReportTooMuchData()
        {
            GenerateReportHandoff(DateType.BookedDate);
            ManipulateReportHandoffRecords("DT:2015,1,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,6,23", "ENDDATE");
            ManipulateReportHandoffRecords("1", "PREPOST");
            InsertReportHandoff();

            //run the report
            var rpt = (ExceptAir)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(2, rptInfo.ReturnCode, "Return code failed.");
            var containsNoDataMsg = rptInfo.ErrorMessage.Contains(ReportGlobals.ReportMessages.RptMsg_BigVolume);
            Assert.AreEqual(true, containsNoDataMsg, "Error message failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportBackOfficeDepartureDate()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            InsertReportHandoff();

            //run the report
            var rpt = (ExceptAir)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            var totalAmtPaid = rpt.FinalDataList.GroupBy(s => s.Reckey, (key,recs) => recs.First().Airchg).Sum();
            var totalAmtOffered = rpt.FinalDataList.GroupBy(s => s.Reckey, (key, recs) => recs.First().Offrdchg).Sum();
            var totalAmtLost = rpt.FinalDataList.GroupBy(s => s.Reckey, (key, recs) => recs.First().Lostamt).Sum();

            Assert.AreEqual(8493.00m, totalAmtPaid);
            Assert.AreEqual(6736.86m, totalAmtOffered);
            Assert.AreEqual(1756.14m, totalAmtLost);
            

            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportBackOfficeInvoiceDate()
        {
            GenerateReportHandoff(DateType.InvoiceDate);
            InsertReportHandoff();

            //run the report
            var rpt = (ExceptAir)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            var totalAmtPaid = rpt.FinalDataList.GroupBy(s => s.Reckey, (key, recs) => recs.First().Airchg).Sum();
            var totalAmtOffered = rpt.FinalDataList.GroupBy(s => s.Reckey, (key, recs) => recs.First().Offrdchg).Sum();
            var totalAmtLost = rpt.FinalDataList.GroupBy(s => s.Reckey, (key, recs) => recs.First().Lostamt).Sum();

            Assert.AreEqual(5898.03m, totalAmtPaid);
            Assert.AreEqual(4943.45m, totalAmtOffered);
            Assert.AreEqual(954.58m, totalAmtLost);


            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportReservationDepartureDate()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("DT:2015,7,1", "BEGDATE");
            ManipulateReportHandoffRecords("1","PREPOST");
            InsertReportHandoff();

            //run the report
            var rpt = (ExceptAir)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            var totalAmtPaid = rpt.FinalDataList.GroupBy(s => s.Reckey, (key, recs) => recs.First().Airchg).Sum();
            var totalAmtOffered = rpt.FinalDataList.GroupBy(s => s.Reckey, (key, recs) => recs.First().Offrdchg).Sum();
            var totalAmtLost = rpt.FinalDataList.GroupBy(s => s.Reckey, (key, recs) => recs.First().Lostamt).Sum();

            Assert.AreEqual(19434.16m, totalAmtPaid);
            Assert.AreEqual(6805.60m, totalAmtOffered);
            Assert.AreEqual(12628.56m, totalAmtLost);


            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportReservationInvoiceDate()
        {
            GenerateReportHandoff(DateType.InvoiceDate);
            ManipulateReportHandoffRecords("DT:2015,6,1", "BEGDATE");
            ManipulateReportHandoffRecords("1", "PREPOST");
            InsertReportHandoff();

            //run the report
            var rpt = (ExceptAir)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            var totalAmtPaid = rpt.FinalDataList.GroupBy(s => s.Reckey, (key, recs) => recs.First().Airchg).Sum();
            var totalAmtOffered = rpt.FinalDataList.GroupBy(s => s.Reckey, (key, recs) => recs.First().Offrdchg).Sum();
            var totalAmtLost = rpt.FinalDataList.GroupBy(s => s.Reckey, (key, recs) => recs.First().Lostamt).Sum();

            Assert.AreEqual(13516.90m, totalAmtPaid);
            Assert.AreEqual(5143.80m, totalAmtOffered);
            Assert.AreEqual(8373.10m, totalAmtLost);

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
            var rpt = (ExceptAir)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            var totalAmtPaid = rpt.FinalDataList.GroupBy(s => s.Reckey, (key, recs) => recs.First().Airchg).Sum();
            var totalAmtOffered = rpt.FinalDataList.GroupBy(s => s.Reckey, (key, recs) => recs.First().Offrdchg).Sum();
            var totalAmtLost = rpt.FinalDataList.GroupBy(s => s.Reckey, (key, recs) => recs.First().Lostamt).Sum();

            Assert.AreEqual(17350.62m, totalAmtPaid);
            Assert.AreEqual(12184.49m, totalAmtOffered);
            Assert.AreEqual(5166.13m, totalAmtLost);


            ClearReportHandoff();
        }


        public void GenerateReportHandoff(DateType dateType)
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2015,8,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2015,8,29", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "USD", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = "2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "100", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibExceptAir", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3384199", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1591", ParmInOut = "IN", LangCode = "" });

            ManipulateReportHandoffRecords(((int)dateType).ToString(), "DATERANGE");

        }
    }
}
