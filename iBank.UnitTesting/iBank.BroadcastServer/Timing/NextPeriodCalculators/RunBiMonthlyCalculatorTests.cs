using System;

using Domain.Interfaces.BroadcastServer;

using iBank.BroadcastServer.Timing.NextPeriodCalculators;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace iBank.UnitTesting.iBank.BroadcastServer.Timing.NextPeriodCalculators
{
    [TestClass]
    public class RunBiMonthlyCalculatorTests
    {
        [TestMethod]
        public void CalculateNextReportPeriod_SameYear()
        {
            var timing = new Mock<IRecordTimingDetails>();
            timing.Setup(x => x.NextReportPeriodStart).Returns(new DateTime(2017, 2, 1));
            var sut = new RunBiMonthlyCalculator(timing.Object);

            var output = sut.CalculateNextReportPeriod();

            Assert.AreEqual(new DateTime(2017, 4, 1), output.ReportPeriodStart);
            Assert.AreEqual(new DateTime(2017, 5, 31), output.ReportPeriodEnd);
        }

        [TestMethod]
        public void CalculateNextReportPeriod_YearRollsover()
        {
            var timing = new Mock<IRecordTimingDetails>();
            timing.Setup(x => x.NextReportPeriodStart).Returns(new DateTime(2016, 12, 1));
            var sut = new RunBiMonthlyCalculator(timing.Object);

            var output = sut.CalculateNextReportPeriod();

            Assert.AreEqual(new DateTime(2017, 2, 1), output.ReportPeriodStart);
            Assert.AreEqual(new DateTime(2017, 3, 31), output.ReportPeriodEnd);
        }
    }
}
