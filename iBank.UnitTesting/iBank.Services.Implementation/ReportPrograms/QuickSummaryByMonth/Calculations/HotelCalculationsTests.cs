using System.Collections.Generic;

using iBank.Services.Implementation.ReportPrograms.QuickSummaryByMonth.Calculations;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.QuickSummaryByMonth.Calculations
{
    [TestClass]
    public class HotelCalculationsTests
    {
        [TestMethod]
        public void CalculateHotelExcepts_EmptyReasCodh_Return0()
        {
            var calc = new HotelCalculations();
            var reasCodh = "";
            var reasExclude = new List<string>();
            var hplusMin = 0;

            var output = calc.CalculateHotelExcepts(reasCodh, reasExclude, hplusMin);

            Assert.AreEqual(0, output);
        }

        [TestMethod]
        public void CalculateHotelExcepts_NonEmptyReasCodhContainedInReasExclude_Return0()
        {
            var calc = new HotelCalculations();
            var reasCodh = "foo";
            var reasExclude = new List<string> { "foo" };
            var hplusMin = 0;

            var output = calc.CalculateHotelExcepts(reasCodh, reasExclude, hplusMin);

            Assert.AreEqual(0, output);
        }

        [TestMethod]
        public void CalculateHotelExcepts_NonEmptyReasCodhNotContainedInReasExclude_ReturnHPlusMin()
        {
            var calc = new HotelCalculations();
            var reasCodh = "foo";
            var reasExclude = new List<string> { "bar" };
            var hplusMin = 1;

            var output = calc.CalculateHotelExcepts(reasCodh, reasExclude, hplusMin);

            Assert.AreEqual(1, output);
        }

        [TestMethod]
        public void CalculateHotelLost_EmptyReasCodh_Return0()
        {
            var calc = new HotelCalculations();
            var reasCodh = "";
            var reasExclude = new List<string>();
            var bookrat = 1M;
            var hexcprat = 2M;
            var nights = 3;
            var rooms = 4;

            var output = calc.CalculateHotelLost(reasCodh, reasExclude, bookrat, hexcprat, nights, rooms);

            Assert.AreEqual(0M, output);
        }

        [TestMethod]
        public void CalculateHotelLost_NonEmptyReasCodhContainedInReasExclude_Return0()
        {
            var calc = new HotelCalculations();
            var reasCodh = "foo";
            var reasExclude = new List<string> { "foo" };
            var bookrat = 1M;
            var hexcprat = 2M;
            var nights = 3;
            var rooms = 4;

            var output = calc.CalculateHotelLost(reasCodh, reasExclude, bookrat, hexcprat, nights, rooms);

            Assert.AreEqual(0M, output);
        }

        [TestMethod]
        public void CalculateHotelLost_NonEmptyReasCodaNotContainedInReasExclude_ReturnDifferenceOfBookRateAndHexCPratTimesNightsTimesRooms()
        {
            var calc = new HotelCalculations();
            var reasCodh = "foo";
            var reasExclude = new List<string> { "bar" };
            var bookrat = 5M;
            var hexcprat = 3M;
            var nights = 4;
            var rooms = 5;

            var output = calc.CalculateHotelLost(reasCodh, reasExclude, bookrat, hexcprat, nights, rooms);

            Assert.AreEqual(40M, output);
        }
    }
}
