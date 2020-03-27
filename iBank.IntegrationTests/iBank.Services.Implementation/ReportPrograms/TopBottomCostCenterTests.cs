using System.Linq;
using Domain.Helper;
using iBank.IntegrationTests.TestTools;
using iBank.Services.Implementation.ReportPrograms.TopBottomCostCenter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.IntegrationTests.iBank.Services.Implementation.ReportPrograms
{
    [TestClass]
    public class TopBottomCostCenterTests : BaseUnitTest
    {

        [TestMethod]
        public void NoDataReport()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("1200", "INACCT");

            InsertReportHandoff();

            var rpt = (TopBottomCostCenter)RunReport();

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

            var rpt = (TopBottomCostCenter)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(10, rpt.FinalDataList.Count, "Count is incorrect");
            Assert.AreEqual(1456314.55m, rpt.FinalDataList.Sum(s => s.Airchg), "Total Fair Paid is incorrect");
            Assert.AreEqual(15935.74m, rpt.FinalDataList.Sum(s => s.Lostamt), "Total Lost Amount is incorrect");
            Assert.AreEqual(2338, rpt.FinalDataList.Sum(s => s.Numtrips), "Total # of Air Trips is incorrect");
            Assert.AreEqual(1647, rpt.FinalDataList.Sum(s => s.Nohotel), "Total Trips w/No Hotel is incorrect");
            Assert.AreEqual(979, rpt.FinalDataList.Sum(s => s.Stays), "Total Hotel Stays is incorrect");
            Assert.AreEqual(1837, rpt.FinalDataList.Sum(s => s.Nights), "Total Room Nights is incorrect");
            Assert.AreEqual(169760.73m, rpt.FinalDataList.Sum(s => s.Hotelcost), "Total Hotel Cost is incorrect");
            Assert.AreEqual(629, rpt.FinalDataList.Sum(s => s.Rentals), "Total Car Rentals is incorrect");
            Assert.AreEqual(1590, rpt.FinalDataList.Sum(s => s.Days), "Total Rental Days is incorrect");
            Assert.AreEqual(41184.93m, rpt.FinalDataList.Sum(s => s.Carcost), "Total Car Cost is incorrect");
            Assert.AreEqual(1667260.21m, rpt.FinalDataList.Sum(s => s.Totalcost), "Total Cost is incorrect");

            Assert.AreEqual(1456920.68m, rpt.TotChg, "Report Total Fair Paid is incorrect");
            Assert.AreEqual(15935.74m, rpt.TotLost, "Report Total Lost Amount is incorrect");
            Assert.AreEqual(2340, rpt.TotCnt, "Report Total # of Air Trips is incorrect");
            Assert.AreEqual(1649, rpt.TotCnt2, "Report Total Trips w/No Hotel is incorrect");
            Assert.AreEqual(979, rpt.TotStays, "Report Total Hotel Stays is incorrect");
            Assert.AreEqual(1837, rpt.TotNites, "Report Total Room Nights is incorrect");
            Assert.AreEqual(169760.73m, rpt.TotHotCost, "Report Total Hotel Cost is incorrect");
            Assert.AreEqual(629, rpt.TotRents, "Report Total Car Rentals is incorrect");
            Assert.AreEqual(1590, rpt.TotDays, "Report Total Rental Days is incorrect");
            Assert.AreEqual(41184.93m, rpt.TotCarCost, "Report Total Car Cost is incorrect");
            Assert.AreEqual(1667866.34m, rpt.TotCost, "Report Total Cost is incorrect");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeDepartureDateBreak2Limit8()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("2", "GROUPBY");
            ManipulateReportHandoffRecords("8", "HOWMANY");
            InsertReportHandoff();

            var rpt = (TopBottomCostCenter)RunReport();
           
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(8, rpt.FinalDataList.Count, "Count is incorrect");
            Assert.AreEqual(1325944.47m, rpt.FinalDataList.Sum(s => s.Airchg), "Total Fair Paid is incorrect");
            Assert.AreEqual(14123.22m, rpt.FinalDataList.Sum(s => s.Lostamt), "Total Lost Amount is incorrect");
            Assert.AreEqual(2116, rpt.FinalDataList.Sum(s => s.Numtrips), "Total # of Air Trips is incorrect");
            Assert.AreEqual(1494, rpt.FinalDataList.Sum(s => s.Nohotel), "Total Trips w/No Hotel is incorrect");
            Assert.AreEqual(897, rpt.FinalDataList.Sum(s => s.Stays), "Total Hotel Stays is incorrect");
            Assert.AreEqual(1639, rpt.FinalDataList.Sum(s => s.Nights), "Total Room Nights is incorrect");
            Assert.AreEqual(140762.37m, rpt.FinalDataList.Sum(s => s.Hotelcost), "Total Hotel Cost is incorrect");
            Assert.AreEqual(582, rpt.FinalDataList.Sum(s => s.Rentals), "Total Car Rentals is incorrect");
            Assert.AreEqual(1445, rpt.FinalDataList.Sum(s => s.Days), "Total Rental Days is incorrect");
            Assert.AreEqual(34841.46m, rpt.FinalDataList.Sum(s => s.Carcost), "Total Car Cost is incorrect");
            Assert.AreEqual(1501548.30m, rpt.FinalDataList.Sum(s => s.Totalcost), "Total Cost is incorrect");

            Assert.AreEqual(1456920.68m, rpt.TotChg, "Report Total Fair Paid is incorrect");
            Assert.AreEqual(15935.74m, rpt.TotLost, "Report Total Lost Amount is incorrect");
            Assert.AreEqual(2340, rpt.TotCnt, "Report Total # of Air Trips is incorrect");
            Assert.AreEqual(1649, rpt.TotCnt2, "Report Total Trips w/No Hotel is incorrect");
            Assert.AreEqual(979, rpt.TotStays, "Report Total Hotel Stays is incorrect");
            Assert.AreEqual(1837, rpt.TotNites, "Report Total Room Nights is incorrect");
            Assert.AreEqual(169760.73m, rpt.TotHotCost, "Report Total Hotel Cost is incorrect");
            Assert.AreEqual(629, rpt.TotRents, "Report Total Car Rentals is incorrect");
            Assert.AreEqual(1590, rpt.TotDays, "Report Total Rental Days is incorrect");
            Assert.AreEqual(41184.93m, rpt.TotCarCost, "Report Total Car Cost is incorrect");
            Assert.AreEqual(1667866.34m, rpt.TotCost, "Report Total Cost is incorrect");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeDepartureDateBreak3Limit8()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("3", "GROUPBY");
            ManipulateReportHandoffRecords("8", "HOWMANY");
            InsertReportHandoff();

            var rpt = (TopBottomCostCenter)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(8, rpt.FinalDataList.Count, "Count is incorrect");
            Assert.AreEqual(1456592.63m, rpt.FinalDataList.Sum(s => s.Airchg), "Total Fair Paid is incorrect");
            Assert.AreEqual(15935.74m, rpt.FinalDataList.Sum(s => s.Lostamt), "Total Lost Amount is incorrect");
            Assert.AreEqual(2339, rpt.FinalDataList.Sum(s => s.Numtrips), "Total # of Air Trips is incorrect");
            Assert.AreEqual(1648, rpt.FinalDataList.Sum(s => s.Nohotel), "Total Trips w/No Hotel is incorrect");
            Assert.AreEqual(979, rpt.FinalDataList.Sum(s => s.Stays), "Total Hotel Stays is incorrect");
            Assert.AreEqual(1837, rpt.FinalDataList.Sum(s => s.Nights), "Total Room Nights is incorrect");
            Assert.AreEqual(169760.73m, rpt.FinalDataList.Sum(s => s.Hotelcost), "Total Hotel Cost is incorrect");
            Assert.AreEqual(629, rpt.FinalDataList.Sum(s => s.Rentals), "Total Car Rentals is incorrect");
            Assert.AreEqual(1590, rpt.FinalDataList.Sum(s => s.Days), "Total Rental Days is incorrect");
            Assert.AreEqual(41184.93m, rpt.FinalDataList.Sum(s => s.Carcost), "Total Car Cost is incorrect");
            Assert.AreEqual(1667538.29m, rpt.FinalDataList.Sum(s => s.Totalcost), "Total Cost is incorrect");

            Assert.AreEqual(1456920.68m, rpt.TotChg, "Report Total Fair Paid is incorrect");
            Assert.AreEqual(15935.74m, rpt.TotLost, "Report Total Lost Amount is incorrect");
            Assert.AreEqual(2340, rpt.TotCnt, "Report Total # of Air Trips is incorrect");
            Assert.AreEqual(1649, rpt.TotCnt2, "Report Total Trips w/No Hotel is incorrect");
            Assert.AreEqual(979, rpt.TotStays, "Report Total Hotel Stays is incorrect");
            Assert.AreEqual(1837, rpt.TotNites, "Report Total Room Nights is incorrect");
            Assert.AreEqual(169760.73m, rpt.TotHotCost, "Report Total Hotel Cost is incorrect");
            Assert.AreEqual(629, rpt.TotRents, "Report Total Car Rentals is incorrect");
            Assert.AreEqual(1590, rpt.TotDays, "Report Total Rental Days is incorrect");
            Assert.AreEqual(41184.93m, rpt.TotCarCost, "Report Total Car Cost is incorrect");
            Assert.AreEqual(1667866.34m, rpt.TotCost, "Report Total Cost is incorrect");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeInvoiceDate()
        {
            GenerateReportHandoff(DateType.InvoiceDate);
            InsertReportHandoff();

            var rpt = (TopBottomCostCenter)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(10, rpt.FinalDataList.Count, "Count is incorrect");
            Assert.AreEqual(1449956.09m, rpt.FinalDataList.Sum(s => s.Airchg), "Total Fair Paid is incorrect");
            Assert.AreEqual(16832.94m, rpt.FinalDataList.Sum(s => s.Lostamt), "Total Lost Amount is incorrect");
            Assert.AreEqual(2398, rpt.FinalDataList.Sum(s => s.Numtrips), "Total # of Air Trips is incorrect");
            Assert.AreEqual(1681, rpt.FinalDataList.Sum(s => s.Nohotel), "Total Trips w/No Hotel is incorrect");
            Assert.AreEqual(1020, rpt.FinalDataList.Sum(s => s.Stays), "Total Hotel Stays is incorrect");
            Assert.AreEqual(1903, rpt.FinalDataList.Sum(s => s.Nights), "Total Room Nights is incorrect");
            Assert.AreEqual(171788.67m, rpt.FinalDataList.Sum(s => s.Hotelcost), "Total Hotel Cost is incorrect");
            Assert.AreEqual(640, rpt.FinalDataList.Sum(s => s.Rentals), "Total Car Rentals is incorrect");
            Assert.AreEqual(1627, rpt.FinalDataList.Sum(s => s.Days), "Total Rental Days is incorrect");
            Assert.AreEqual(41549.02m, rpt.FinalDataList.Sum(s => s.Carcost), "Total Car Cost is incorrect");
            Assert.AreEqual(1663293.78m, rpt.FinalDataList.Sum(s => s.Totalcost), "Total Cost is incorrect");

            Assert.AreEqual(1450562.22m, rpt.TotChg, "Report Total Fair Paid is incorrect");
            Assert.AreEqual(16832.94m, rpt.TotLost, "Report Total Lost Amount is incorrect");
            Assert.AreEqual(2400, rpt.TotCnt, "Report Total # of Air Trips is incorrect");
            Assert.AreEqual(1683, rpt.TotCnt2, "Report Total Trips w/No Hotel is incorrect");
            Assert.AreEqual(1020, rpt.TotStays, "Report Total Hotel Stays is incorrect");
            Assert.AreEqual(1903, rpt.TotNites, "Report Total Room Nights is incorrect");
            Assert.AreEqual(171788.67m, rpt.TotHotCost, "Report Total Hotel Cost is incorrect");
            Assert.AreEqual(640, rpt.TotRents, "Report Total Car Rentals is incorrect");
            Assert.AreEqual(1627, rpt.TotDays, "Report Total Rental Days is incorrect");
            Assert.AreEqual(41549.02m, rpt.TotCarCost, "Report Total Car Cost is incorrect");
            Assert.AreEqual(1663899.91m, rpt.TotCost, "Report Total Cost is incorrect");

            ClearReportHandoff();
        }
      
        [TestMethod]
        public void ReservationInvoiceDate()
        {
            GenerateReportHandoff(DateType.InvoiceDate);
            ManipulateReportHandoffRecords("1","PREPOST");
            ManipulateReportHandoffRecords("DT:2015,1,5", "ENDDATE");
            InsertReportHandoff();

            var rpt = (TopBottomCostCenter)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(10, rpt.FinalDataList.Count, "Count is incorrect");
            Assert.AreEqual(443040.21m, rpt.FinalDataList.Sum(s => s.Airchg), "Total Fair Paid is incorrect");
            Assert.AreEqual(37325.49m, rpt.FinalDataList.Sum(s => s.Lostamt), "Total Lost Amount is incorrect");
            Assert.AreEqual(880, rpt.FinalDataList.Sum(s => s.Numtrips), "Total # of Air Trips is incorrect");
            Assert.AreEqual(605, rpt.FinalDataList.Sum(s => s.Nohotel), "Total Trips w/No Hotel is incorrect");
            Assert.AreEqual(687, rpt.FinalDataList.Sum(s => s.Stays), "Total Hotel Stays is incorrect");
            Assert.AreEqual(1652, rpt.FinalDataList.Sum(s => s.Nights), "Total Room Nights is incorrect");
            Assert.AreEqual(202900.94m, rpt.FinalDataList.Sum(s => s.Hotelcost), "Total Hotel Cost is incorrect");
            Assert.AreEqual(459, rpt.FinalDataList.Sum(s => s.Rentals), "Total Car Rentals is incorrect");
            Assert.AreEqual(1400, rpt.FinalDataList.Sum(s => s.Days), "Total Rental Days is incorrect");
            Assert.AreEqual(98685.12m, rpt.FinalDataList.Sum(s => s.Carcost), "Total Car Cost is incorrect");
            Assert.AreEqual(744626.27m, rpt.FinalDataList.Sum(s => s.Totalcost), "Total Cost is incorrect");

            Assert.AreEqual(551596.82m, rpt.TotChg, "Report Total Fair Paid is incorrect");
            Assert.AreEqual(52254.97m, rpt.TotLost, "Report Total Lost Amount is incorrect");
            Assert.AreEqual(1096, rpt.TotCnt, "Report Total # of Air Trips is incorrect");
            Assert.AreEqual(775, rpt.TotCnt2, "Report Total Trips w/No Hotel is incorrect");
            Assert.AreEqual(770, rpt.TotStays, "Report Total Hotel Stays is incorrect");
            Assert.AreEqual(1811, rpt.TotNites, "Report Total Room Nights is incorrect");
            Assert.AreEqual(228157.24m, rpt.TotHotCost, "Report Total Hotel Cost is incorrect");
            Assert.AreEqual(507, rpt.TotRents, "Report Total Car Rentals is incorrect");
            Assert.AreEqual(1527, rpt.TotDays, "Report Total Rental Days is incorrect");
            Assert.AreEqual(104808.49m, rpt.TotCarCost, "Report Total Car Cost is incorrect");
            Assert.AreEqual(884562.55m, rpt.TotCost, "Report Total Cost is incorrect");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationDepartureDate()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("1", "PREPOST");
            ManipulateReportHandoffRecords("DT:2015,1,5", "ENDDATE");
            InsertReportHandoff();

            var rpt = (TopBottomCostCenter)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(10, rpt.FinalDataList.Count, "Count is incorrect");
            Assert.AreEqual(45864.37m, rpt.FinalDataList.Sum(s => s.Airchg), "Total Fair Paid is incorrect");
            Assert.AreEqual(6033.07m, rpt.FinalDataList.Sum(s => s.Lostamt), "Total Lost Amount is incorrect");
            Assert.AreEqual(70, rpt.FinalDataList.Sum(s => s.Numtrips), "Total # of Air Trips is incorrect");
            Assert.AreEqual(33, rpt.FinalDataList.Sum(s => s.Nohotel), "Total Trips w/No Hotel is incorrect");
            Assert.AreEqual(151, rpt.FinalDataList.Sum(s => s.Stays), "Total Hotel Stays is incorrect");
            Assert.AreEqual(422, rpt.FinalDataList.Sum(s => s.Nights), "Total Room Nights is incorrect");
            Assert.AreEqual(55026.62m, rpt.FinalDataList.Sum(s => s.Hotelcost), "Total Hotel Cost is incorrect");
            Assert.AreEqual(94, rpt.FinalDataList.Sum(s => s.Rentals), "Total Car Rentals is incorrect");
            Assert.AreEqual(337, rpt.FinalDataList.Sum(s => s.Days), "Total Rental Days is incorrect");
            Assert.AreEqual(51943.11m, rpt.FinalDataList.Sum(s => s.Carcost), "Total Car Cost is incorrect");
            Assert.AreEqual(152834.10m, rpt.FinalDataList.Sum(s => s.Totalcost), "Total Cost is incorrect");

            Assert.AreEqual(54994.42m, rpt.TotChg, "Report Total Fair Paid is incorrect");
            Assert.AreEqual(7872.63m, rpt.TotLost, "Report Total Lost Amount is incorrect");
            Assert.AreEqual(84, rpt.TotCnt, "Report Total # of Air Trips is incorrect");
            Assert.AreEqual(45, rpt.TotCnt2, "Report Total Trips w/No Hotel is incorrect");
            Assert.AreEqual(159, rpt.TotStays, "Report Total Hotel Stays is incorrect");
            Assert.AreEqual(433, rpt.TotNites, "Report Total Room Nights is incorrect");
            Assert.AreEqual(56344.57m, rpt.TotHotCost, "Report Total Hotel Cost is incorrect");
            Assert.AreEqual(98, rpt.TotRents, "Report Total Car Rentals is incorrect");
            Assert.AreEqual(355, rpt.TotDays, "Report Total Rental Days is incorrect");
            Assert.AreEqual(52655.37m, rpt.TotCarCost, "Report Total Car Cost is incorrect");
            Assert.AreEqual(163994.36m, rpt.TotCost, "Report Total Cost is incorrect");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationBookedDate()
        {
            GenerateReportHandoff(DateType.BookedDate);
            ManipulateReportHandoffRecords("1", "PREPOST");
            ManipulateReportHandoffRecords("DT:2015,1,5", "ENDDATE");
            InsertReportHandoff();

            var rpt = (TopBottomCostCenter)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            Assert.AreEqual(10, rpt.FinalDataList.Count, "Count is incorrect");
            Assert.AreEqual(791612.18m, rpt.FinalDataList.Sum(s => s.Airchg), "Total Fair Paid is incorrect");
            Assert.AreEqual(139239.07m, rpt.FinalDataList.Sum(s => s.Lostamt), "Total Lost Amount is incorrect");
            Assert.AreEqual(1202, rpt.FinalDataList.Sum(s => s.Numtrips), "Total # of Air Trips is incorrect");
            Assert.AreEqual(792, rpt.FinalDataList.Sum(s => s.Nohotel), "Total Trips w/No Hotel is incorrect");
            Assert.AreEqual(903, rpt.FinalDataList.Sum(s => s.Stays), "Total Hotel Stays is incorrect");
            Assert.AreEqual(2112, rpt.FinalDataList.Sum(s => s.Nights), "Total Room Nights is incorrect");
            Assert.AreEqual(261217.70m, rpt.FinalDataList.Sum(s => s.Hotelcost), "Total Hotel Cost is incorrect");
            Assert.AreEqual(571, rpt.FinalDataList.Sum(s => s.Rentals), "Total Car Rentals is incorrect");
            Assert.AreEqual(1728, rpt.FinalDataList.Sum(s => s.Days), "Total Rental Days is incorrect");
            Assert.AreEqual(156196.99m, rpt.FinalDataList.Sum(s => s.Carcost), "Total Car Cost is incorrect");
            Assert.AreEqual(1209026.87m, rpt.FinalDataList.Sum(s => s.Totalcost), "Total Cost is incorrect");

            Assert.AreEqual(996793.53m, rpt.TotChg, "Report Total Fair Paid is incorrect");
            Assert.AreEqual(167414.89m, rpt.TotLost, "Report Total Lost Amount is incorrect");
            Assert.AreEqual(1512, rpt.TotCnt, "Report Total # of Air Trips is incorrect");
            Assert.AreEqual(1029, rpt.TotCnt2, "Report Total Trips w/No Hotel is incorrect");
            Assert.AreEqual(1032, rpt.TotStays, "Report Total Hotel Stays is incorrect");
            Assert.AreEqual(2403, rpt.TotNites, "Report Total Room Nights is incorrect");
            Assert.AreEqual(306044.56m, rpt.TotHotCost, "Report Total Hotel Cost is incorrect");
            Assert.AreEqual(636, rpt.TotRents, "Report Total Car Rentals is incorrect");
            Assert.AreEqual(1945, rpt.TotDays, "Report Total Rental Days is incorrect");
            Assert.AreEqual(165988.85m, rpt.TotCarCost, "Report Total Car Cost is incorrect");
            Assert.AreEqual(1468826.94m, rpt.TotCost, "Report Total Cost is incorrect");

            ClearReportHandoff();
        }

        public void GenerateReportHandoff(DateType dateType)
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2015,1,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2015,12,31", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "GROUPBY", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "HOWMANY", ParmValue = "10", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "USD", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = "2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "53", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibTopCostCtr", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RBAPPLYTOLEGORSEG", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RBSORTDESCASC", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3394261", ParmInOut = "IN", LangCode = "" });
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
