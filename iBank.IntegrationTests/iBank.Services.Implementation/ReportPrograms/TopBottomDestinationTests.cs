using System;
using System.Linq;
using Domain.Helper;
using iBank.IntegrationTests.TestTools;
using iBank.Services.Implementation.ReportPrograms.TopBottomDestinations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.IntegrationTests.iBank.Services.Implementation.ReportPrograms
{
    [TestClass]
    public class TopBottomDestinationTests : BaseUnitTest
    {
        #region First Destination tests
        [TestMethod]
        public void BackOfficeFirstDestDepartureDate()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            InsertReportHandoff();

            var rpt = (TopBottomDestinations)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            var firstRec = rpt.FinalDataList.FirstOrDefault();
            if (firstRec == null)
            {
                Assert.Fail("No records in final data list");
            }
            Assert.AreEqual(7, rpt.FinalDataList.Count, "Count is incorrect");

            Assert.AreEqual(97, firstRec.Trips, "# of trips is incorrect");
            Assert.AreEqual(150, firstRec.Ytdtrips, "# of trips YTD is incorrect");
            Assert.AreEqual(38594.79m, firstRec.Volume, "First record volume is incorrect");
            Assert.AreEqual(59620.06m, firstRec.Ytdvolume, "First record volume YTD is incorrect");
            Assert.AreEqual(397.88m, Math.Round(firstRec.Avgcost,2), "First record average cost is incorrect");
            Assert.AreEqual(397.47m, Math.Round(firstRec.Ytdavgcost,2), "First record average cost YTD is incorrect");

            Assert.AreEqual(138, rpt.FinalDataList.Sum(s => s.Trips), "Total # of trips");
            Assert.AreEqual(229, rpt.FinalDataList.Sum(s => s.Ytdtrips), "Total # of trips YTD");
            Assert.AreEqual(74942.43m, rpt.FinalDataList.Sum(s => s.Volume), "Total Volume Booked is incorrect");
            Assert.AreEqual(117770.66m, rpt.FinalDataList.Sum(s => s.Ytdvolume), "Total Volume Booked YTD is incorrect");


            Assert.AreEqual(138, rpt.TotCnt, "Report Total # of trips");
            Assert.AreEqual(229, rpt.TotCnt2, "Report Total # of trips YTD");
            Assert.AreEqual(74942.43m, rpt.TotChg, "Report Total Volume Booked is incorrect");
            Assert.AreEqual(117770.66m, rpt.TotChg2, "Report Total Volume Booked YTD is incorrect");
           
            ClearReportHandoff();
        }

       

        [TestMethod]
        public void BackOfficeFirstDestDepartureDateGroupByCountryAndDestination()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("3","GROUPBY");
            InsertReportHandoff();

            var rpt = (TopBottomDestinations)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            var firstRec = rpt.FinalDataList.FirstOrDefault();
            if (firstRec == null)
            {
                Assert.Fail("No records in final data list");
            }
            Assert.AreEqual(36, rpt.FinalDataList.Count, "Count is incorrect");

            Assert.AreEqual(32, firstRec.Trips, "# of trips is incorrect");
            Assert.AreEqual(38, firstRec.Ytdtrips, "# of trips YTD is incorrect");
            Assert.AreEqual(13648.88m, firstRec.Volume, "First record volume is incorrect");
            Assert.AreEqual(15858.42m, firstRec.Ytdvolume, "First record volume YTD is incorrect");
            Assert.AreEqual(426.53m, Math.Round(firstRec.Avgcost, 2), "First record average cost is incorrect");
            Assert.AreEqual(417.33m, Math.Round(firstRec.Ytdavgcost, 2), "First record average cost YTD is incorrect");

            Assert.AreEqual(138, rpt.FinalDataList.Sum(s => s.Trips), "Total # of trips");
            Assert.AreEqual(229, rpt.FinalDataList.Sum(s => s.Ytdtrips), "Total # of trips YTD");
            Assert.AreEqual(74942.43m, rpt.FinalDataList.Sum(s => s.Volume), "Total Volume Booked is incorrect");
            Assert.AreEqual(117770.66m, rpt.FinalDataList.Sum(s => s.Ytdvolume), "Total Volume Booked YTD is incorrect");


            Assert.AreEqual(138, rpt.TotCnt, "Report Total # of trips");
            Assert.AreEqual(229, rpt.TotCnt2, "Report Total # of trips YTD");
            Assert.AreEqual(74942.43m, rpt.TotChg, "Report Total Volume Booked is incorrect");
            Assert.AreEqual(117770.66m, rpt.TotChg2, "Report Total Volume Booked YTD is incorrect");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeFirstDestDepartureDateGroupByDestinationCity()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("4", "GROUPBY");
            InsertReportHandoff();

            var rpt = (TopBottomDestinations)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            var firstRec = rpt.FinalDataList.FirstOrDefault();
            if (firstRec == null)
            {
                Assert.Fail("No records in final data list");
            }
            Assert.AreEqual(10, rpt.FinalDataList.Count, "Count is incorrect");

            Assert.AreEqual(32, firstRec.Trips, "# of trips is incorrect");
            Assert.AreEqual(38, firstRec.Ytdtrips, "# of trips YTD is incorrect");
            Assert.AreEqual(13648.88m, firstRec.Volume, "First record volume is incorrect");
            Assert.AreEqual(15858.42m, firstRec.Ytdvolume, "First record volume YTD is incorrect");
            Assert.AreEqual(426.53m, Math.Round(firstRec.Avgcost, 2), "First record average cost is incorrect");
            Assert.AreEqual(417.33m, Math.Round(firstRec.Ytdavgcost, 2), "First record average cost YTD is incorrect");

            Assert.AreEqual(93, rpt.FinalDataList.Sum(s => s.Trips), "Total # of trips");
            Assert.AreEqual(142, rpt.FinalDataList.Sum(s => s.Ytdtrips), "Total # of trips YTD");
            Assert.AreEqual(57535.67m, rpt.FinalDataList.Sum(s => s.Volume), "Total Volume Booked is incorrect");
            Assert.AreEqual(79235.83m, rpt.FinalDataList.Sum(s => s.Ytdvolume), "Total Volume Booked YTD is incorrect");


            Assert.AreEqual(138, rpt.TotCnt, "Report Total # of trips");
            Assert.AreEqual(229, rpt.TotCnt2, "Report Total # of trips YTD");
            Assert.AreEqual(74942.43m, rpt.TotChg, "Report Total Volume Booked is incorrect");
            Assert.AreEqual(117770.66m, rpt.TotChg2, "Report Total Volume Booked YTD is incorrect");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeFirstDestDepartureDateGroupByDestinationCityValCarr()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("5", "GROUPBY");
            InsertReportHandoff();

            var rpt = (TopBottomDestinations)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            var firstRec = rpt.FinalDataList.FirstOrDefault();
            if (firstRec == null)
            {
                Assert.Fail("No records in final data list");
            }
            Assert.AreEqual(20, rpt.FinalDataList.Count, "Count is incorrect");

            Assert.AreEqual(25, firstRec.Trips, "# of trips is incorrect");
            Assert.AreEqual(29, firstRec.Ytdtrips, "# of trips YTD is incorrect");
            Assert.AreEqual(11534.49m, firstRec.Volume, "First record volume is incorrect");
            Assert.AreEqual(13197.34m, firstRec.Ytdvolume, "First record volume YTD is incorrect");
            Assert.AreEqual(461.38m, Math.Round(firstRec.Avgcost, 2), "First record average cost is incorrect");
            Assert.AreEqual(455.08m, Math.Round(firstRec.Ytdavgcost, 2), "First record average cost YTD is incorrect");

            Assert.AreEqual(93, rpt.FinalDataList.Sum(s => s.Trips), "Total # of trips");
            Assert.AreEqual(142, rpt.FinalDataList.Sum(s => s.Ytdtrips), "Total # of trips YTD");
            Assert.AreEqual(57535.67m, rpt.FinalDataList.Sum(s => s.Volume), "Total Volume Booked is incorrect");
            Assert.AreEqual(79235.83m, rpt.FinalDataList.Sum(s => s.Ytdvolume), "Total Volume Booked YTD is incorrect");


            Assert.AreEqual(138, rpt.TotCnt, "Report Total # of trips");
            Assert.AreEqual(229, rpt.TotCnt2, "Report Total # of trips YTD");
            Assert.AreEqual(74942.43m, rpt.TotChg, "Report Total Volume Booked is incorrect");
            Assert.AreEqual(117770.66m, rpt.TotChg2, "Report Total Volume Booked YTD is incorrect");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeFirstDestDepartureDateGroupByTripCityPair()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("6", "GROUPBY");
            InsertReportHandoff();

            var rpt = (TopBottomDestinations)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            var firstRec = rpt.FinalDataList.FirstOrDefault();
            if (firstRec == null)
            {
                Assert.Fail("No records in final data list");
            }
            Assert.AreEqual(10, rpt.FinalDataList.Count, "Count is incorrect");

            Assert.AreEqual(22, firstRec.Trips, "# of trips is incorrect");
            Assert.AreEqual(30, firstRec.Ytdtrips, "# of trips YTD is incorrect");
            Assert.AreEqual(12265.14m, firstRec.Volume, "First record volume is incorrect");
            Assert.AreEqual(17853.77m, firstRec.Ytdvolume, "First record volume YTD is incorrect");
            Assert.AreEqual(557.51m, Math.Round(firstRec.Avgcost, 2), "First record average cost is incorrect");
            Assert.AreEqual(595.13m, Math.Round(firstRec.Ytdavgcost, 2), "First record average cost YTD is incorrect");

            Assert.AreEqual(72, rpt.FinalDataList.Sum(s => s.Trips), "Total # of trips");
            Assert.AreEqual(108, rpt.FinalDataList.Sum(s => s.Ytdtrips), "Total # of trips YTD");
            Assert.AreEqual(47807.80m, rpt.FinalDataList.Sum(s => s.Volume), "Total Volume Booked is incorrect");
            Assert.AreEqual(63001.45m, rpt.FinalDataList.Sum(s => s.Ytdvolume), "Total Volume Booked YTD is incorrect");


            Assert.AreEqual(138, rpt.TotCnt, "Report Total # of trips");
            Assert.AreEqual(229, rpt.TotCnt2, "Report Total # of trips YTD");
            Assert.AreEqual(74942.43m, rpt.TotChg, "Report Total Volume Booked is incorrect");
            Assert.AreEqual(117770.66m, rpt.TotChg2, "Report Total Volume Booked YTD is incorrect");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeFirstDestInvoiceDate()
        {
            GenerateReportHandoff(DateType.InvoiceDate);
            InsertReportHandoff();

            var rpt = (TopBottomDestinations)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            var firstRec = rpt.FinalDataList.FirstOrDefault();
            if (firstRec == null)
            {
                Assert.Fail("No records in final data list");
            }
            Assert.AreEqual(8, rpt.FinalDataList.Count, "Count is incorrect");

            Assert.AreEqual(60, firstRec.Trips, "# of trips is incorrect");
            Assert.AreEqual(103, firstRec.Ytdtrips, "# of trips YTD is incorrect");
            Assert.AreEqual(38126.52m, firstRec.Volume, "First record volume is incorrect");
            Assert.AreEqual(65038.18m, firstRec.Ytdvolume, "First record volume YTD is incorrect");
            Assert.AreEqual(635.44m, Math.Round(firstRec.Avgcost, 2), "First record average cost is incorrect");
            Assert.AreEqual(631.44m, Math.Round(firstRec.Ytdavgcost, 2), "First record average cost YTD is incorrect");

            Assert.AreEqual(135, rpt.FinalDataList.Sum(s => s.Trips), "Total # of trips");
            Assert.AreEqual(226, rpt.FinalDataList.Sum(s => s.Ytdtrips), "Total # of trips YTD");
            Assert.AreEqual(75993.18m, rpt.FinalDataList.Sum(s => s.Volume), "Total Volume Booked is incorrect");
            Assert.AreEqual(140491.80m, rpt.FinalDataList.Sum(s => s.Ytdvolume), "Total Volume Booked YTD is incorrect");


            Assert.AreEqual(135, rpt.TotCnt, "Report Total # of trips");
            Assert.AreEqual(226, rpt.TotCnt2, "Report Total # of trips YTD");
            Assert.AreEqual(75993.18m, rpt.TotChg, "Report Total Volume Booked is incorrect");
            Assert.AreEqual(140491.80m, rpt.TotChg2, "Report Total Volume Booked YTD is incorrect");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationFirstDestDepartureDate()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("1","PREPOST");
            ManipulateReportHandoffRecords("DT:2015,1,15", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,1,16", "ENDDATE");
            InsertReportHandoff();

            var rpt = (TopBottomDestinations)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            var firstRec = rpt.FinalDataList.FirstOrDefault();
            if (firstRec == null)
            {
                Assert.Fail("No records in final data list");
            }
            Assert.AreEqual(10, rpt.FinalDataList.Count, "Count is incorrect");

            Assert.AreEqual(401, firstRec.Trips, "# of trips is incorrect");
            Assert.AreEqual(2010, firstRec.Ytdtrips, "# of trips YTD is incorrect");
            Assert.AreEqual(203327.31m, firstRec.Volume, "First record volume is incorrect");
            Assert.AreEqual(1094196.16m, firstRec.Ytdvolume, "First record volume YTD is incorrect");
            Assert.AreEqual(507.05m, Math.Round(firstRec.Avgcost, 2), "First record average cost is incorrect");
            Assert.AreEqual(544.38m, Math.Round(firstRec.Ytdavgcost, 2), "First record average cost YTD is incorrect");

            Assert.AreEqual(418, rpt.FinalDataList.Sum(s => s.Trips), "Total # of trips");
            Assert.AreEqual(2054, rpt.FinalDataList.Sum(s => s.Ytdtrips), "Total # of trips YTD");
            Assert.AreEqual(221788.92m, rpt.FinalDataList.Sum(s => s.Volume), "Total Volume Booked is incorrect");
            Assert.AreEqual(1176143.21m, rpt.FinalDataList.Sum(s => s.Ytdvolume), "Total Volume Booked YTD is incorrect");


            Assert.AreEqual(426, rpt.TotCnt, "Report Total # of trips");
            Assert.AreEqual(2118, rpt.TotCnt2, "Report Total # of trips YTD");
            Assert.AreEqual(224284.47m, rpt.TotChg, "Report Total Volume Booked is incorrect");
            Assert.AreEqual(1219268.71m, rpt.TotChg2, "Report Total Volume Booked YTD is incorrect");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationFirstDestInvoiceDate()
        {
            GenerateReportHandoff(DateType.InvoiceDate);
            ManipulateReportHandoffRecords("1", "PREPOST");
            ManipulateReportHandoffRecords("DT:2015,1,15", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,1,16", "ENDDATE");
            InsertReportHandoff();

            var rpt = (TopBottomDestinations)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            var firstRec = rpt.FinalDataList.FirstOrDefault();
            if (firstRec == null)
            {
                Assert.Fail("No records in final data list");
            }
            Assert.AreEqual(10, rpt.FinalDataList.Count, "Count is incorrect");

            Assert.AreEqual(1217, firstRec.Trips, "# of trips is incorrect");
            Assert.AreEqual(7481, firstRec.Ytdtrips, "# of trips YTD is incorrect");
            Assert.AreEqual(534076.39m, firstRec.Volume, "First record volume is incorrect");
            Assert.AreEqual(3258765.41m, firstRec.Ytdvolume, "First record volume YTD is incorrect");
            Assert.AreEqual(438.85m, Math.Round(firstRec.Avgcost, 2), "First record average cost is incorrect");
            Assert.AreEqual(435.61m, Math.Round(firstRec.Ytdavgcost, 2), "First record average cost YTD is incorrect");

            Assert.AreEqual(1275, rpt.FinalDataList.Sum(s => s.Trips), "Total # of trips");
            Assert.AreEqual(7702, rpt.FinalDataList.Sum(s => s.Ytdtrips), "Total # of trips YTD");
            Assert.AreEqual(750474.47m, rpt.FinalDataList.Sum(s => s.Volume), "Total Volume Booked is incorrect");
            Assert.AreEqual(4061354.93m, rpt.FinalDataList.Sum(s => s.Ytdvolume), "Total Volume Booked YTD is incorrect");


            Assert.AreEqual(1353, rpt.TotCnt, "Report Total # of trips");
            Assert.AreEqual(8087, rpt.TotCnt2, "Report Total # of trips YTD");
            Assert.AreEqual(839980.64m, rpt.TotChg, "Report Total Volume Booked is incorrect");
            Assert.AreEqual(4551386.98m, rpt.TotChg2, "Report Total Volume Booked YTD is incorrect");

            ClearReportHandoff();
        }

        [TestMethod]
        public void ReservationFirstDestBookedDate()
        {
            GenerateReportHandoff(DateType.BookedDate);
            ManipulateReportHandoffRecords("1", "PREPOST");
            ManipulateReportHandoffRecords("DT:2015,1,15", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,1,16", "ENDDATE");
            InsertReportHandoff();

            var rpt = (TopBottomDestinations)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            var firstRec = rpt.FinalDataList.FirstOrDefault();
            if (firstRec == null)
            {
                Assert.Fail("No records in final data list");
            }
            Assert.AreEqual(10, rpt.FinalDataList.Count, "Count is incorrect");

            Assert.AreEqual(1293, firstRec.Trips, "# of trips is incorrect");
            Assert.AreEqual(8455, firstRec.Ytdtrips, "# of trips YTD is incorrect");
            Assert.AreEqual(596331.22m, firstRec.Volume, "First record volume is incorrect");
            Assert.AreEqual(3815640.75m, firstRec.Ytdvolume, "First record volume YTD is incorrect");
            Assert.AreEqual(461.20m, Math.Round(firstRec.Avgcost, 2), "First record average cost is incorrect");
            Assert.AreEqual(451.29m, Math.Round(firstRec.Ytdavgcost, 2), "First record average cost YTD is incorrect");

            Assert.AreEqual(1362, rpt.FinalDataList.Sum(s => s.Trips), "Total # of trips");
            Assert.AreEqual(8760, rpt.FinalDataList.Sum(s => s.Ytdtrips), "Total # of trips YTD");
            Assert.AreEqual(903202.53m, rpt.FinalDataList.Sum(s => s.Volume), "Total Volume Booked is incorrect");
            Assert.AreEqual(5016426.72m, rpt.FinalDataList.Sum(s => s.Ytdvolume), "Total Volume Booked YTD is incorrect");


            Assert.AreEqual(1457, rpt.TotCnt, "Report Total # of trips");
            Assert.AreEqual(9270, rpt.TotCnt2, "Report Total # of trips YTD");
            Assert.AreEqual(1026103.65m, rpt.TotChg, "Report Total Volume Booked is incorrect");
            Assert.AreEqual(5840767.55m, rpt.TotChg2, "Report Total Volume Booked YTD is incorrect");

            ClearReportHandoff();
        }

        #endregion

        #region Furthest Distance
        [TestMethod]
        public void BackOfficeFurthestDestDepartureDate()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("2","DDWHICHDEST");
            InsertReportHandoff();

            var rpt = (TopBottomDestinations)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            var firstRec = rpt.FinalDataList.FirstOrDefault();
            if (firstRec == null)
            {
                Assert.Fail("No records in final data list");
            }
            Assert.AreEqual(3, rpt.FinalDataList.Count, "Count is incorrect");

            Assert.AreEqual(109, firstRec.Trips, "# of trips is incorrect");
            Assert.AreEqual(184, firstRec.Ytdtrips, "# of trips YTD is incorrect");
            Assert.AreEqual(61552.63m, firstRec.Volume, "First record volume is incorrect");
            Assert.AreEqual(91909.44m, firstRec.Ytdvolume, "First record volume YTD is incorrect");
            Assert.AreEqual(564.70m, Math.Round(firstRec.Avgcost, 2), "First record average cost is incorrect");
            Assert.AreEqual(499.51m, Math.Round(firstRec.Ytdavgcost, 2), "First record average cost YTD is incorrect");

            Assert.AreEqual(138, rpt.FinalDataList.Sum(s => s.Trips), "Total # of trips");
            Assert.AreEqual(229, rpt.FinalDataList.Sum(s => s.Ytdtrips), "Total # of trips YTD");
            Assert.AreEqual(74942.43m, rpt.FinalDataList.Sum(s => s.Volume), "Total Volume Booked is incorrect");
            Assert.AreEqual(117770.66m, rpt.FinalDataList.Sum(s => s.Ytdvolume), "Total Volume Booked YTD is incorrect");


            Assert.AreEqual(138, rpt.TotCnt, "Report Total # of trips");
            Assert.AreEqual(229, rpt.TotCnt2, "Report Total # of trips YTD");
            Assert.AreEqual(74942.43m, rpt.TotChg, "Report Total Volume Booked is incorrect");
            Assert.AreEqual(117770.66m, rpt.TotChg2, "Report Total Volume Booked YTD is incorrect");

            ClearReportHandoff();
        }

        [TestMethod]
        public void BackOfficeFurthestDestDepartureDateGroupByDestinationCity()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("2", "DDWHICHDEST");
            ManipulateReportHandoffRecords("4", "GROUPBY");
            InsertReportHandoff();

            var rpt = (TopBottomDestinations)RunReport();

            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            var firstRec = rpt.FinalDataList.FirstOrDefault();
            if (firstRec == null)
            {
                Assert.Fail("No records in final data list");
            }
            Assert.AreEqual(10, rpt.FinalDataList.Count, "Count is incorrect");

            Assert.AreEqual(45, firstRec.Trips, "# of trips is incorrect");
            Assert.AreEqual(89, firstRec.Ytdtrips, "# of trips YTD is incorrect");
            Assert.AreEqual(35581.36m, firstRec.Volume, "First record volume is incorrect");
            Assert.AreEqual(51378.60m, firstRec.Ytdvolume, "First record volume YTD is incorrect");
            Assert.AreEqual(790.70m, Math.Round(firstRec.Avgcost, 2), "First record average cost is incorrect");
            Assert.AreEqual(577.29m, Math.Round(firstRec.Ytdavgcost, 2), "First record average cost YTD is incorrect");

            Assert.AreEqual(113, rpt.FinalDataList.Sum(s => s.Trips), "Total # of trips");
            Assert.AreEqual(179, rpt.FinalDataList.Sum(s => s.Ytdtrips), "Total # of trips YTD");
            Assert.AreEqual(66607.09m, rpt.FinalDataList.Sum(s => s.Volume), "Total Volume Booked is incorrect");
            Assert.AreEqual(100058.66m, rpt.FinalDataList.Sum(s => s.Ytdvolume), "Total Volume Booked YTD is incorrect");


            Assert.AreEqual(138, rpt.TotCnt, "Report Total # of trips");
            Assert.AreEqual(229, rpt.TotCnt2, "Report Total # of trips YTD");
            Assert.AreEqual(74942.43m, rpt.TotChg, "Report Total Volume Booked is incorrect");
            Assert.AreEqual(117770.66m, rpt.TotChg2, "Report Total Volume Booked YTD is incorrect");

            ClearReportHandoff();
        }
        #endregion 

        public void GenerateReportHandoff(DateType dateType)
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2016,1,15", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DDWHICHDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2016,1,31", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "GROUPBY", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "HOWMANY", ParmValue = "10", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "USD", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "3", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = "2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "64", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibTopDests", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RBSORTDESCASC", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3399432", ParmInOut = "IN", LangCode = "" });
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
