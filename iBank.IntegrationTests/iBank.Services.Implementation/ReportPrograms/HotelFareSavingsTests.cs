using System.Linq;
using Domain.Helper;
using iBank.IntegrationTests.TestTools;
using iBank.Services.Implementation.ReportPrograms.HotelFareSavings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.IntegrationTests.iBank.Services.Implementation.ReportPrograms
{
    [TestClass]
    public class HotelFareSavingsTests : BaseUnitTest
    {

        [TestMethod]
        public void GenerateReportNoData()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("DT:2014,1,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2014,1,1", "ENDDATE");

            InsertReportHandoff();

            //run the report
            var rpt = (HotelSavings)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(2, rptInfo.ReturnCode, "Return code failed.");
            var containsNoDataMsg = rptInfo.ErrorMessage.Contains(ReportGlobals.ReportMessages.RptMsg_NoData);
            Assert.AreEqual(true, containsNoDataMsg, "Error message failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportTooMuchData()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("DT:2015,1,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,1,31", "ENDDATE");
            ManipulateReportHandoffRecords("1", "PREPOST");

            InsertReportHandoff();

            //run the report
            var rpt = (HotelSavings)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(2, rptInfo.ReturnCode, "Return code failed.");
            var containsNoDataMsg = rptInfo.ErrorMessage.Contains(ReportGlobals.ReportMessages.RptMsg_BigVolume);
            Assert.AreEqual(true, containsNoDataMsg, "Error message failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportBackOfficeInvoiceDate()
        {
            GenerateReportHandoff(DateType.InvoiceDate);
            ManipulateReportHandoffRecords("DT:2015,8,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,9,1", "ENDDATE");

            InsertReportHandoff();

            //run the report
            var rpt = (HotelSavings)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(84, rpt.FinalDataList.Count, "Final Count incorrect.");
            Assert.AreEqual(8, rpt.SubReportList.Count, "Subreport Count incorrect.");
            var svgTotal = rpt.SubReportList.Sum(s => s.Svgamt);
            Assert.AreEqual(12461.24m, svgTotal, "Subreport Loss Total incorrect.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportBackOfficeCheckInDate()
        {
            GenerateReportHandoff(DateType.HotelCheckInDate);
            ManipulateReportHandoffRecords("DT:2015,8,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,9,1", "ENDDATE");

            InsertReportHandoff();

            //run the report
            var rpt = (HotelSavings)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(86, rpt.FinalDataList.Count, "Final Count incorrect.");
            Assert.AreEqual(7, rpt.SubReportList.Count, "Subreport Count incorrect.");
            var svgTotal = rpt.SubReportList.Sum(s => s.Svgamt);
            Assert.AreEqual(12743.49m, svgTotal, "Subreport Loss Total incorrect.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportReservationCheckInDate()
        {
            GenerateReportHandoff(DateType.HotelCheckInDate);
            ManipulateReportHandoffRecords("DT:2015,8,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,9,1", "ENDDATE");
            ManipulateReportHandoffRecords("1", "PREPOST");

            InsertReportHandoff();

            //run the report
            var rpt = (HotelSavings)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(6, rpt.FinalDataList.Count, "Final Count incorrect.");
            Assert.AreEqual(1, rpt.SubReportList.Count, "Subreport Count incorrect.");
            var svgTotal = rpt.SubReportList.Sum(s => s.Svgamt);
            Assert.AreEqual(0m, svgTotal, "Subreport Loss Total incorrect.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportReservationBookedDate()
        {
            GenerateReportHandoff(DateType.BookedDate);
            ManipulateReportHandoffRecords("DT:2015,1,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,1,2", "ENDDATE");
            ManipulateReportHandoffRecords("1", "PREPOST");
            InsertReportHandoff();

            //run the report
            var rpt = (HotelSavings)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(317, rpt.FinalDataList.Count, "Final Count incorrect.");
            Assert.AreEqual(1, rpt.SubReportList.Count, "Subreport Count incorrect.");
            var svgTotal = rpt.SubReportList.Sum(s => s.Svgamt);
            Assert.AreEqual(0m, svgTotal, "Subreport Loss Total incorrect.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportReservationInvoiceDate()
        {
            GenerateReportHandoff(DateType.InvoiceDate);
            ManipulateReportHandoffRecords("DT:2016,1,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2016,1,31", "ENDDATE");
            ManipulateReportHandoffRecords("1", "PREPOST");
            InsertReportHandoff();

            //run the report
            var rpt = (HotelSavings)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(5, rpt.FinalDataList.Count, "Final Count incorrect.");
            Assert.AreEqual(1, rpt.SubReportList.Count, "Subreport Count incorrect.");
            var svgTotal = rpt.SubReportList.Sum(s => s.Svgamt);
            Assert.AreEqual(0m, svgTotal, "Subreport Loss Total incorrect.");

            ClearReportHandoff();
        }

        public void GenerateReportHandoff(DateType dateType)
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2015,8,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "7", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2015,9,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "USD", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTLANGUAGE", ParmValue = "EN", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = "2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "92", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibHotelSavings", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3384567", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UdidLbl1", ParmValue = "Label 1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UdidLbl10", ParmValue = "Label 10", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UdidLbl2", ParmValue = "Label 2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UdidLbl3", ParmValue = "Label 3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UdidLbl4", ParmValue = "Label 4", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UdidLbl5", ParmValue = "Label 5", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UdidLbl6", ParmValue = "Label 6", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UdidLbl7", ParmValue = "Label 7", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UdidLbl8", ParmValue = "Label 8", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UdidLbl9", ParmValue = "Label 9", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UdidOnRpt1", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UdidOnRpt10", ParmValue = "10", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UdidOnRpt2", ParmValue = "2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UdidOnRpt3", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UdidOnRpt4", ParmValue = "4", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UdidOnRpt5", ParmValue = "5", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UdidOnRpt6", ParmValue = "6", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UdidOnRpt7", ParmValue = "7", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UdidOnRpt8", ParmValue = "8", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UdidOnRpt9", ParmValue = "9", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1591", ParmInOut = "IN", LangCode = "" });

            ManipulateReportHandoffRecords(((int)dateType).ToString(), "DATERANGE");

        }
    }
}
