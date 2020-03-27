using iBank.BroadcastServer.Timing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace iBank.UnitTesting.iBank.BroadcastServer.Timing
{
    [TestClass]
    public class TimingCalculatorTests
    {
        [TestMethod]
        public void GetSecondsFromHours_PositiveValue()
        {
            var hours = 2;
            var calc = new TimingCalculator();
            var expectedSeconds = 7200;

            var output = calc.GetSecondsFromHours(hours);

            Assert.AreEqual(expectedSeconds, output);
        }

        [TestMethod]
        public void GetSecondsFromHours_NegativeValue()
        {
            var hours = -2;
            var calc = new TimingCalculator();
            var expectedSeconds = 7200;

            var output = calc.GetSecondsFromHours(hours);

            Assert.AreEqual(expectedSeconds, output);
        }

        [TestMethod]
        public void GetsecondsFromMinutes_PositiveValue()
        {
            var minutes = 2;
            var calc = new TimingCalculator();
            var expectedSeconds = 120;

            var output = calc.GetSecondsFromMinutes(minutes);

            Assert.AreEqual(expectedSeconds, output);
        }

        [TestMethod]
        public void GetsecondsFromMinutes_NegativeValue()
        {
            var minutes = -2;
            var calc = new TimingCalculator();
            var expectedSeconds = 120;

            var output = calc.GetSecondsFromMinutes(minutes);

            Assert.AreEqual(expectedSeconds, output);
        }
        

        [TestMethod]
        public void GetValidDate_MonthWithThirtyOneDays_ValidDay()
        {
            var calc = new TimingCalculator();

            var year = 2016;
            var month = 1;
            var day = 1;

            var expected = new DateTime(2016, 1, 1);

            var output = calc.GetValidDate(year, month, day);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void GetValidDate_MonthWithThirtyOneDays_InValidDay()
        {
            var calc = new TimingCalculator();

            var year = 2016;
            var month = 1;
            var day = 100;

            var expected = new DateTime(2016, 1, 31);

            var output = calc.GetValidDate(year, month, day);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void GetValidDate_MonthWithThirtyDays_ValidDay()
        {
            var calc = new TimingCalculator();

            var year = 2016;
            var month = 4;
            var day = 1;

            var expected = new DateTime(2016, 4, 1);

            var output = calc.GetValidDate(year, month, day);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void GetValidDate_MonthWithThirtyDays_InValidDay()
        {
            var calc = new TimingCalculator();

            var year = 2016;
            var month = 4;
            var day = 100;

            var expected = new DateTime(2016, 4, 30);

            var output = calc.GetValidDate(year, month, day);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void GetValidDate_February_ValidDay()
        {
            var calc = new TimingCalculator();

            var year = 2016;
            var month = 2;
            var day = 1;

            var expected = new DateTime(2016, 2, 1);

            var output = calc.GetValidDate(year, month, day);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void GetValidDate_February_InvalidDay()
        {
            var calc = new TimingCalculator();

            var year = 2016;
            var month = 2;
            var day = 100;

            var expected = new DateTime(2016, 2, 28);

            var output = calc.GetValidDate(year, month, day);

            Assert.AreEqual(expected, output);
        }
    }
}
