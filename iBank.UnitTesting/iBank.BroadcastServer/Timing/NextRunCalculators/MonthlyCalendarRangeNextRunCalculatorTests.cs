using System;

using Domain.Constants;
using Domain.Interfaces.BroadcastServer;
using iBank.BroadcastServer.Timing.NextRunCalculators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace iBank.UnitTesting.iBank.BroadcastServer.Timing.NextRunCalculators
{
    [TestClass]
    public class MonthlyCalendarRangeNextRunCalculatorTests
    {
        [TestMethod]
        public void CalculateNextRun_BiMonthlyFifteenthOfMonth()
        {
            var dateToBaseIncrementOff = new DateTime(2017, 1, 15);
            var dayOfMonthToRunReport = 15;
            var sut = new MonthlyCalendarRangeNextRunCalculator(dateToBaseIncrementOff, dayOfMonthToRunReport, CalendarRange.BiMonthly);

            var output = sut.CalculateNextRun();

            Assert.AreEqual(new DateTime(2017, 3, 15), output);
        }

        [TestMethod]
        public void CalculateNextRun_BiMonthlyEndOfMonth()
        {
            var dateToBaseIncrementOff = new DateTime(2017, 2, 28);
            var dayOfMonthToRunReport = 31;
            var sut = new MonthlyCalendarRangeNextRunCalculator(dateToBaseIncrementOff, dayOfMonthToRunReport, CalendarRange.BiMonthly);

            var output = sut.CalculateNextRun();

            Assert.AreEqual(new DateTime(2017, 4, 30), output);
        }

        [TestMethod]
        public void CalculateNextRun_RolloverYear()
        {
            var dateToBaseIncrementOff = new DateTime(2016, 12, 15);
            var dayOfMonthToRunReport = 15;
            var sut = new MonthlyCalendarRangeNextRunCalculator(dateToBaseIncrementOff, dayOfMonthToRunReport, CalendarRange.BiMonthly);

            var output = sut.CalculateNextRun();

            Assert.AreEqual(new DateTime(2017, 2, 15), output);
        }
    }
}
