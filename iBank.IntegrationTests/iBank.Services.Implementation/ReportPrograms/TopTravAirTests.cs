using System.Linq;
using Domain.Helper;
using iBank.IntegrationTests.TestTools;
using iBank.Services.Implementation.ReportPrograms.TopBottomTravelerAir;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.IntegrationTests.iBank.Services.Implementation.ReportPrograms
{
    [TestClass]
    public class TopTravAirTests : BaseUnitTest
    {

        [TestMethod]
        public void NoDataReport()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("1200", "INACCT");

            InsertReportHandoff();

            var rpt = (TopTravAir)RunReport();

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

            var rpt = (TopTravAir)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(10, rpt.FinalDataList.Count, "Count is incorrect");
            Assert.AreEqual(86, rpt.FinalDataList.Sum(s => s.Trips), "Total Trips is incorrect");
            Assert.AreEqual(215408.98m, rpt.FinalDataList.Sum(s => s.Amt), "Total Volume Booked is incorrect");
            Assert.AreEqual(2080, rpt.FinalDataList.Sum(s => s.Totbkdays), "Total Commission is incorrect");
            
            Assert.AreEqual(1681, rpt.TotCount, "Report Total Trips is incorrect");
            Assert.AreEqual(1119626.79m, rpt.TotCharge, "Report Total Volume Booked is incorrect");
            Assert.AreEqual(34571, rpt.TotBkDays, "Report Total Commission is incorrect");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeDepartureDateLowFares()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("ON", "CBINCLLOWFARELOSTSVGS");
            InsertReportHandoff();

            var rpt = (TopTravAir)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(10, rpt.FinalDataList.Count, "Count is incorrect");
            Assert.AreEqual(86, rpt.FinalDataList.Sum(s => s.Trips), "Total Trips is incorrect");
            Assert.AreEqual(215408.98m, rpt.FinalDataList.Sum(s => s.Amt), "Total Volume Booked is incorrect");
            Assert.AreEqual(2080, rpt.FinalDataList.Sum(s => s.Totbkdays), "Total Commission is incorrect");
            Assert.AreEqual(1116.43m, rpt.FinalDataList.Sum(s => s.Lostamt), "Lost Savings is incorrect");

            Assert.AreEqual(1681, rpt.TotCount, "Report Total Trips is incorrect");
            Assert.AreEqual(1119626.79m, rpt.TotCharge, "Report Total Volume Booked is incorrect");
            Assert.AreEqual(34571, rpt.TotBkDays, "Report Total Commission is incorrect");
            Assert.AreEqual(12143.20m, rpt.TotLost, "Report Total Lost Savings is incorrect");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeDepartureDateOriginLeg()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("YYZ", "ORIGIN");
            ManipulateReportHandoffRecords("1", "RBAPPLYTOLEGORSEG");
            InsertReportHandoff();

            var rpt = (TopTravAir)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(10, rpt.FinalDataList.Count, "Count is incorrect");
            Assert.AreEqual(64, rpt.FinalDataList.Sum(s => s.Trips), "Total Trips is incorrect");
            Assert.AreEqual(201491.90m, rpt.FinalDataList.Sum(s => s.Amt), "Total Volume Booked is incorrect");
            Assert.AreEqual(1664, rpt.FinalDataList.Sum(s => s.Totbkdays), "Total Commission is incorrect");

            Assert.AreEqual(1080, rpt.TotCount, "Report Total Trips is incorrect");
            Assert.AreEqual(859506.62m, rpt.TotCharge, "Report Total Volume Booked is incorrect");
            Assert.AreEqual(23928, rpt.TotBkDays, "Report Total Commission is incorrect");

            ClearReportHandoff();
        }

        //TODO: Test fails due to "collapse" issue. Not sure if it's Fox or .NET issue. 
        //[TestMethod]
        //public void BackOfficeDepartureDateOriginSeg()
        //{
        //    GenerateReportHandoff(DateType.DepartureDate);
        //    ManipulateReportHandoffRecords("YYZ", "ORIGIN");
        //    ManipulateReportHandoffRecords("2", "RBAPPLYTOLEGORSEG");
        //    InsertReportHandoff();

        //    var rpt = (TopTravAir)RunReport();

        //    var rptInfo = rpt.Globals.ReportInformation;

        //    //check for validity
        //    Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

        //    Assert.AreEqual(10, rpt.FinalDataList.Count, "Count is incorrect");
        //    Assert.AreEqual(64, rpt.FinalDataList.Sum(s => s.Trips), "Total Trips is incorrect");
        //    Assert.AreEqual(201491.90m, rpt.FinalDataList.Sum(s => s.Amt), "Total Volume Booked is incorrect");
        //    Assert.AreEqual(1664, rpt.FinalDataList.Sum(s => s.Totbkdays), "Total Commission is incorrect");

        //    Assert.AreEqual(1058, rpt.TotCount, "Report Total Trips is incorrect");
        //    Assert.AreEqual(843218.18m, rpt.TotCharge, "Report Total Volume Booked is incorrect");
        //    Assert.AreEqual(23928, rpt.TotBkDays, "Report Total Commission is incorrect");

        //    ClearReportHandoff();
        //}

        [TestMethod]
        public void BackOfficeInvoiceDate()
        {
            GenerateReportHandoff(DateType.InvoiceDate);
            InsertReportHandoff();

            var rpt = (TopTravAir)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(10, rpt.FinalDataList.Count, "Count is incorrect");
            Assert.AreEqual(90, rpt.FinalDataList.Sum(s => s.Trips), "Total Trips is incorrect");
            Assert.AreEqual(248816.91m, rpt.FinalDataList.Sum(s => s.Amt), "Total Volume Booked is incorrect");
            Assert.AreEqual(2557, rpt.FinalDataList.Sum(s => s.Totbkdays), "Total Commission is incorrect");

            Assert.AreEqual(1787, rpt.TotCount, "Report Total Trips is incorrect");
            Assert.AreEqual(1141703.38m, rpt.TotCharge, "Report Total Volume Booked is incorrect");
            Assert.AreEqual(38078, rpt.TotBkDays, "Report Total Commission is incorrect");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationDepartureDate()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("1", "PREPOST");
            InsertReportHandoff();

            var rpt = (TopTravAir)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(10, rpt.FinalDataList.Count, "Count is incorrect");
            Assert.AreEqual(10, rpt.FinalDataList.Sum(s => s.Trips), "Total Trips is incorrect");
            Assert.AreEqual(186028.63m, rpt.FinalDataList.Sum(s => s.Amt), "Total Volume Booked is incorrect");
            Assert.AreEqual(296, rpt.FinalDataList.Sum(s => s.Totbkdays), "Total Commission is incorrect");

            Assert.AreEqual(16856, rpt.TotCount, "Report Total Trips is incorrect");
            Assert.AreEqual(10823357.29m, rpt.TotCharge, "Report Total Volume Booked is incorrect");
            Assert.AreEqual(389631, rpt.TotBkDays, "Report Total Commission is incorrect");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationInvoiceDate()
        {
            GenerateReportHandoff(DateType.InvoiceDate);
            ManipulateReportHandoffRecords("1","PREPOST");
            InsertReportHandoff();

            var rpt = (TopTravAir)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(10, rpt.FinalDataList.Count, "Count is incorrect");
            Assert.AreEqual(10, rpt.FinalDataList.Sum(s => s.Trips), "Total Trips is incorrect");
            Assert.AreEqual(180181.23m, rpt.FinalDataList.Sum(s => s.Amt), "Total Volume Booked is incorrect");
            Assert.AreEqual(305, rpt.FinalDataList.Sum(s => s.Totbkdays), "Total Commission is incorrect");

            Assert.AreEqual(15859, rpt.TotCount, "Report Total Trips is incorrect");
            Assert.AreEqual(9818171.85m, rpt.TotCharge, "Report Total Volume Booked is incorrect");
            Assert.AreEqual(363647, rpt.TotBkDays, "Report Total Commission is incorrect");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationBookedDate()
        {
            GenerateReportHandoff(DateType.BookedDate);
            ManipulateReportHandoffRecords("1", "PREPOST");
            InsertReportHandoff();

            var rpt = (TopTravAir)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(10, rpt.FinalDataList.Count, "Count is incorrect");
            Assert.AreEqual(10, rpt.FinalDataList.Sum(s => s.Trips), "Total Trips is incorrect");
            Assert.AreEqual(186028.63m, rpt.FinalDataList.Sum(s => s.Amt), "Total Volume Booked is incorrect");
            Assert.AreEqual(296, rpt.FinalDataList.Sum(s => s.Totbkdays), "Total Commission is incorrect");

            Assert.AreEqual(16873, rpt.TotCount, "Report Total Trips is incorrect");
            Assert.AreEqual(10831336.96m, rpt.TotCharge, "Report Total Volume Booked is incorrect");
            Assert.AreEqual(394532, rpt.TotBkDays, "Report Total Commission is incorrect");

            ClearReportHandoff();
        }

        public void GenerateReportHandoff(DateType dateType)
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2015,1,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2015,10,3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "HOWMANY", ParmValue = "10", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "USD", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = "2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "56", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibTopTravs", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RBAPPLYTOLEGORSEG", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RBSORTDESCASC", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3394051", ParmInOut = "IN", LangCode = "" });
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
