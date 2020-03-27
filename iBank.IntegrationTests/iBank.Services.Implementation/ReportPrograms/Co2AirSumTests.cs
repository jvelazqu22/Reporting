using System;
using System.Linq;
using Domain.Helper;
using iBank.IntegrationTests.TestTools;
using iBank.Services.Implementation.ReportPrograms.CO2AirSummary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.IntegrationTests.iBank.Services.Implementation.ReportPrograms
{
    [TestClass]
    public class Co2AirSumTests : BaseUnitTest
    {

        [TestMethod]
        public void TooMuchDataReport()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            InsertReportHandoff();

            var rpt = (Co2AirSum)RunReport();
            
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(2, rptInfo.ReturnCode, "Return code failed.");
            var containsNoDataMsg = rptInfo.ErrorMessage.Contains(ReportGlobals.ReportMessages.RptMsg_BigVolume);
            Assert.AreEqual(true, containsNoDataMsg, "Error message failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeDepartureDate()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("2600,8999855,VENDOR,9000933,9000521","INACCT");
            InsertReportHandoff();

            var rpt = (Co2AirSum)RunReport();
      
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            var airSummary = rpt.AirSummary.FirstOrDefault();
            Assert.AreEqual(142, airSummary.Invoices, "Incorrect number of Invoices");
            Assert.AreEqual(0, airSummary.Credits, "Incorrect number of Credits");
            Assert.AreEqual(142, airSummary.Trips, "Incorrect number of Trips");
            Assert.AreEqual(66515.36m, airSummary.Airchg, "Incorrect Air Charges");
            Assert.AreEqual(468.42m, Math.Round(airSummary.Avgairchg, 2), "Incorrect Average Air Charges");
            Assert.AreEqual(118134, airSummary.Airco2, "Incorrect Air CO2");
            Assert.AreEqual(35517, airSummary.Altrailco2, "Incorrect Rail CO2");
            Assert.AreEqual(130762, airSummary.Altcarco2, "Incorrect Car CO2");

            Assert.AreEqual(315, rpt.TopGroup.Sum(s => s.Segs), "Incorrect Top Group # of segments");
            Assert.AreEqual(275654, rpt.TopGroup.Sum(s => s.Miles), "Incorrect Top Group Miles");
            Assert.AreEqual(118132, rpt.TopGroup.Sum(s => s.Airco2), "Incorrect Top Group Air CO2");

            Assert.AreEqual(80, rpt.CityPair.Sum(s => s.Segments), "Incorrect City Pair # of segments");
            Assert.AreEqual(107295, rpt.CityPair.Sum(s => s.Miles), "Incorrect City Pair Miles");
            Assert.AreEqual(47553, rpt.CityPair.Sum(s => s.Airco2), "Incorrect City Pair Air CO2");

            Assert.AreEqual(294, rpt.TopCarriers.Sum(s => s.Segs), "Incorrect Top Carriers # of segments");
            Assert.AreEqual(267059, rpt.TopCarriers.Sum(s => s.Miles), "Incorrect Top Carriers Miles");
            Assert.AreEqual(113300, rpt.TopCarriers.Sum(s => s.Airco2), "Incorrect Top Carriers Air CO2");

            Assert.AreEqual(2, rpt.ServiceClass.Sum(s => s.Fclssegs), "Incorrect First Class # of segments");
            Assert.AreEqual(678, rpt.ServiceClass.Sum(s => s.Fclsmiles), "Incorrect First Class Miles");
            Assert.AreEqual(415, rpt.ServiceClass.Sum(s => s.Fclsco2), "Incorrect First Class Air CO2");
            Assert.AreEqual(4, rpt.ServiceClass.Sum(s => s.Bclssegs), "Incorrect Business Class # of segments");
            Assert.AreEqual(8404, rpt.ServiceClass.Sum(s => s.Bclsmiles), "Incorrect Business Class Miles");
            Assert.AreEqual(4337, rpt.ServiceClass.Sum(s => s.Bclsco2), "Incorrect Business Class Air CO2");
            Assert.AreEqual(309, rpt.ServiceClass.Sum(s => s.Eclssegs), "Incorrect Economy Class # of segments");
            Assert.AreEqual(266572, rpt.ServiceClass.Sum(s => s.Eclsmiles), "Incorrect Economy Class Miles");
            Assert.AreEqual(113381, rpt.ServiceClass.Sum(s => s.Eclsco2), "Incorrect Economy Class Air CO2");

            Assert.AreEqual(7, rpt.ServiceClass.Sum(s => s.Lhaulsegs), "Incorrect Long Haul # of segments");
            Assert.AreEqual(21761, rpt.ServiceClass.Sum(s => s.Lhaulmiles), "Incorrect Long Haul Miles");
            Assert.AreEqual(13777, rpt.ServiceClass.Sum(s => s.Lhaulco2), "Incorrect Long Haul Air CO2");
            Assert.AreEqual(153, rpt.ServiceClass.Sum(s => s.Mhaulsegs), "Incorrect Medium Haul # of segments");
            Assert.AreEqual(200300, rpt.ServiceClass.Sum(s => s.Mhaulmiles), "Incorrect Medium Haul Miles");
            Assert.AreEqual(71518, rpt.ServiceClass.Sum(s => s.Mhaulco2), "Incorrect Medium Haul Air CO2");
            Assert.AreEqual(155, rpt.ServiceClass.Sum(s => s.Shaulsegs), "Incorrect Short Haul # of segments");
            Assert.AreEqual(53593, rpt.ServiceClass.Sum(s => s.Shaulmiles), "Incorrect Short Haul Miles");
            Assert.AreEqual(32838, rpt.ServiceClass.Sum(s => s.Shaulco2), "Incorrect Short Haul Air CO2");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeInvoiceDate()
        {
            GenerateReportHandoff(DateType.InvoiceDate);
            ManipulateReportHandoffRecords("2600,8999855,VENDOR,9000933,9000521", "INACCT");
            InsertReportHandoff();

            var rpt = (Co2AirSum)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            var airSummary = rpt.AirSummary.FirstOrDefault();
            Assert.AreEqual(148, airSummary.Invoices, "Incorrect number of Invoices");
            Assert.AreEqual(0, airSummary.Credits, "Incorrect number of Credits");
            Assert.AreEqual(148, airSummary.Trips, "Incorrect number of Trips");
            Assert.AreEqual(96830.87m, airSummary.Airchg, "Incorrect Air Charges");
            Assert.AreEqual(654.26m, Math.Round(airSummary.Avgairchg, 2), "Incorrect Average Air Charges");
            Assert.AreEqual(163077, airSummary.Airco2, "Incorrect Air CO2");
            Assert.AreEqual(39551, airSummary.Altrailco2, "Incorrect Rail CO2");
            Assert.AreEqual(162789, airSummary.Altcarco2, "Incorrect Car CO2");

            Assert.AreEqual(318, rpt.TopGroup.Sum(s => s.Segs), "Incorrect Top Group # of segments");
            Assert.AreEqual(343166, rpt.TopGroup.Sum(s => s.Miles), "Incorrect Top Group Miles");
            Assert.AreEqual(163075, rpt.TopGroup.Sum(s => s.Airco2), "Incorrect Top Group Air CO2");

            //TODO: This will be incorrect until we resolve the carbon calc/collapse stuff
            //Assert.AreEqual(89, rpt.CityPair.Sum(s => s.Segments), "Incorrect City Pair # of segments");
            //Assert.AreEqual(156580, rpt.CityPair.Sum(s => s.Miles), "Incorrect City Pair Miles");
            //Assert.AreEqual(76024, rpt.CityPair.Sum(s => s.Airco2), "Incorrect City Pair Air CO2");

            Assert.AreEqual(299, rpt.TopCarriers.Sum(s => s.Segs), "Incorrect Top Carriers # of segments");
            Assert.AreEqual(333838, rpt.TopCarriers.Sum(s => s.Miles), "Incorrect Top Carriers Miles");
            Assert.AreEqual(157872, rpt.TopCarriers.Sum(s => s.Airco2), "Incorrect Top Carriers Air CO2");

            Assert.AreEqual(5, rpt.ServiceClass.Sum(s => s.Fclssegs), "Incorrect First Class # of segments");
            Assert.AreEqual(9502, rpt.ServiceClass.Sum(s => s.Fclsmiles), "Incorrect First Class Miles");
            Assert.AreEqual(8112, rpt.ServiceClass.Sum(s => s.Fclsco2), "Incorrect First Class Air CO2");
            Assert.AreEqual(20, rpt.ServiceClass.Sum(s => s.Bclssegs), "Incorrect Business Class # of segments");
            Assert.AreEqual(44117, rpt.ServiceClass.Sum(s => s.Bclsmiles), "Incorrect Business Class Miles");
            Assert.AreEqual(36145, rpt.ServiceClass.Sum(s => s.Bclsco2), "Incorrect Business Class Air CO2");
            Assert.AreEqual(293, rpt.ServiceClass.Sum(s => s.Eclssegs), "Incorrect Economy Class # of segments");
            Assert.AreEqual(289547, rpt.ServiceClass.Sum(s => s.Eclsmiles), "Incorrect Economy Class Miles");
            Assert.AreEqual(118820, rpt.ServiceClass.Sum(s => s.Eclsco2), "Incorrect Economy Class Air CO2");

            Assert.AreEqual(15, rpt.ServiceClass.Sum(s => s.Lhaulsegs), "Incorrect Long Haul # of segments");
            Assert.AreEqual(53487, rpt.ServiceClass.Sum(s => s.Lhaulmiles), "Incorrect Long Haul Miles");
            Assert.AreEqual(47844, rpt.ServiceClass.Sum(s => s.Lhaulco2), "Incorrect Long Haul Air CO2");
            Assert.AreEqual(176, rpt.ServiceClass.Sum(s => s.Mhaulsegs), "Incorrect Medium Haul # of segments");
            Assert.AreEqual(247121, rpt.ServiceClass.Sum(s => s.Mhaulmiles), "Incorrect Medium Haul Miles");
            Assert.AreEqual(89156, rpt.ServiceClass.Sum(s => s.Mhaulco2), "Incorrect Medium Haul Air CO2");
            Assert.AreEqual(127, rpt.ServiceClass.Sum(s => s.Shaulsegs), "Incorrect Short Haul # of segments");
            Assert.AreEqual(42558, rpt.ServiceClass.Sum(s => s.Shaulmiles), "Incorrect Short Haul Miles");
            Assert.AreEqual(26076, rpt.ServiceClass.Sum(s => s.Shaulco2), "Incorrect Short Haul Air CO2");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationDepartureDate()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("1200,1600,30279,1188,401025", "INACCT");
            ManipulateReportHandoffRecords("1", "PREPOST");
            ManipulateReportHandoffRecords("DT:2015,1,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,1,20", "ENDDATE");
            InsertReportHandoff();

            var rpt = (Co2AirSum)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            var airSummary = rpt.AirSummary.FirstOrDefault();
            Assert.AreEqual(540, airSummary.Invoices, "Incorrect number of Invoices");
            Assert.AreEqual(0, airSummary.Credits, "Incorrect number of Credits");
            Assert.AreEqual(540, airSummary.Trips, "Incorrect number of Trips");
            Assert.AreEqual(463664.15m, airSummary.Airchg, "Incorrect Air Charges");
            Assert.AreEqual(858.64m, Math.Round(airSummary.Avgairchg, 2), "Incorrect Average Air Charges");
            Assert.AreEqual(688568, airSummary.Airco2, "Incorrect Air CO2");
            Assert.AreEqual(197251, airSummary.Altrailco2, "Incorrect Rail CO2");
            Assert.AreEqual(619457, airSummary.Altcarco2, "Incorrect Car CO2");

            Assert.AreEqual(1473, rpt.TopGroup.Sum(s => s.Segs), "Incorrect Top Group # of segments");
            Assert.AreEqual(1305861, rpt.TopGroup.Sum(s => s.Miles), "Incorrect Top Group Miles");
            Assert.AreEqual(688566, rpt.TopGroup.Sum(s => s.Airco2), "Incorrect Top Group Air CO2");

            //TODO: This will be incorrect until we resolve the carbon calc/collapse stuff
            //Assert.AreEqual(19, rpt.CityPair.Sum(s => s.Segments), "Incorrect City Pair # of segments");
            //Assert.AreEqual(98988, rpt.CityPair.Sum(s => s.Miles), "Incorrect City Pair Miles");
            //Assert.AreEqual(91534, rpt.CityPair.Sum(s => s.Airco2), "Incorrect City Pair Air CO2");

            Assert.AreEqual(1265, rpt.TopCarriers.Sum(s => s.Segs), "Incorrect Top Carriers # of segments");
            Assert.AreEqual(1004487, rpt.TopCarriers.Sum(s => s.Miles), "Incorrect Top Carriers Miles");
            Assert.AreEqual(531864, rpt.TopCarriers.Sum(s => s.Airco2), "Incorrect Top Carriers Air CO2");

            Assert.AreEqual(40, rpt.ServiceClass.Sum(s => s.Fclssegs), "Incorrect First Class # of segments");
            Assert.AreEqual(73600, rpt.ServiceClass.Sum(s => s.Fclsmiles), "Incorrect First Class Miles");
            Assert.AreEqual(75603, rpt.ServiceClass.Sum(s => s.Fclsco2), "Incorrect First Class Air CO2");
            Assert.AreEqual(44, rpt.ServiceClass.Sum(s => s.Bclssegs), "Incorrect Business Class # of segments");
            Assert.AreEqual(155623, rpt.ServiceClass.Sum(s => s.Bclsmiles), "Incorrect Business Class Miles");
            Assert.AreEqual(137971, rpt.ServiceClass.Sum(s => s.Bclsco2), "Incorrect Business Class Air CO2");
            Assert.AreEqual(1389, rpt.ServiceClass.Sum(s => s.Eclssegs), "Incorrect Economy Class # of segments");
            Assert.AreEqual(1076638, rpt.ServiceClass.Sum(s => s.Eclsmiles), "Incorrect Economy Class Miles");
            Assert.AreEqual(474992, rpt.ServiceClass.Sum(s => s.Eclsco2), "Incorrect Economy Class Air CO2");

            Assert.AreEqual(113, rpt.ServiceClass.Sum(s => s.Lhaulsegs), "Incorrect Long Haul # of segments");
            Assert.AreEqual(489302, rpt.ServiceClass.Sum(s => s.Lhaulmiles), "Incorrect Long Haul Miles");
            Assert.AreEqual(328218, rpt.ServiceClass.Sum(s => s.Lhaulco2), "Incorrect Long Haul Air CO2");
            Assert.AreEqual(565, rpt.ServiceClass.Sum(s => s.Mhaulsegs), "Incorrect Medium Haul # of segments");
            Assert.AreEqual(551703, rpt.ServiceClass.Sum(s => s.Mhaulmiles), "Incorrect Medium Haul Miles");
            Assert.AreEqual(198069, rpt.ServiceClass.Sum(s => s.Mhaulco2), "Incorrect Medium Haul Air CO2");
            Assert.AreEqual(795, rpt.ServiceClass.Sum(s => s.Shaulsegs), "Incorrect Short Haul # of segments");
            Assert.AreEqual(264856, rpt.ServiceClass.Sum(s => s.Shaulmiles), "Incorrect Short Haul Miles");
            Assert.AreEqual(162279, rpt.ServiceClass.Sum(s => s.Shaulco2), "Incorrect Short Haul Air CO2");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationInvoiceDate()
        {
            GenerateReportHandoff(DateType.InvoiceDate);
            ManipulateReportHandoffRecords("1200,1600,30279,1188,401025", "INACCT");
            ManipulateReportHandoffRecords("1", "PREPOST");
            ManipulateReportHandoffRecords("DT:2015,1,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,1,20", "ENDDATE");
            InsertReportHandoff();

            var rpt = (Co2AirSum)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            var airSummary = rpt.AirSummary.FirstOrDefault();
            Assert.AreEqual(1373, airSummary.Invoices, "Incorrect number of Invoices");
            Assert.AreEqual(0, airSummary.Credits, "Incorrect number of Credits");
            Assert.AreEqual(1373, airSummary.Trips, "Incorrect number of Trips");
            Assert.AreEqual(1094698.34m, airSummary.Airchg, "Incorrect Air Charges");
            Assert.AreEqual(797.30m, Math.Round(airSummary.Avgairchg, 2), "Incorrect Average Air Charges");
            Assert.AreEqual(1839480, airSummary.Airco2, "Incorrect Air CO2");
            Assert.AreEqual(542272, airSummary.Altrailco2, "Incorrect Rail CO2");
            Assert.AreEqual(1656989, airSummary.Altcarco2, "Incorrect Car CO2");

            Assert.AreEqual(3627, rpt.TopGroup.Sum(s => s.Segs), "Incorrect Top Group # of segments");
            Assert.AreEqual(3493053, rpt.TopGroup.Sum(s => s.Miles), "Incorrect Top Group Miles");
            Assert.AreEqual(1839477, rpt.TopGroup.Sum(s => s.Airco2), "Incorrect Top Group Air CO2");

            //TODO: This will be incorrect until we resolve the carbon calc/collapse stuff
            Assert.AreEqual(68, rpt.CityPair.Sum(s => s.Segments), "Incorrect City Pair # of segments");
            Assert.AreEqual(255328, rpt.CityPair.Sum(s => s.Miles), "Incorrect City Pair Miles");
            Assert.AreEqual(206884, rpt.CityPair.Sum(s => s.Airco2), "Incorrect City Pair Air CO2");

            Assert.AreEqual(3286, rpt.TopCarriers.Sum(s => s.Segs), "Incorrect Top Carriers # of segments");
            Assert.AreEqual(3027065, rpt.TopCarriers.Sum(s => s.Miles), "Incorrect Top Carriers Miles");
            Assert.AreEqual(1568290, rpt.TopCarriers.Sum(s => s.Airco2), "Incorrect Top Carriers Air CO2");

            Assert.AreEqual(135, rpt.ServiceClass.Sum(s => s.Fclssegs), "Incorrect First Class # of segments");
            Assert.AreEqual(207626, rpt.ServiceClass.Sum(s => s.Fclsmiles), "Incorrect First Class Miles");
            Assert.AreEqual(187528, rpt.ServiceClass.Sum(s => s.Fclsco2), "Incorrect First Class Air CO2");
            Assert.AreEqual(124, rpt.ServiceClass.Sum(s => s.Bclssegs), "Incorrect Business Class # of segments");
            Assert.AreEqual(438896, rpt.ServiceClass.Sum(s => s.Bclsmiles), "Incorrect Business Class Miles");
            Assert.AreEqual(384570, rpt.ServiceClass.Sum(s => s.Bclsco2), "Incorrect Business Class Air CO2");
            Assert.AreEqual(3368, rpt.ServiceClass.Sum(s => s.Eclssegs), "Incorrect Economy Class # of segments");
            Assert.AreEqual(2846531, rpt.ServiceClass.Sum(s => s.Eclsmiles), "Incorrect Economy Class Miles");
            Assert.AreEqual(1267380, rpt.ServiceClass.Sum(s => s.Eclsco2), "Incorrect Economy Class Air CO2");

            Assert.AreEqual(315, rpt.ServiceClass.Sum(s => s.Lhaulsegs), "Incorrect Long Haul # of segments");
            Assert.AreEqual(1271416, rpt.ServiceClass.Sum(s => s.Lhaulmiles), "Incorrect Long Haul Miles");
            Assert.AreEqual(873504, rpt.ServiceClass.Sum(s => s.Lhaulco2), "Incorrect Long Haul Air CO2");
            Assert.AreEqual(1548, rpt.ServiceClass.Sum(s => s.Mhaulsegs), "Incorrect Medium Haul # of segments");
            Assert.AreEqual(1603015, rpt.ServiceClass.Sum(s => s.Mhaulmiles), "Incorrect Medium Haul Miles");
            Assert.AreEqual(586938, rpt.ServiceClass.Sum(s => s.Mhaulco2), "Incorrect Medium Haul Air CO2");
            Assert.AreEqual(1764, rpt.ServiceClass.Sum(s => s.Shaulsegs), "Incorrect Short Haul # of segments");
            Assert.AreEqual(618622, rpt.ServiceClass.Sum(s => s.Shaulmiles), "Incorrect Short Haul Miles");
            Assert.AreEqual(379037, rpt.ServiceClass.Sum(s => s.Shaulco2), "Incorrect Short Haul Air CO2");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationInvoiceDateBreak1()
        {
            GenerateReportHandoff(DateType.InvoiceDate);
            ManipulateReportHandoffRecords("1200,1600,30279,1188,401025", "INACCT");
            ManipulateReportHandoffRecords("1", "PREPOST");
            ManipulateReportHandoffRecords("DT:2015,1,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,1,20", "ENDDATE");
            ManipulateReportHandoffRecords("Break1", "DDTOPBRKCAT");
            InsertReportHandoff();

            var rpt = (Co2AirSum)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            var airSummary = rpt.AirSummary.FirstOrDefault();
            Assert.AreEqual(1373, airSummary.Invoices, "Incorrect number of Invoices");
            Assert.AreEqual(0, airSummary.Credits, "Incorrect number of Credits");
            Assert.AreEqual(1373, airSummary.Trips, "Incorrect number of Trips");
            Assert.AreEqual(1094698.34m, airSummary.Airchg, "Incorrect Air Charges");
            Assert.AreEqual(797.30m, Math.Round(airSummary.Avgairchg, 2), "Incorrect Average Air Charges");
            Assert.AreEqual(1839480, airSummary.Airco2, "Incorrect Air CO2");
            Assert.AreEqual(542272, airSummary.Altrailco2, "Incorrect Rail CO2");
            Assert.AreEqual(1656989, airSummary.Altcarco2, "Incorrect Car CO2");

            Assert.AreEqual(3193, rpt.TopGroup.Sum(s => s.Segs), "Incorrect Top Group # of segments");
            Assert.AreEqual(3123252, rpt.TopGroup.Sum(s => s.Miles), "Incorrect Top Group Miles");
            Assert.AreEqual(1664390, rpt.TopGroup.Sum(s => s.Airco2), "Incorrect Top Group Air CO2");

            //TODO: This will be incorrect until we resolve the carbon calc/collapse stuff
            Assert.AreEqual(68, rpt.CityPair.Sum(s => s.Segments), "Incorrect City Pair # of segments");
            Assert.AreEqual(255328, rpt.CityPair.Sum(s => s.Miles), "Incorrect City Pair Miles");
            Assert.AreEqual(206884, rpt.CityPair.Sum(s => s.Airco2), "Incorrect City Pair Air CO2");

            Assert.AreEqual(3286, rpt.TopCarriers.Sum(s => s.Segs), "Incorrect Top Carriers # of segments");
            Assert.AreEqual(3027065, rpt.TopCarriers.Sum(s => s.Miles), "Incorrect Top Carriers Miles");
            Assert.AreEqual(1568290, rpt.TopCarriers.Sum(s => s.Airco2), "Incorrect Top Carriers Air CO2");

            Assert.AreEqual(135, rpt.ServiceClass.Sum(s => s.Fclssegs), "Incorrect First Class # of segments");
            Assert.AreEqual(207626, rpt.ServiceClass.Sum(s => s.Fclsmiles), "Incorrect First Class Miles");
            Assert.AreEqual(187528, rpt.ServiceClass.Sum(s => s.Fclsco2), "Incorrect First Class Air CO2");
            Assert.AreEqual(124, rpt.ServiceClass.Sum(s => s.Bclssegs), "Incorrect Business Class # of segments");
            Assert.AreEqual(438896, rpt.ServiceClass.Sum(s => s.Bclsmiles), "Incorrect Business Class Miles");
            Assert.AreEqual(384570, rpt.ServiceClass.Sum(s => s.Bclsco2), "Incorrect Business Class Air CO2");
            Assert.AreEqual(3368, rpt.ServiceClass.Sum(s => s.Eclssegs), "Incorrect Economy Class # of segments");
            Assert.AreEqual(2846531, rpt.ServiceClass.Sum(s => s.Eclsmiles), "Incorrect Economy Class Miles");
            Assert.AreEqual(1267380, rpt.ServiceClass.Sum(s => s.Eclsco2), "Incorrect Economy Class Air CO2");

            Assert.AreEqual(315, rpt.ServiceClass.Sum(s => s.Lhaulsegs), "Incorrect Long Haul # of segments");
            Assert.AreEqual(1271416, rpt.ServiceClass.Sum(s => s.Lhaulmiles), "Incorrect Long Haul Miles");
            Assert.AreEqual(873504, rpt.ServiceClass.Sum(s => s.Lhaulco2), "Incorrect Long Haul Air CO2");
            Assert.AreEqual(1548, rpt.ServiceClass.Sum(s => s.Mhaulsegs), "Incorrect Medium Haul # of segments");
            Assert.AreEqual(1603015, rpt.ServiceClass.Sum(s => s.Mhaulmiles), "Incorrect Medium Haul Miles");
            Assert.AreEqual(586938, rpt.ServiceClass.Sum(s => s.Mhaulco2), "Incorrect Medium Haul Air CO2");
            Assert.AreEqual(1764, rpt.ServiceClass.Sum(s => s.Shaulsegs), "Incorrect Short Haul # of segments");
            Assert.AreEqual(618622, rpt.ServiceClass.Sum(s => s.Shaulmiles), "Incorrect Short Haul Miles");
            Assert.AreEqual(379037, rpt.ServiceClass.Sum(s => s.Shaulco2), "Incorrect Short Haul Air CO2");

            ClearReportHandoff();
        }


        public void GenerateReportHandoff(DateType dateType)
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2016,1,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CARBONCALC", ParmValue = "CISCARBON", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2016,1,20", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "USD", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "4", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = "2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "222", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibCO2AirSum", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3393747", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1591", ParmInOut = "IN", LangCode = "" });

            ManipulateReportHandoffRecords(((int)dateType).ToString(), "DATERANGE");

        }
    }
}
