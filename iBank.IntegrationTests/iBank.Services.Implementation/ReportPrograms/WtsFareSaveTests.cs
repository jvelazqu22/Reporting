using System.Linq;
using Domain.Helper;
using iBank.IntegrationTests.TestTools;
using iBank.Services.Implementation.ReportPrograms.WtsFareSavings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.IntegrationTests.iBank.Services.Implementation.ReportPrograms
{
    [TestClass]
    public class WtsFareSaveTests : BaseUnitTest
    {

        [TestMethod]
        public void BackOfficeDepartureDate()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            InsertReportHandoff();

            var rpt = (WtsFareSavings)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(229, rpt.FinalDataList.Count, "Count is incorrect.");
            Assert.AreEqual(7, rpt.SummaryData.Count,"Summary Count is incorrect.");
            Assert.AreEqual(117770.66m, rpt.FinalDataList.Sum(s => s.Airchg),"Total Actual Fare is incorrect.");
            Assert.AreEqual(209102.06m, rpt.FinalDataList.Sum(s => s.Stndchg), "Total Full Value Fare is incorrect.");
            Assert.AreEqual(91331.40m, rpt.FinalDataList.Sum(s => s.Savings), "Total Full Value Savings is incorrect.");
            Assert.AreEqual(116729.53m, rpt.FinalDataList.Sum(s => s.Offrdchg), "Total Lowest Logical is incorrect.");
            Assert.AreEqual(1041.13m, rpt.FinalDataList.Sum(s => s.LostAmt), "Total Unrealized Savings is incorrect.");

            var topRow = rpt.SummaryData.OrderByDescending(s => s.Savings).FirstOrDefault();
            Assert.AreEqual(200,topRow.ReasCount, "Count for reason " + topRow.ReasCode + " Should be 200");
            Assert.AreEqual(90480.61m, topRow.Savings, "Savings for reason " + topRow.ReasCode + " Should be 200");
            Assert.AreEqual(7.91m, topRow.LostAmt, "Unrealized Savings for reason " + topRow.ReasCode + " Should be 200");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeInvoiceDate()
        {
            GenerateReportHandoff(DateType.InvoiceDate);
            InsertReportHandoff();

            var rpt = (WtsFareSavings)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(226, rpt.FinalDataList.Count, "Count is incorrect.");
            Assert.AreEqual(7, rpt.SummaryData.Count, "Summary Count is incorrect.");
            Assert.AreEqual(140491.80m, rpt.FinalDataList.Sum(s => s.Airchg), "Total Actual Fare is incorrect.");
            Assert.AreEqual(241506.00m, rpt.FinalDataList.Sum(s => s.Stndchg), "Total Full Value Fare is incorrect.");
            Assert.AreEqual(101014.20m, rpt.FinalDataList.Sum(s => s.Savings), "Total Full Value Savings is incorrect.");
            Assert.AreEqual(139650.46m, rpt.FinalDataList.Sum(s => s.Offrdchg), "Total Lowest Logical is incorrect.");
            Assert.AreEqual(841.34m, rpt.FinalDataList.Sum(s => s.LostAmt), "Total Unrealized Savings is incorrect.");

            var topRow = rpt.SummaryData.OrderByDescending(s => s.Savings).FirstOrDefault();
            Assert.AreEqual(192, topRow.ReasCount, "Count for reason " + topRow.ReasCode + " is incorrect");
            Assert.AreEqual(98976.56m, topRow.Savings, "Savings for reason " + topRow.ReasCode + " is incorrect");
            Assert.AreEqual(10.11m, topRow.LostAmt, "Unrealized Savings for reason " + topRow.ReasCode + " is incorrect");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationDepartureDate()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("1","PREPOST");
            ManipulateReportHandoffRecords("DT:2015,1,9", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,1,10", "ENDDATE");
            InsertReportHandoff();

            var rpt = (WtsFareSavings)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(116, rpt.FinalDataList.Count, "Count is incorrect.");
            Assert.AreEqual(11, rpt.SummaryData.Count, "Summary Count is incorrect.");
            Assert.AreEqual(80229.64m, rpt.FinalDataList.Sum(s => s.Airchg), "Total Actual Fare is incorrect.");
            Assert.AreEqual(109746.06m, rpt.FinalDataList.Sum(s => s.Stndchg), "Total Full Value Fare is incorrect.");
            Assert.AreEqual(29516.42m, rpt.FinalDataList.Sum(s => s.Savings), "Total Full Value Savings is incorrect.");
            Assert.AreEqual(75912.27m, rpt.FinalDataList.Sum(s => s.Offrdchg), "Total Lowest Logical is incorrect.");
            Assert.AreEqual(4317.37m, rpt.FinalDataList.Sum(s => s.LostAmt), "Total Unrealized Savings is incorrect.");

            var topRow = rpt.SummaryData.OrderByDescending(s => s.Savings).FirstOrDefault();
            Assert.AreEqual(46, topRow.ReasCount, "Count for reason " + topRow.ReasCode + " is incorrect");
            Assert.AreEqual(11404.60m, topRow.Savings, "Savings for reason " + topRow.ReasCode + " is incorrect");
            Assert.AreEqual(664.25m, topRow.LostAmt, "Unrealized Savings for reason " + topRow.ReasCode + " is incorrect");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationInvoiceDate()
        {
            GenerateReportHandoff(DateType.InvoiceDate);
            ManipulateReportHandoffRecords("1", "PREPOST");
            ManipulateReportHandoffRecords("DT:2015,1,9", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,1,9", "ENDDATE");
            InsertReportHandoff();

            var rpt = (WtsFareSavings)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(755, rpt.FinalDataList.Count, "Count is incorrect.");
            Assert.AreEqual(16, rpt.SummaryData.Count, "Summary Count is incorrect.");
            Assert.AreEqual(430935.94m, rpt.FinalDataList.Sum(s => s.Airchg), "Total Actual Fare is incorrect.");
            Assert.AreEqual(694319.89m, rpt.FinalDataList.Sum(s => s.Stndchg), "Total Full Value Fare is incorrect.");
            Assert.AreEqual(263383.95m, rpt.FinalDataList.Sum(s => s.Savings), "Total Full Value Savings is incorrect.");
            Assert.AreEqual(396996.16m, rpt.FinalDataList.Sum(s => s.Offrdchg), "Total Lowest Logical is incorrect.");
            Assert.AreEqual(33939.78m, rpt.FinalDataList.Sum(s => s.LostAmt), "Total Unrealized Savings is incorrect.");

            var topRow = rpt.SummaryData.OrderByDescending(s => s.Savings).FirstOrDefault();
            Assert.AreEqual(294, topRow.ReasCount, "Count for reason " + topRow.ReasCode + " is incorrect");
            Assert.AreEqual(117759.37m, topRow.Savings, "Savings for reason " + topRow.ReasCode + " is incorrect");
            Assert.AreEqual(2915.23m, topRow.LostAmt, "Unrealized Savings for reason " + topRow.ReasCode + " is incorrect");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationBookedDate()
        {
            GenerateReportHandoff(DateType.BookedDate);
            ManipulateReportHandoffRecords("1", "PREPOST");
            ManipulateReportHandoffRecords("DT:2015,1,9", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,1,9", "ENDDATE");
            InsertReportHandoff();

            var rpt = (WtsFareSavings)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(821, rpt.FinalDataList.Count, "Count is incorrect.");
            Assert.AreEqual(18, rpt.SummaryData.Count, "Summary Count is incorrect.");
            Assert.AreEqual(502660.34m, rpt.FinalDataList.Sum(s => s.Airchg), "Total Actual Fare is incorrect.");
            Assert.AreEqual(756546.65m, rpt.FinalDataList.Sum(s => s.Stndchg), "Total Full Value Fare is incorrect.");
            Assert.AreEqual(253886.31m, rpt.FinalDataList.Sum(s => s.Savings), "Total Full Value Savings is incorrect.");
            Assert.AreEqual(434049.53m, rpt.FinalDataList.Sum(s => s.Offrdchg), "Total Lowest Logical is incorrect.");
            Assert.AreEqual(68610.81m, rpt.FinalDataList.Sum(s => s.LostAmt), "Total Unrealized Savings is incorrect.");

            var topRow = rpt.SummaryData.OrderByDescending(s => s.Savings).FirstOrDefault();
            Assert.AreEqual(295, topRow.ReasCount, "Count for reason " + topRow.ReasCode + " is incorrect");
            Assert.AreEqual(116532.52m, topRow.Savings, "Savings for reason " + topRow.ReasCode + " is incorrect");
            Assert.AreEqual(2666.79m, topRow.LostAmt, "Unrealized Savings for reason " + topRow.ReasCode + " is incorrect");

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
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "701", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "WTSFareSave", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3402557", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1591", ParmInOut = "IN", LangCode = "" });

            ManipulateReportHandoffRecords(((int)dateType).ToString(), "DATERANGE");
        }
    }
}
