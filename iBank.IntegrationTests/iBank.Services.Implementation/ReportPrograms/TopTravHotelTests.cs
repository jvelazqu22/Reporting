using System.Linq;
using Domain.Helper;
using iBank.IntegrationTests.TestTools;
using iBank.Services.Implementation.ReportPrograms.HotelTopBottomTravelers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.IntegrationTests.iBank.Services.Implementation.ReportPrograms
{
    [TestClass]
    public class TopTravHotelTests : BaseUnitTest
    {

        [TestMethod]
        public void NoDataReport()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("1200", "INACCT");

            InsertReportHandoff();

            var rpt = (TopTravHotel)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(2, rptInfo.ReturnCode, "Return code failed.");
            var containsNoDataMsg = rptInfo.ErrorMessage.Contains(ReportGlobals.ReportMessages.RptMsg_NoData);
            Assert.AreEqual(true, containsNoDataMsg, "Error message failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeCheckInDate()
        {
            GenerateReportHandoff(DateType.HotelCheckInDate);
            InsertReportHandoff();

            var rpt = (TopTravHotel)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(10, rpt.FinalDataList.Count, "Count is incorrect");
            Assert.AreEqual(34, rpt.FinalDataList.Sum(s => s.Stays), "Total Stays is incorrect");
            Assert.AreEqual(97, rpt.FinalDataList.Sum(s => s.Nights), "Total Nights is incorrect");
            Assert.AreEqual(13998.99m, rpt.FinalDataList.Sum(s => s.HotelCost), "Total Cost is incorrect");
            Assert.AreEqual(30, rpt.FinalDataList.Sum(s => s.BookCnt), "Total Book Count is incorrect");

            Assert.AreEqual(250, rpt.TotCount, "Report Total Stays is incorrect");
            Assert.AreEqual(483, rpt.TotNights, "Report Total Nights is incorrect");
            Assert.AreEqual(42587.23m, rpt.TotCost, "Report Total Cost is incorrect");
            Assert.AreEqual(129, rpt.TotBookCount, "Report Total Book Count is incorrect");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeCheckInDateLimit5()
        {
            GenerateReportHandoff(DateType.HotelCheckInDate);
            ManipulateReportHandoffRecords("5", "HOWMANY");
            InsertReportHandoff();

            var rpt = (TopTravHotel)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(5, rpt.FinalDataList.Count, "Count is incorrect");
            Assert.AreEqual(22, rpt.FinalDataList.Sum(s => s.Stays), "Total Stays is incorrect");
            Assert.AreEqual(63, rpt.FinalDataList.Sum(s => s.Nights), "Total Nights is incorrect");
            Assert.AreEqual(8577.42m, rpt.FinalDataList.Sum(s => s.HotelCost), "Total Cost is incorrect");
            Assert.AreEqual(18, rpt.FinalDataList.Sum(s => s.BookCnt), "Total Book Count is incorrect");

            Assert.AreEqual(250, rpt.TotCount, "Report Total Stays is incorrect");
            Assert.AreEqual(483, rpt.TotNights, "Report Total Nights is incorrect");
            Assert.AreEqual(42587.23m, rpt.TotCost, "Report Total Cost is incorrect");
            Assert.AreEqual(129, rpt.TotBookCount, "Report Total Book Count is incorrect");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeInvoiceDate()
        {
            GenerateReportHandoff(DateType.InvoiceDate);
            InsertReportHandoff();

            var rpt = (TopTravHotel)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(10, rpt.FinalDataList.Count, "Count is incorrect");
            Assert.AreEqual(29, rpt.FinalDataList.Sum(s => s.Stays), "Total Stays is incorrect");
            Assert.AreEqual(85, rpt.FinalDataList.Sum(s => s.Nights), "Total Nights is incorrect");
            Assert.AreEqual(11584.37m, rpt.FinalDataList.Sum(s => s.HotelCost), "Total Cost is incorrect");
            Assert.AreEqual(25, rpt.FinalDataList.Sum(s => s.BookCnt), "Total Book Count is incorrect");

            Assert.AreEqual(186, rpt.TotCount, "Report Total Stays is incorrect");
            Assert.AreEqual(346, rpt.TotNights, "Report Total Nights is incorrect");
            Assert.AreEqual(29038.97m, rpt.TotCost, "Report Total Cost is incorrect");
            Assert.AreEqual(93, rpt.TotBookCount, "Report Total Book Count is incorrect");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationInvoiceDate()
        {
            GenerateReportHandoff(DateType.InvoiceDate);
            ManipulateReportHandoffRecords("1","PREPOST");
            InsertReportHandoff();

            var rpt = (TopTravHotel)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(5, rpt.FinalDataList.Count, "Count is incorrect");
            Assert.AreEqual(5, rpt.FinalDataList.Sum(s => s.Stays), "Total Stays is incorrect");
            Assert.AreEqual(15, rpt.FinalDataList.Sum(s => s.Nights), "Total Nights is incorrect");
            Assert.AreEqual(3957.10m, rpt.FinalDataList.Sum(s => s.HotelCost), "Total Cost is incorrect");
            Assert.AreEqual(5, rpt.FinalDataList.Sum(s => s.BookCnt), "Total Book Count is incorrect");

            Assert.AreEqual(5, rpt.TotCount, "Report Total Stays is incorrect");
            Assert.AreEqual(15, rpt.TotNights, "Report Total Nights is incorrect");
            Assert.AreEqual(3957.10m, rpt.TotCost, "Report Total Cost is incorrect");
            Assert.AreEqual(5, rpt.TotBookCount, "Report Total Book Count is incorrect");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationBookedDate()
        {
            GenerateReportHandoff(DateType.BookedDate);
            ManipulateReportHandoffRecords("1", "PREPOST");
            ManipulateReportHandoffRecords("DT:2015,1,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,3,30", "ENDDATE");
            InsertReportHandoff();

            var rpt = (TopTravHotel)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(10, rpt.FinalDataList.Count, "Count is incorrect");
            Assert.AreEqual(22, rpt.FinalDataList.Sum(s => s.Stays), "Total Stays is incorrect");
            Assert.AreEqual(315, rpt.FinalDataList.Sum(s => s.Nights), "Total Nights is incorrect");
            Assert.AreEqual(87514.15m, rpt.FinalDataList.Sum(s => s.HotelCost), "Total Cost is incorrect");
            Assert.AreEqual(20, rpt.FinalDataList.Sum(s => s.BookCnt), "Total Book Count is incorrect");

            Assert.AreEqual(10243, rpt.TotCount, "Report Total Stays is incorrect");
            Assert.AreEqual(22678, rpt.TotNights, "Report Total Nights is incorrect");
            Assert.AreEqual(3127056.01m, rpt.TotCost, "Report Total Cost is incorrect");
            Assert.AreEqual(9325, rpt.TotBookCount, "Report Total Book Count is incorrect");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationCheckInDate()
        {
            GenerateReportHandoff(DateType.HotelCheckInDate);
            ManipulateReportHandoffRecords("1", "PREPOST");
            ManipulateReportHandoffRecords("DT:2015,1,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,3,30", "ENDDATE");
            InsertReportHandoff();

            var rpt = (TopTravHotel)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(10, rpt.FinalDataList.Count, "Count is incorrect");
            Assert.AreEqual(19, rpt.FinalDataList.Sum(s => s.Stays), "Total Stays is incorrect");
            Assert.AreEqual(324, rpt.FinalDataList.Sum(s => s.Nights), "Total Nights is incorrect");
            Assert.AreEqual(86903.15m, rpt.FinalDataList.Sum(s => s.HotelCost), "Total Cost is incorrect");
            Assert.AreEqual(17, rpt.FinalDataList.Sum(s => s.BookCnt), "Total Book Count is incorrect");

            Assert.AreEqual(10007, rpt.TotCount, "Report Total Stays is incorrect");
            Assert.AreEqual(22018, rpt.TotNights, "Report Total Nights is incorrect");
            Assert.AreEqual(2999840.99m, rpt.TotCost, "Report Total Cost is incorrect");
            Assert.AreEqual(9110, rpt.TotBookCount, "Report Total Book Count is incorrect");

            ClearReportHandoff();
        }

        public void GenerateReportHandoff(DateType dateType)
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2016,1,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "7", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2016,10,3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "HOWMANY", ParmValue = "10", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "USD", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = "2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "57", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibTopTravHotel", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RBSORTDESCASC", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3394118", ParmInOut = "IN", LangCode = "" });
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
