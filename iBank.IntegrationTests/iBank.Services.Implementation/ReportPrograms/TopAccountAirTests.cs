using System.Linq;
using Domain.Helper;
using iBank.IntegrationTests.TestTools;
using iBank.Services.Implementation.ReportPrograms.TopAccountAir;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.IntegrationTests.iBank.Services.Implementation.ReportPrograms
{
    [TestClass]
    public class TopAccountAirTests : BaseUnitTest
    {
        [TestMethod]
        public void NoDataReport()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("1200", "INACCT");
            
            InsertReportHandoff();

            var rpt = (TopAccountAir)RunReport();

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

            var rpt = (TopAccountAir)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(10, rpt.FinalDataList.Count ,"Count is incorrect");
            Assert.AreEqual(1295, rpt.FinalDataList.Sum(s => s.Trips), "Total Trips is incorrect");
            Assert.AreEqual(863184.54m, rpt.FinalDataList.Sum(s => s.Amt), "Total Volume Booked is incorrect");
            Assert.AreEqual(2523.69m, rpt.FinalDataList.Sum(s => s.Acommisn), "Total Commission is incorrect");
            Assert.AreEqual(0m, rpt.FinalDataList.Sum(s => s.Svcfee), "Total Service Fee is incorrect");

            Assert.AreEqual(1296, rpt.TotCount, "Report Total Trips is incorrect");
            Assert.AreEqual(863727.87m, rpt.TotCharge, "Report Total Volume Booked is incorrect");
            Assert.AreEqual(2523.69m, rpt.TotCommission, "Report Total Commission is incorrect");
            Assert.AreEqual(0m, rpt.TotSvcFee, "Report Total Service Fee is incorrect");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeDepartureDateOriginLeg()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("YYZ", "ORIGIN");
            ManipulateReportHandoffRecords("1", "RBAPPLYTOLEGORSEG");
            InsertReportHandoff();

            var rpt = (TopAccountAir)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(10, rpt.FinalDataList.Count, "Count is incorrect");
            Assert.AreEqual(828, rpt.FinalDataList.Sum(s => s.Trips), "Total Trips is incorrect");
            Assert.AreEqual(661293.43m, rpt.FinalDataList.Sum(s => s.Amt), "Total Volume Booked is incorrect");
            Assert.AreEqual(1337.76m, rpt.FinalDataList.Sum(s => s.Acommisn), "Total Commission is incorrect");
            Assert.AreEqual(0m, rpt.FinalDataList.Sum(s => s.Svcfee), "Total Service Fee is incorrect");

            Assert.AreEqual(828, rpt.TotCount, "Report Total Trips is incorrect");
            Assert.AreEqual(661293.43m, rpt.TotCharge, "Report Total Volume Booked is incorrect");
            Assert.AreEqual(1337.76m, rpt.TotCommission, "Report Total Commission is incorrect");
            Assert.AreEqual(0m, rpt.TotSvcFee, "Report Total Service Fee is incorrect");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeDepartureDateOriginSeg()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("YYZ", "ORIGIN");
            ManipulateReportHandoffRecords("2", "RBAPPLYTOLEGORSEG");
            InsertReportHandoff();

            var rpt = (TopAccountAir)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(9, rpt.FinalDataList.Count, "Count is incorrect");
            Assert.AreEqual(809, rpt.FinalDataList.Sum(s => s.Trips), "Total Trips is incorrect");
            Assert.AreEqual(647194.18m, rpt.FinalDataList.Sum(s => s.Amt), "Total Volume Booked is incorrect");
            Assert.AreEqual(1226.50m, rpt.FinalDataList.Sum(s => s.Acommisn), "Total Commission is incorrect");
            Assert.AreEqual(0m, rpt.FinalDataList.Sum(s => s.Svcfee), "Total Service Fee is incorrect");

            Assert.AreEqual(809, rpt.TotCount, "Report Total Trips is incorrect");
            Assert.AreEqual(647194.18m, rpt.TotCharge, "Report Total Volume Booked is incorrect");
            Assert.AreEqual(1226.50m, rpt.TotCommission, "Report Total Commission is incorrect");
            Assert.AreEqual(0m, rpt.TotSvcFee, "Report Total Service Fee is incorrect");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeDepartureDateGroupByDatasource()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("3", "GROUPBY");
            InsertReportHandoff();

            var rpt = (TopAccountAir)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(1, rpt.FinalDataList.Count, "Count is incorrect");
            Assert.AreEqual(1296, rpt.FinalDataList.Sum(s => s.Trips), "Total Trips is incorrect");
            Assert.AreEqual(863727.87m, rpt.FinalDataList.Sum(s => s.Amt), "Total Volume Booked is incorrect");
            Assert.AreEqual(2523.69m, rpt.FinalDataList.Sum(s => s.Acommisn), "Total Commission is incorrect");
            Assert.AreEqual(0m, rpt.FinalDataList.Sum(s => s.Svcfee), "Total Service Fee is incorrect");

            Assert.AreEqual(1296, rpt.TotCount, "Report Total Trips is incorrect");
            Assert.AreEqual(863727.87m, rpt.TotCharge, "Report Total Volume Booked is incorrect");
            Assert.AreEqual(2523.69m, rpt.TotCommission, "Report Total Commission is incorrect");
            Assert.AreEqual(0m, rpt.TotSvcFee, "Report Total Service Fee is incorrect");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeDepartureDateLowFares()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("ON", "CBINCLLOWFARELOSTSVGS");
            InsertReportHandoff();

            var rpt = (TopAccountAir)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(10, rpt.FinalDataList.Count, "Count is incorrect");
            Assert.AreEqual(1295, rpt.FinalDataList.Sum(s => s.Trips), "Total Trips is incorrect");
            Assert.AreEqual(863184.54m, rpt.FinalDataList.Sum(s => s.Amt), "Total Volume Booked is incorrect");
            Assert.AreEqual(2523.69m, rpt.FinalDataList.Sum(s => s.Acommisn), "Total Commission is incorrect");
            Assert.AreEqual(0m, rpt.FinalDataList.Sum(s => s.Svcfee), "Total Service Fee is incorrect");
            Assert.AreEqual(852597.88m, rpt.FinalDataList.Sum(s => s.LowFare), "Total Low Fare is incorrect");
            Assert.AreEqual(10586.66m, rpt.FinalDataList.Sum(s => s.LostAmt), "Total Lost Savings is incorrect");

            Assert.AreEqual(1296, rpt.TotCount, "Report Total Trips is incorrect");
            Assert.AreEqual(863727.87m, rpt.TotCharge, "Report Total Volume Booked is incorrect");
            Assert.AreEqual(2523.69m, rpt.TotCommission, "Report Total Commission is incorrect");
            Assert.AreEqual(0m, rpt.TotSvcFee, "Report Total Service Fee is incorrect");
            Assert.AreEqual(10586.66m, rpt.TotLostAmt, "Report Total Lost Amount is incorrect");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeInvoiceDate()
        {
            GenerateReportHandoff(DateType.InvoiceDate);
            InsertReportHandoff();

            var rpt = (TopAccountAir)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(10, rpt.FinalDataList.Count, "Count is incorrect");
            Assert.AreEqual(1352, rpt.FinalDataList.Sum(s => s.Trips), "Total Trips is incorrect");
            Assert.AreEqual(849024.82m, rpt.FinalDataList.Sum(s => s.Amt), "Total Volume Booked is incorrect");
            Assert.AreEqual(2651.21m, rpt.FinalDataList.Sum(s => s.Acommisn), "Total Commission is incorrect");
            Assert.AreEqual(0m, rpt.FinalDataList.Sum(s => s.Svcfee), "Total Service Fee is incorrect");

            Assert.AreEqual(1353, rpt.TotCount, "Report Total Trips is incorrect");
            Assert.AreEqual(849568.15m, rpt.TotCharge, "Report Total Volume Booked is incorrect");
            Assert.AreEqual(2651.21m, rpt.TotCommission, "Report Total Commission is incorrect");
            Assert.AreEqual(0m, rpt.TotSvcFee, "Report Total Service Fee is incorrect");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationDepartureDate()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("1", "PREPOST");
            InsertReportHandoff();

            var rpt = (TopAccountAir)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(10, rpt.FinalDataList.Count, "Count is incorrect");
            Assert.AreEqual(993, rpt.FinalDataList.Sum(s => s.Trips), "Total Trips is incorrect");
            Assert.AreEqual(918787.54m, rpt.FinalDataList.Sum(s => s.Amt), "Total Volume Booked is incorrect");
            
            Assert.AreEqual(2659, rpt.TotCount, "Report Total Trips is incorrect");
            Assert.AreEqual(1952373.22m, rpt.TotCharge, "Report Total Volume Booked is incorrect");
  
            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationInvoiceDate()
        {
            GenerateReportHandoff(DateType.InvoiceDate);
            ManipulateReportHandoffRecords("1","PREPOST");
            InsertReportHandoff();

            var rpt = (TopAccountAir)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(10, rpt.FinalDataList.Count, "Count is incorrect");
            Assert.AreEqual(55, rpt.FinalDataList.Sum(s => s.Trips), "Total Trips is incorrect");
            Assert.AreEqual(210497.46m, rpt.FinalDataList.Sum(s => s.Amt), "Total Volume Booked is incorrect");

            Assert.AreEqual(146, rpt.TotCount, "Report Total Trips is incorrect");
            Assert.AreEqual(259293.08m, rpt.TotCharge, "Report Total Volume Booked is incorrect");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationBookedDate()
        {
            GenerateReportHandoff(DateType.BookedDate);
            ManipulateReportHandoffRecords("1", "PREPOST");
            InsertReportHandoff();

            var rpt = (TopAccountAir)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(1, rpt.FinalDataList.Count, "Count is incorrect");
            Assert.AreEqual(2, rpt.FinalDataList.Sum(s => s.Trips), "Total Trips is incorrect");
            Assert.AreEqual(0m, rpt.FinalDataList.Sum(s => s.Amt), "Total Volume Booked is incorrect");

            Assert.AreEqual(2, rpt.TotCount, "Report Total Trips is incorrect");
            Assert.AreEqual(0m, rpt.TotCharge, "Report Total Volume Booked is incorrect");

            ClearReportHandoff();
        }

        public void GenerateReportHandoff(DateType dateType)
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2015,3,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DDGRPFIELD", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2015,9,30", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "GROUPBY", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "HOWMANY", ParmValue = "10", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "USD", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = "2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "50", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibTopAcctAir", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RBAPPLYTOLEGORSEG", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RBSORTDESCASC", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3393911", ParmInOut = "IN", LangCode = "" });
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
