using System;
using System.Linq;
using Domain.Helper;
using iBank.IntegrationTests.TestTools;
using iBank.Services.Implementation.ReportPrograms.CO2CombinedSummary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.IntegrationTests.iBank.Services.Implementation.ReportPrograms
{
    [TestClass]
    public class Co2ExecSumTests : BaseUnitTest
    {

        [TestMethod]
        public void TooMuchDataReport()
        {

            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("1", "PREPOST");
            ManipulateReportHandoffRecords("DT:2015,2,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,2,28", "ENDDATE");
            InsertReportHandoff();


            var rpt = (Co2ExecSum)RunReport();

            //artificially lower the record limit to trip the conditional
            rpt.Globals.RecordLimit = 50;
            rpt.RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(2, rptInfo.ReturnCode, "Return code failed.");
            var containsNoDataMsg = rptInfo.ErrorMessage.Contains(ReportGlobals.ReportMessages.RptMsg_BigVolume);
            Assert.AreEqual(true, containsNoDataMsg, "Error message failed.");

            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportBackOfficeDepartureDate()
        {
            GenerateReportHandoff(DateType.DepartureDate);

            ManipulateReportHandoffRecords("DT:2015,9,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,9,5", "ENDDATE");

            InsertReportHandoff();

            //run the report
            var rpt = (Co2ExecSum)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            var airSummary = rpt.AirSummary.FirstOrDefault();
            Assert.AreEqual(12,airSummary.Invoices,"Incorrect number of Invoices");
            Assert.AreEqual(0, airSummary.Credits, "Incorrect number of Credits");
            Assert.AreEqual(12, airSummary.Trips, "Incorrect number of Trips");
            Assert.AreEqual(9568.98m, airSummary.Airchg, "Incorrect Air Charges");
            Assert.AreEqual(797.42m, Math.Round(airSummary.Avgairchg,2), "Incorrect Average Air Charges");
            Assert.AreEqual(21166, airSummary.Airco2, "Incorrect Air CO2");
            Assert.AreEqual(4621, airSummary.Altrailco2, "Incorrect Rail CO2");
            Assert.AreEqual(19749, airSummary.Altcarco2, "Incorrect Car CO2");

            Assert.AreEqual(15, rpt.CityPair.Sum(s => s.Segments), "Incorrect # of segments");
            Assert.AreEqual(34050, rpt.CityPair.Sum(s => s.Miles), "Incorrect Miles");
            Assert.AreEqual(18266, rpt.CityPair.Sum(s => s.Airco2), "Incorrect Air CO2");

            var carSummary = rpt.CarSummary.FirstOrDefault();
            Assert.AreEqual(4, carSummary.Rentals, "Incorrect Car Rentals");
            Assert.AreEqual(7, carSummary.Days, "Incorrect Car Days");
            Assert.AreEqual(257.18m, carSummary.Carcost, "Incorrect Car Cost");
            Assert.AreEqual(375, carSummary.Carco2, "Incorrect Car CO2");

            Assert.AreEqual(7, rpt.TopCar.Sum(s => s.Days), "Incorrect Car Days");
            Assert.AreEqual(376, rpt.TopCar.Sum(s => Math.Round(s.Carco2)), "Incorrect Car CO2");

            var hotelSummary = rpt.HotelSummary.FirstOrDefault();
            Assert.AreEqual(12, hotelSummary.Bookings, "Incorrect Hotel Bookings");
            Assert.AreEqual(16, hotelSummary.Nights, "Incorrect Hotel Nights");
            Assert.AreEqual(1078.40m, hotelSummary.Hotelcost, "Incorrect Hotel Cost");
            Assert.AreEqual(1168, hotelSummary.Hotelco2, "Incorrect Hotel CO2");

            Assert.AreEqual(11, rpt.TopHotel.Sum(s => s.Nights), "Incorrect Hotel Nights");
            Assert.AreEqual(803, rpt.TopHotel.Sum(s => s.Hotelco2), "Incorrect Hotel CO2");

            ClearReportHandoff();
        }


        [TestMethod]
        public void GenerateReportBackOfficeInvoiceDate()
        {
            GenerateReportHandoff(DateType.InvoiceDate);

            ManipulateReportHandoffRecords("DT:2015,9,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,9,5", "ENDDATE");

            InsertReportHandoff();

            //run the report
            var rpt = (Co2ExecSum)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            var airSummary = rpt.AirSummary.FirstOrDefault();
            Assert.AreEqual(56, airSummary.Invoices, "Incorrect number of Invoices");
            Assert.AreEqual(0, airSummary.Credits, "Incorrect number of Credits");
            Assert.AreEqual(56, airSummary.Trips, "Incorrect number of Trips");
            Assert.AreEqual(27264.76m, airSummary.Airchg, "Incorrect Air Charges");
            Assert.AreEqual(486.87m, Math.Round(airSummary.Avgairchg, 2), "Incorrect Average Air Charges");
             Assert.AreEqual(55173, airSummary.Airco2, "Incorrect Air CO2");
            Assert.AreEqual(22564, airSummary.Altrailco2, "Incorrect Rail CO2");
            Assert.AreEqual(59562, airSummary.Altcarco2, "Incorrect Car CO2");

            Assert.AreEqual(52, rpt.CityPair.Sum(s => s.Segments), "Incorrect # of segments");
            Assert.AreEqual(71701, rpt.CityPair.Sum(s => s.Miles), "Incorrect Miles");
            Assert.AreEqual(33064, rpt.CityPair.Sum(s => s.Airco2), "Incorrect Air CO2");

            var carSummary = rpt.CarSummary.FirstOrDefault();
            Assert.AreEqual(19, carSummary.Rentals, "Incorrect Car Rentals");
            Assert.AreEqual(37, carSummary.Days, "Incorrect Car Days");
            Assert.AreEqual(433.46m, carSummary.Carcost, "Incorrect Car Cost");
            Assert.AreEqual(1982, carSummary.Carco2, "Incorrect Car CO2");

            Assert.AreEqual(29, rpt.TopCar.Sum(s => s.Days), "Incorrect Car Days");
            Assert.AreEqual(1553, rpt.TopCar.Sum(s => Math.Round(s.Carco2)), "Incorrect Car CO2");

            var hotelSummary = rpt.HotelSummary.FirstOrDefault();
            Assert.AreEqual(24, hotelSummary.Bookings, "Incorrect Hotel Bookings");
            Assert.AreEqual(41, hotelSummary.Nights, "Incorrect Hotel Nights");
            Assert.AreEqual(1605.75m, hotelSummary.Hotelcost, "Incorrect Hotel Cost");
            Assert.AreEqual(2993, hotelSummary.Hotelco2, "Incorrect Hotel CO2");

            Assert.AreEqual(29, rpt.TopHotel.Sum(s => s.Nights), "Incorrect Hotel Nights");
            Assert.AreEqual(2117, rpt.TopHotel.Sum(s => s.Hotelco2), "Incorrect Hotel CO2");

            ClearReportHandoff();
        }

        [TestMethod]
        public void GenerateReportReservationDepartureDate()
        {
            GenerateReportHandoff(DateType.DepartureDate);
            ManipulateReportHandoffRecords("1", "PREPOST");
            ManipulateReportHandoffRecords("DT:2015,9,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,9,30", "ENDDATE");

            InsertReportHandoff();

            //run the report
            var rpt = (Co2ExecSum)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            var airSummary = rpt.AirSummary.FirstOrDefault();
            Assert.AreEqual(13, airSummary.Invoices, "Incorrect number of Invoices");
            Assert.AreEqual(0, airSummary.Credits, "Incorrect number of Credits");
            Assert.AreEqual(13, airSummary.Trips, "Incorrect number of Trips");
            Assert.AreEqual(32457.72m, airSummary.Airchg, "Incorrect Air Charges");
            Assert.AreEqual(2496.75m, Math.Round(airSummary.Avgairchg, 2), "Incorrect Average Air Charges");
            Assert.AreEqual(114009, airSummary.Airco2, "Incorrect Air CO2");
            Assert.AreEqual(9075, airSummary.Altrailco2, "Incorrect Rail CO2");
            Assert.AreEqual(78235, airSummary.Altcarco2, "Incorrect Car CO2");

            Assert.AreEqual(7, rpt.CityPair.Sum(s => s.Segments), "Incorrect # of segments");
            Assert.AreEqual(64008, rpt.CityPair.Sum(s => s.Miles), "Incorrect Miles");
            Assert.AreEqual(73403, rpt.CityPair.Sum(s => s.Airco2), "Incorrect Air CO2");

            var carSummary = rpt.CarSummary.FirstOrDefault();
            Assert.AreEqual(4, carSummary.Rentals, "Incorrect Car Rentals");
            Assert.AreEqual(21, carSummary.Days, "Incorrect Car Days");
            Assert.AreEqual(894.02m, carSummary.Carcost, "Incorrect Car Cost");
            Assert.AreEqual(1125, carSummary.Carco2, "Incorrect Car CO2");

            Assert.AreEqual(21, rpt.TopCar.Sum(s => s.Days), "Incorrect Car Days");
            Assert.AreEqual(1125, rpt.TopCar.Sum(s => Math.Round(s.Carco2)), "Incorrect Car CO2");

            var hotelSummary = rpt.HotelSummary.FirstOrDefault();
            Assert.AreEqual(13, hotelSummary.Bookings, "Incorrect Hotel Bookings");
            Assert.AreEqual(42, hotelSummary.Nights, "Incorrect Hotel Nights");
            Assert.AreEqual(9139.00m, hotelSummary.Hotelcost, "Incorrect Hotel Cost");
            Assert.AreEqual(3072, hotelSummary.Hotelco2, "Incorrect Hotel CO2");

            Assert.AreEqual(42, rpt.TopHotel.Sum(s => s.Nights), "Incorrect Hotel Nights");
            Assert.AreEqual(3072, rpt.TopHotel.Sum(s => s.Hotelco2), "Incorrect Hotel CO2");

            ClearReportHandoff();
        }


        [TestMethod]
        public void GenerateReportReservationInvoiceDate()
        {
            GenerateReportHandoff(DateType.InvoiceDate);
            ManipulateReportHandoffRecords("1", "PREPOST");
            ManipulateReportHandoffRecords("DT:2015,2,1", "BEGDATE");
            ManipulateReportHandoffRecords("DT:2015,2,28", "ENDDATE");

            InsertReportHandoff();

            //run the report
            var rpt = (Co2ExecSum)RunReport();
            var rptInfo = rpt.Globals.ReportInformation;

            //check for validity
            Assert.AreEqual(1, rptInfo.ReturnCode, "Return code failed.");

            var airSummary = rpt.AirSummary.FirstOrDefault();
            Assert.AreEqual(566, airSummary.Invoices, "Incorrect number of Invoices");
            Assert.AreEqual(0, airSummary.Credits, "Incorrect number of Credits");
            Assert.AreEqual(566, airSummary.Trips, "Incorrect number of Trips");
            Assert.AreEqual(605486.68m, airSummary.Airchg, "Incorrect Air Charges");
            Assert.AreEqual(1069.76m, Math.Round(airSummary.Avgairchg, 2), "Incorrect Average Air Charges");
            Assert.AreEqual(1043667, airSummary.Airco2, "Incorrect Air CO2");
            Assert.AreEqual(250774, airSummary.Altrailco2, "Incorrect Rail CO2");
            Assert.AreEqual(888057, airSummary.Altcarco2, "Incorrect Car CO2");

            Assert.AreEqual(26, rpt.CityPair.Sum(s => s.Segments), "Incorrect # of segments");
            Assert.AreEqual(160980, rpt.CityPair.Sum(s => s.Miles), "Incorrect Miles");
            Assert.AreEqual(155086, rpt.CityPair.Sum(s => s.Airco2), "Incorrect Air CO2");

            var carSummary = rpt.CarSummary.FirstOrDefault();
            Assert.AreEqual(196, carSummary.Rentals, "Incorrect Car Rentals");
            Assert.AreEqual(590, carSummary.Days, "Incorrect Car Days");
            Assert.AreEqual(42495.70m, carSummary.Carcost, "Incorrect Car Cost");
            Assert.AreEqual(33226, carSummary.Carco2, "Incorrect Car CO2");

            Assert.AreEqual(140, rpt.TopCar.Sum(s => s.Days), "Incorrect Car Days");
            Assert.AreEqual(8403, rpt.TopCar.Sum(s => Math.Round(s.Carco2)), "Incorrect Car CO2");

            var hotelSummary = rpt.HotelSummary.FirstOrDefault();
            Assert.AreEqual(437, hotelSummary.Bookings, "Incorrect Hotel Bookings");
            Assert.AreEqual(987, hotelSummary.Nights, "Incorrect Hotel Nights");
            Assert.AreEqual(154942.82m, hotelSummary.Hotelcost, "Incorrect Hotel Cost");
            Assert.AreEqual(72122, hotelSummary.Hotelco2, "Incorrect Hotel CO2");

            Assert.AreEqual(272, rpt.TopHotel.Sum(s => s.Nights), "Incorrect Hotel Nights");
            Assert.AreEqual(19878, rpt.TopHotel.Sum(s => s.Hotelco2), "Incorrect Hotel CO2");

            ClearReportHandoff();
        }

        public void GenerateReportHandoff(DateType dateType)
        {
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "AGENCY", ParmValue = "DEMO", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "BEGDATE", ParmValue = "DT:2015,9,1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "CARBONCALC", ParmValue = "CISCARBON", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "country", ParmValue = "United States", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "DATERANGE", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "ENDDATE", ParmValue = "DT:2015,9,5", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "INSOURCEABBR", ParmValue = "DEMOCA01", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "MONEYTYPE", ParmValue = "USD", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTDEST", ParmValue = "1", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "OUTPUTTYPE", ParmValue = "4", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PREPOST", ParmValue = "2", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROCESSID", ParmValue = "221", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "PROGRAM", ParmValue = "ibCO2ExecSum", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTINGTRAVET", ParmValue = "YES", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTLOGKEY", ParmValue = "3393256", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "REPORTSTATUS", ParmValue = "DONE", ParmInOut = "IN", LangCode = "EN" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "RPTTITLE2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "TITLEACCT2", ParmValue = "", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "UDRKEY", ParmValue = "0", ParmInOut = "IN", LangCode = "" });
            ReportHandoff.Add(new ReportHandoffInformation { ParmName = "USERNBR", ParmValue = "1591", ParmInOut = "IN", LangCode = "" });

            ManipulateReportHandoffRecords(((int)dateType).ToString(), "DATERANGE");

        }
    }
}
