using System.Linq;
using Domain.Helper;
using iBank.IntegrationTests.TestTools;
using iBank.Services.Implementation.ReportPrograms.AgentProductivity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.IntegrationTests.iBank.Services.Implementation.ReportPrograms
{
    [TestClass]
    public class AgentProductivityAirTests : BaseUnitTest
    {
        [TestMethod]
        public void BackOfficeDepartureDateTripCount()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            InsertReportHandoff();

            var rpt = (AgentProductivity)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");
            
            Assert.AreEqual(8, rpt.FinalDataList.Count, "Count is incorrect");

            Assert.AreEqual(165, rpt.FinalDataList.Sum(s => s.Carr1));
            Assert.AreEqual(29, rpt.FinalDataList.Sum(s => s.Carr2));
            Assert.AreEqual(15, rpt.FinalDataList.Sum(s => s.Carr3));
            Assert.AreEqual(8, rpt.FinalDataList.Sum(s => s.Carr4));
            Assert.AreEqual(6, rpt.FinalDataList.Sum(s => s.Carr5));
            Assert.AreEqual(3, rpt.FinalDataList.Sum(s => s.Carr6));
            Assert.AreEqual(1, rpt.FinalDataList.Sum(s => s.Carr7));
            Assert.AreEqual(1, rpt.FinalDataList.Sum(s => s.Carr8));
            Assert.AreEqual(1, rpt.FinalDataList.Sum(s => s.Carr9));

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeDepartureDateFare()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("2", "DDSHOWCOUNTSORFARE");
            InsertReportHandoff();

            var rpt = (AgentProductivity)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(8, rpt.FinalDataList.Count, "Count is incorrect");

            Assert.AreEqual(92864.18m, rpt.FinalDataList.Sum(s => s.Carr1));
            Assert.AreEqual(11585.53m, rpt.FinalDataList.Sum(s => s.Carr2));
            Assert.AreEqual(4480.14m, rpt.FinalDataList.Sum(s => s.Carr3));
            Assert.AreEqual(2953.84m, rpt.FinalDataList.Sum(s => s.Carr4));
            Assert.AreEqual(2116.29m, rpt.FinalDataList.Sum(s => s.Carr5));
            Assert.AreEqual(2063.89m, rpt.FinalDataList.Sum(s => s.Carr6));
            Assert.AreEqual(1271.70m, rpt.FinalDataList.Sum(s => s.Carr7));
            Assert.AreEqual(271.28m, rpt.FinalDataList.Sum(s => s.Carr8));
            Assert.AreEqual(163.81m, rpt.FinalDataList.Sum(s => s.Carr9));

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeInvoiceDateTripCount()
        {
            GenerateReportHandoff(DateType.InvoiceDate);
            InsertReportHandoff();

            var rpt = (AgentProductivity)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(8, rpt.FinalDataList.Count, "Count is incorrect");

            Assert.AreEqual(161, rpt.FinalDataList.Sum(s => s.Carr1));
            Assert.AreEqual(30, rpt.FinalDataList.Sum(s => s.Carr2));
            Assert.AreEqual(12, rpt.FinalDataList.Sum(s => s.Carr3));
            Assert.AreEqual(9, rpt.FinalDataList.Sum(s => s.Carr4));
            Assert.AreEqual(7, rpt.FinalDataList.Sum(s => s.Carr5));
            Assert.AreEqual(4, rpt.FinalDataList.Sum(s => s.Carr6));
            Assert.AreEqual(1, rpt.FinalDataList.Sum(s => s.Carr7));
            Assert.AreEqual(1, rpt.FinalDataList.Sum(s => s.Carr8));
            Assert.AreEqual(1, rpt.FinalDataList.Sum(s => s.Carr9));

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeInvoiceDateFare()
        {
            GenerateReportHandoff(DateType.InvoiceDate);
            ManipulateReportHandoffRecords("2", "DDSHOWCOUNTSORFARE");
            InsertReportHandoff();

            var rpt = (AgentProductivity)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(8, rpt.FinalDataList.Count, "Count is incorrect");

            Assert.AreEqual(114524.54m, rpt.FinalDataList.Sum(s => s.Carr1));
            Assert.AreEqual(12421.21m, rpt.FinalDataList.Sum(s => s.Carr2));
            Assert.AreEqual(3642.99m, rpt.FinalDataList.Sum(s => s.Carr3));
            Assert.AreEqual(3098.53m, rpt.FinalDataList.Sum(s => s.Carr4));
            Assert.AreEqual(2953.84m, rpt.FinalDataList.Sum(s => s.Carr5));
            Assert.AreEqual(2090.13m, rpt.FinalDataList.Sum(s => s.Carr6));
            Assert.AreEqual(1251.86m, rpt.FinalDataList.Sum(s => s.Carr7));
            Assert.AreEqual(271.28m, rpt.FinalDataList.Sum(s => s.Carr8));
            Assert.AreEqual(237.42m, rpt.FinalDataList.Sum(s => s.Carr9));

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationDepartureDateTripCount()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("1", "PREPOST");
            ManipulateReportHandoffRecords("DT:2015,1,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,1,5", "ENDDATE");
            InsertReportHandoff();

            var rpt = (AgentProductivity)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(19, rpt.FinalDataList.Count, "Count is incorrect");

            Assert.AreEqual(24, rpt.FinalDataList.Sum(s => s.Carr1));
            Assert.AreEqual(14, rpt.FinalDataList.Sum(s => s.Carr2));
            Assert.AreEqual(12, rpt.FinalDataList.Sum(s => s.Carr3));
            Assert.AreEqual(11, rpt.FinalDataList.Sum(s => s.Carr4));
            Assert.AreEqual(10, rpt.FinalDataList.Sum(s => s.Carr5));
            Assert.AreEqual(4, rpt.FinalDataList.Sum(s => s.Carr6));
            Assert.AreEqual(2, rpt.FinalDataList.Sum(s => s.Carr7));
            Assert.AreEqual(1, rpt.FinalDataList.Sum(s => s.Carr8));
            Assert.AreEqual(6, rpt.FinalDataList.Sum(s => s.Carr9));

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationDepartureDateFare()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("1", "PREPOST");
            ManipulateReportHandoffRecords("DT:2015,1,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,1,5", "ENDDATE");
            ManipulateReportHandoffRecords("2", "DDSHOWCOUNTSORFARE");
            InsertReportHandoff();

            var rpt = (AgentProductivity)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(19, rpt.FinalDataList.Count, "Count is incorrect");

            Assert.AreEqual(16125.46m, rpt.FinalDataList.Sum(s => s.Carr1));
            Assert.AreEqual(10662.10m, rpt.FinalDataList.Sum(s => s.Carr2));
            Assert.AreEqual(8551.66m, rpt.FinalDataList.Sum(s => s.Carr3));
            Assert.AreEqual(7656.62m, rpt.FinalDataList.Sum(s => s.Carr4));
            Assert.AreEqual(4777.20m, rpt.FinalDataList.Sum(s => s.Carr5));
            Assert.AreEqual(1365.04m, rpt.FinalDataList.Sum(s => s.Carr6));
            Assert.AreEqual(1217.53m, rpt.FinalDataList.Sum(s => s.Carr7));
            Assert.AreEqual(1213.90m, rpt.FinalDataList.Sum(s => s.Carr8));
            Assert.AreEqual(3424.91m, rpt.FinalDataList.Sum(s => s.Carr9));

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationInvoiceDateTripCount()
        {
            GenerateReportHandoff(DateType.InvoiceDate);
            ManipulateReportHandoffRecords("1", "PREPOST");
            ManipulateReportHandoffRecords("DT:2015,1,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,1,5", "ENDDATE");
            InsertReportHandoff();

            var rpt = (AgentProductivity)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(56, rpt.FinalDataList.Count, "Count is incorrect");

            Assert.AreEqual(253, rpt.FinalDataList.Sum(s => s.Carr1));
            Assert.AreEqual(233, rpt.FinalDataList.Sum(s => s.Carr2));
            Assert.AreEqual(200, rpt.FinalDataList.Sum(s => s.Carr3));
            Assert.AreEqual(151, rpt.FinalDataList.Sum(s => s.Carr4));
            Assert.AreEqual(139, rpt.FinalDataList.Sum(s => s.Carr5));
            Assert.AreEqual(31, rpt.FinalDataList.Sum(s => s.Carr6));
            Assert.AreEqual(19, rpt.FinalDataList.Sum(s => s.Carr7));
            Assert.AreEqual(18, rpt.FinalDataList.Sum(s => s.Carr8));
            Assert.AreEqual(52, rpt.FinalDataList.Sum(s => s.Carr9));

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationInvoiceDateFare()
        {
            GenerateReportHandoff(DateType.InvoiceDate);
            ManipulateReportHandoffRecords("1", "PREPOST");
            ManipulateReportHandoffRecords("DT:2015,1,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,1,5", "ENDDATE");
            ManipulateReportHandoffRecords("2", "DDSHOWCOUNTSORFARE");
            InsertReportHandoff();

            var rpt = (AgentProductivity)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(56, rpt.FinalDataList.Count, "Count is incorrect");

            Assert.AreEqual(141628.34m, rpt.FinalDataList.Sum(s => s.Carr1));
            Assert.AreEqual(116530.52m, rpt.FinalDataList.Sum(s => s.Carr2));
            Assert.AreEqual(75918.09m, rpt.FinalDataList.Sum(s => s.Carr3));
            Assert.AreEqual(67284.26m, rpt.FinalDataList.Sum(s => s.Carr4));
            Assert.AreEqual(63440.91m, rpt.FinalDataList.Sum(s => s.Carr5));
            Assert.AreEqual(14095.90m, rpt.FinalDataList.Sum(s => s.Carr6));
            Assert.AreEqual(11540.00m, rpt.FinalDataList.Sum(s => s.Carr7));
            Assert.AreEqual(10586.37m, rpt.FinalDataList.Sum(s => s.Carr8));
            Assert.AreEqual(50572.43m, rpt.FinalDataList.Sum(s => s.Carr9));

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationBookedDateTripCount()
        {
            GenerateReportHandoff(DateType.BookedDate);
            ManipulateReportHandoffRecords("1", "PREPOST");
            ManipulateReportHandoffRecords("DT:2015,1,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,1,5", "ENDDATE");
            InsertReportHandoff();

            var rpt = (AgentProductivity)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(61, rpt.FinalDataList.Count, "Count is incorrect");

            Assert.AreEqual(381, rpt.FinalDataList.Sum(s => s.Carr1));
            Assert.AreEqual(308, rpt.FinalDataList.Sum(s => s.Carr2));
            Assert.AreEqual(236, rpt.FinalDataList.Sum(s => s.Carr3));
            Assert.AreEqual(213, rpt.FinalDataList.Sum(s => s.Carr4));
            Assert.AreEqual(184, rpt.FinalDataList.Sum(s => s.Carr5));
            Assert.AreEqual(48, rpt.FinalDataList.Sum(s => s.Carr6));
            Assert.AreEqual(30, rpt.FinalDataList.Sum(s => s.Carr7));
            Assert.AreEqual(26, rpt.FinalDataList.Sum(s => s.Carr8));
            Assert.AreEqual(86, rpt.FinalDataList.Sum(s => s.Carr9));

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationBookedDateFare()
        {
            GenerateReportHandoff(DateType.BookedDate);
            ManipulateReportHandoffRecords("1", "PREPOST");
            ManipulateReportHandoffRecords("DT:2015,1,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,1,5", "ENDDATE");
            ManipulateReportHandoffRecords("2", "DDSHOWCOUNTSORFARE");
            InsertReportHandoff();

            var rpt = (AgentProductivity)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(61, rpt.FinalDataList.Count, "Count is incorrect");

            Assert.AreEqual(308853.13m, rpt.FinalDataList.Sum(s => s.Carr1));
            Assert.AreEqual(173072.78m, rpt.FinalDataList.Sum(s => s.Carr2));
            Assert.AreEqual(139303.00m, rpt.FinalDataList.Sum(s => s.Carr3));
            Assert.AreEqual(85763.40m, rpt.FinalDataList.Sum(s => s.Carr4));
            Assert.AreEqual(74643.31m, rpt.FinalDataList.Sum(s => s.Carr5));
            Assert.AreEqual(39870.20m, rpt.FinalDataList.Sum(s => s.Carr6));
            Assert.AreEqual(30216.50m, rpt.FinalDataList.Sum(s => s.Carr7));
            Assert.AreEqual(25141.00m, rpt.FinalDataList.Sum(s => s.Carr8));
            Assert.AreEqual(119930.21m, rpt.FinalDataList.Sum(s => s.Carr9));

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationBookedDateFareTwoAirlines()
        {
            GenerateReportHandoff(DateType.BookedDate);
            ManipulateReportHandoffRecords("1", "PREPOST");
            ManipulateReportHandoffRecords("UA,AA", "INVALCARR");
            ManipulateReportHandoffRecords("DT:2015,1,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,1,5", "ENDDATE");
            ManipulateReportHandoffRecords("2", "DDSHOWCOUNTSORFARE");
            InsertReportHandoff();

            var rpt = (AgentProductivity)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(53, rpt.FinalDataList.Count, "Count is incorrect");

            Assert.AreEqual(308853.13m, rpt.FinalDataList.Sum(s => s.Carr1));
            Assert.AreEqual(139303.00m, rpt.FinalDataList.Sum(s => s.Carr2));
            Assert.AreEqual(0m, rpt.FinalDataList.Sum(s => s.Carr3));


            ClearReportHandoff();
        }

        public void GenerateReportHandoff(DateType dateType)
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2016,1,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DDAGENTTYPE", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DDSHOWCOUNTSORFARE", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2016,1,31", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "USD", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = "2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "120", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibAgentProd", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3402291", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1591", ParmInOut = "IN", LangCode = "" });

            ManipulateReportHandoffRecords(((int)dateType).ToString(), "DATERANGE");
        }
    }
}
