using System;
using System.Linq;

using Domain.Helper;

using iBank.IntegrationTests.TestTools;
using iBank.Services.Implementation.ReportPrograms.ExecutiveSummarywithGraphs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.IntegrationTests.iBank.Services.Implementation.ReportPrograms
{
    [TestClass]
    public class ExecutiveSummaryWithGraphsTests : BaseUnitTest
    {
        [TestMethod]
        public void GenerateReportTooMuchData()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords(string.Empty, "INACCT");
            InsertReportHandoff();

            //run the report
            var rpt = (Qview4)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(2, rptInfo.ReturnCode, "Return code failed.");
            var containsNoDataMsg = rptInfo.ErrorMessage.Contains(ReportGlobals.ReportMessages.RptMsg_BigVolume);
            Assert.AreEqual(true, containsNoDataMsg, "Error message failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportDepartureDateAllBackoffice()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            InsertReportHandoff();

            //run the report
            var rpt = (Qview4)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");


            //Check air charges
            var airSum = rpt.AirSumSubreport.FirstOrDefault();
            Assert.AreEqual(59350.70m,airSum.Airchg);
            Assert.AreEqual(23486.56m, airSum.Savings);
            Assert.AreEqual(0m, airSum.Negosvngs);
            Assert.AreEqual(518.01m, airSum.Lostamt);
            Assert.AreEqual(0m, airSum.Svcfee);

            //Check air city pairs
            var airCityPairs = rpt.TopCitySubreport.FirstOrDefault();
            Assert.AreEqual(6014.48m,airCityPairs.Cost);

            //check car rental summary
            var carSum = rpt.CarSumSubreport.FirstOrDefault();
            Assert.AreEqual(1424.74m, carSum.Carchg);
            Assert.AreEqual(41.46m, Math.Round(carSum.Avgrate,2));
            Assert.AreEqual(22.98m, Math.Round(carSum.Avgdaycost,2));

            //check car rental cities
            var carCityPairs = rpt.TopCarSubreport.FirstOrDefault();
            Assert.AreEqual("TORONTO, ON", carCityPairs.Carcity.Trim().ToUpper());

            //check hotel booking summary
            var hotelSum = rpt.HotelSumSubreport.FirstOrDefault();
            Assert.AreEqual(7489.07m, hotelSum.Roomchg);
            Assert.AreEqual(150.74m, Math.Round(hotelSum.Avgrate, 2));
            Assert.AreEqual(94.80m, Math.Round(hotelSum.Avgnitcost, 2));

            //check hotel cities
            var hotelCityPairs = rpt.TopHotSubreport.FirstOrDefault();
            Assert.AreEqual(1447.81m, hotelCityPairs.Hotcost);

            //Check air pie chart
            var pieChart = rpt.AirPieSubreport.OrderByDescending(s => s.Data1).FirstOrDefault();
            Assert.AreEqual("AIR CANADA",pieChart.Catdesc.Trim().ToUpper());
            Assert.AreEqual(76.8m,Math.Round(pieChart.Data1,1));

            //Check car pie chart
            pieChart = rpt.CarPieSubreport.OrderByDescending(s => s.Data1).FirstOrDefault();
            Assert.AreEqual("HERTZ", pieChart.Catdesc.Trim().ToUpper());
            Assert.AreEqual(.9839m, Math.Round(pieChart.Data1, 4));

            //Check hotel pie chart
            pieChart = rpt.HotelPieSubreport.OrderByDescending(s => s.Data1).FirstOrDefault();
            Assert.AreEqual("OTHER", pieChart.Catdesc.Trim().ToUpper());
            Assert.AreEqual(.3671m, Math.Round(pieChart.Data1, 4));


            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportInvoiceDateAllBackoffice()
        {
            GenerateReportHandoff(DateType.InvoiceDate);
            InsertReportHandoff();

            //run the report
            var rpt = (Qview4)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");


            //Check air charges
            var airSum = rpt.AirSumSubreport.FirstOrDefault();
            Assert.AreEqual(71613.97m, airSum.Airchg);
            Assert.AreEqual(33467.51m, airSum.Savings);
            Assert.AreEqual(0m, airSum.Negosvngs);
            Assert.AreEqual(86.63m, airSum.Lostamt);
            Assert.AreEqual(0m, airSum.Svcfee);

            //Check air city pairs
            var airCityPairs = rpt.TopCitySubreport.FirstOrDefault();
            Assert.AreEqual(3320.21m, airCityPairs.Cost);

            //check car rental summary
            var carSum = rpt.CarSumSubreport.FirstOrDefault();
            Assert.AreEqual(1861.91m, carSum.Carchg);
            Assert.AreEqual(40.87m, Math.Round(carSum.Avgrate, 2));
            Assert.AreEqual(22.71m, Math.Round(carSum.Avgdaycost, 2));

            //check car rental cities
            var carCityPairs = rpt.TopCarSubreport.FirstOrDefault();
            Assert.AreEqual(378.54m, carCityPairs.Carcost);

            //check hotel booking summary
            var hotelSum = rpt.HotelSumSubreport.FirstOrDefault();
            Assert.AreEqual(6199.51m, hotelSum.Roomchg);
            Assert.AreEqual(156.27m, Math.Round(hotelSum.Avgrate, 2));
            Assert.AreEqual(78.47m, Math.Round(hotelSum.Avgnitcost, 2));

            //check hotel cities
            var hotelCityPairs = rpt.TopHotSubreport.FirstOrDefault();
            Assert.AreEqual(131.52m, hotelCityPairs.Hotcost);

            //Check air pie chart
            var pieChart = rpt.AirPieSubreport.OrderByDescending(s => s.Data1).FirstOrDefault();
            Assert.AreEqual("AIR CANADA", pieChart.Catdesc.Trim().ToUpper());
            Assert.AreEqual(72.8m, Math.Round(pieChart.Data1, 1));

            //Check car pie chart
            pieChart = rpt.CarPieSubreport.OrderByDescending(s => s.Data1).FirstOrDefault();
            Assert.AreEqual("HERTZ", pieChart.Catdesc.Trim().ToUpper());
            Assert.AreEqual(.9634m, Math.Round(pieChart.Data1, 4));

            //Check hotel pie chart
            pieChart = rpt.HotelPieSubreport.OrderByDescending(s => s.Data1).FirstOrDefault();
            Assert.AreEqual("OTHER", pieChart.Catdesc.Trim().ToUpper());
            Assert.AreEqual(.3671m, Math.Round(pieChart.Data1, 4));


            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportDepartureDateAllReservation()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("1", "PREPOSTAIR");
            ManipulateReportHandoffRecords("1", "PREPOSTCAR");
            ManipulateReportHandoffRecords("1", "PREPOSTHOT");
            ManipulateReportHandoffRecords("DT:2016,8,30", "ENDDATE");
            ManipulateReportHandoffRecords("9000234,1200,30032,30448", "INACCT");

            InsertReportHandoff();

            //run the report
            var rpt = (Qview4)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");


            //Check air charges
            var airSum = rpt.AirSumSubreport.FirstOrDefault();
            Assert.AreEqual(27518.64m, airSum.Airchg);
            Assert.AreEqual(15007.66m, airSum.Savings);
            Assert.AreEqual(0m, airSum.Negosvngs);
            Assert.AreEqual(1630.69m, airSum.Lostamt);
            Assert.AreEqual(0m, airSum.Svcfee);

            //Check air city pairs
            var airCityPairs = rpt.TopCitySubreport.FirstOrDefault();
            Assert.AreEqual(4610.25m, airCityPairs.Cost);

            //check car rental summary
            var carSum = rpt.CarSumSubreport.FirstOrDefault();
            Assert.AreEqual(241.05m, carSum.Carchg);
            Assert.AreEqual(47.18m, Math.Round(carSum.Avgrate, 2));
            Assert.AreEqual(48.21m, Math.Round(carSum.Avgdaycost, 2));

            //check car rental cities
            var carCityPairs = rpt.TopCarSubreport.FirstOrDefault();
            Assert.AreEqual(157.05m, carCityPairs.Carcost);

            //check hotel booking summary
            var hotelSum = rpt.HotelSumSubreport.FirstOrDefault();
            Assert.AreEqual(6191.00m, hotelSum.Roomchg);
            Assert.AreEqual(240.60m, Math.Round(hotelSum.Avgrate, 2));
            Assert.AreEqual(247.64m, Math.Round(hotelSum.Avgnitcost, 2));

            //check hotel cities
            var hotelCityPairs = rpt.TopHotSubreport.FirstOrDefault();
            Assert.AreEqual(6191.00m, hotelCityPairs.Hotcost);

            //Check air pie chart
            var pieChart = rpt.AirPieSubreport.OrderByDescending(s => s.Data1).FirstOrDefault();
            Assert.AreEqual("UNITED AIRLINES", pieChart.Catdesc.Trim().ToUpper());
            Assert.AreEqual(46.1m, Math.Round(pieChart.Data1, 1));

            //Check car pie chart
            pieChart = rpt.CarPieSubreport.OrderByDescending(s => s.Data1).FirstOrDefault();
            Assert.AreEqual("HERTZ", pieChart.Catdesc.Trim().ToUpper());
            Assert.AreEqual(.6000m, Math.Round(pieChart.Data1, 4));

            //Check hotel pie chart
            pieChart = rpt.HotelPieSubreport.OrderByDescending(s => s.Data1).FirstOrDefault();
            Assert.AreEqual("MARRIOTT", pieChart.Catdesc.Trim().ToUpper());
            Assert.AreEqual(.4000m, Math.Round(pieChart.Data1, 4));


            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportInvoiceDateAllReservation()
        {
            GenerateReportHandoff(DateType.InvoiceDate);
            ManipulateReportHandoffRecords("1", "PREPOSTAIR");
            ManipulateReportHandoffRecords("1", "PREPOSTCAR");
            ManipulateReportHandoffRecords("1", "PREPOSTHOT");
            ManipulateReportHandoffRecords("DT:2016,8,30", "ENDDATE");
            ManipulateReportHandoffRecords("9000234,1200,30032,30448", "INACCT");

            InsertReportHandoff();

            //run the report
            var rpt = (Qview4)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");


            //Check air charges
            var airSum = rpt.AirSumSubreport.FirstOrDefault();
            Assert.AreEqual(15308.94m, airSum.Airchg);
            Assert.AreEqual(4624.66m, airSum.Savings);
            Assert.AreEqual(0m, airSum.Negosvngs);
            Assert.AreEqual(428.59m, airSum.Lostamt);
            Assert.AreEqual(0m, airSum.Svcfee);

            //Check air city pairs
            var airCityPairs = rpt.TopCitySubreport.FirstOrDefault();
            Assert.AreEqual(0m, airCityPairs.Cost);

            //check car rental summary
            var carSum = rpt.CarSumSubreport.FirstOrDefault();
            Assert.AreEqual(309.60m, carSum.Carchg);
            Assert.AreEqual(77.40m, Math.Round(carSum.Avgrate, 2));
            Assert.AreEqual(77.40m, Math.Round(carSum.Avgdaycost, 2));

            //check car rental cities
            var carCityPairs = rpt.TopCarSubreport.FirstOrDefault();
            Assert.AreEqual(309.60m, carCityPairs.Carcost);

            //check hotel booking summary
            var hotelSum = rpt.HotelSumSubreport.FirstOrDefault();
            Assert.AreEqual(2181.00m, hotelSum.Roomchg);
            Assert.AreEqual(255.67m, Math.Round(hotelSum.Avgrate, 2));
            Assert.AreEqual(242.33m, Math.Round(hotelSum.Avgnitcost, 2));

            //check hotel cities
            var hotelCityPairs = rpt.TopHotSubreport.FirstOrDefault();
            Assert.AreEqual(2181.00m, hotelCityPairs.Hotcost);

            //Check air pie chart
            var pieChart = rpt.AirPieSubreport.OrderByDescending(s => s.Data1).FirstOrDefault();
            Assert.AreEqual("UNITED AIRLINES", pieChart.Catdesc.Trim().ToUpper());
            Assert.AreEqual(52.8m, Math.Round(pieChart.Data1, 1));

            //Check car pie chart
            pieChart = rpt.CarPieSubreport.OrderByDescending(s => s.Data1).FirstOrDefault();
            Assert.AreEqual("HERTZ", pieChart.Catdesc.Trim().ToUpper());
            Assert.AreEqual(1.000m, Math.Round(pieChart.Data1, 4));

            //Check hotel pie chart
            pieChart = rpt.HotelPieSubreport.OrderByDescending(s => s.Data1).FirstOrDefault();
            Assert.AreEqual("MARRIOTT", pieChart.Catdesc.Trim().ToUpper());
            Assert.AreEqual(.4444m, Math.Round(pieChart.Data1, 4));


            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportInvoiceDateMixed()
        {
            GenerateReportHandoff(DateType.InvoiceDate);
            ManipulateReportHandoffRecords("1", "PREPOSTAIR");
            ManipulateReportHandoffRecords("2", "PREPOSTCAR");
            ManipulateReportHandoffRecords("1", "PREPOSTHOT");
            ManipulateReportHandoffRecords("DT:2016,8,30", "ENDDATE");
            ManipulateReportHandoffRecords("9000234,1200,30032,30448", "INACCT");

            InsertReportHandoff();

            //run the report
            var rpt = (Qview4)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");


            //Check air charges
            var airSum = rpt.AirSumSubreport.FirstOrDefault();
            Assert.AreEqual(15308.94m, airSum.Airchg);
            Assert.AreEqual(4624.66m, airSum.Savings);
            Assert.AreEqual(0m, airSum.Negosvngs);
            Assert.AreEqual(428.59m, airSum.Lostamt);
            Assert.AreEqual(0m, airSum.Svcfee);

            //Check air city pairs
            var airCityPairs = rpt.TopCitySubreport.FirstOrDefault();
            Assert.AreEqual(0m, airCityPairs.Cost);

            //check car rental summary
            var carSum = rpt.CarSumSubreport.FirstOrDefault();
            Assert.AreEqual(0m, carSum.Carchg);
            Assert.AreEqual(0m, Math.Round(carSum.Avgrate, 2));
            Assert.AreEqual(0m, Math.Round(carSum.Avgdaycost, 2));

            //check car rental cities
            Assert.AreEqual(0, rpt.TopCarSubreport.Count);

            //check hotel booking summary
            var hotelSum = rpt.HotelSumSubreport.FirstOrDefault();
            Assert.AreEqual(2181.00m, hotelSum.Roomchg);
            Assert.AreEqual(255.67m, Math.Round(hotelSum.Avgrate, 2));
            Assert.AreEqual(242.33m, Math.Round(hotelSum.Avgnitcost, 2));

            //check hotel cities
            var hotelCityPairs = rpt.TopHotSubreport.FirstOrDefault();
            Assert.AreEqual(2181.00m, hotelCityPairs.Hotcost);

            //Check air pie chart
            var pieChart = rpt.AirPieSubreport.OrderByDescending(s => s.Data1).FirstOrDefault();
            Assert.AreEqual("UNITED AIRLINES", pieChart.Catdesc.Trim().ToUpper());
            Assert.AreEqual(52.8m, Math.Round(pieChart.Data1, 1));

            //Check car pie chart
            Assert.AreEqual(0, rpt.CarPieSubreport.Count);

            //Check hotel pie chart
            pieChart = rpt.HotelPieSubreport.OrderByDescending(s => s.Data1).FirstOrDefault();
            Assert.AreEqual("MARRIOTT", pieChart.Catdesc.Trim().ToUpper());
            Assert.AreEqual(.4444m, Math.Round(pieChart.Data1, 4));


            ClearReportHandoff();
        }

        public void GenerateReportHandoff(DateType dateType)
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2015,8,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CARBONCALC", ParmValue = "CISCARBON", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DDCOSMETIC", ParmValue = "CLASSIC", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2015,8,30", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INACCT", ParmValue = "2600", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "USD", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTLANGUAGE", ParmValue = "EN", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "4", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOSTAIR", ParmValue = "2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOSTCAR", ParmValue = "2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOSTHOT", ParmValue = "2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "36", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibQview4", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RBONGRAPHSSHOW", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RBRPTVERSION", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3383069", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1591", ParmInOut = "IN", LangCode = "" });

            ManipulateReportHandoffRecords(((int)dateType).ToString(), "DATERANGE");

        }
    }
}
