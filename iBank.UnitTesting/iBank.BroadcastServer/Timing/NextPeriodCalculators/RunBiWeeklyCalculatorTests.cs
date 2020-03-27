using System;

using Domain.Interfaces.BroadcastServer;

using iBank.BroadcastServer.Timing.NextPeriodCalculators;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace iBank.UnitTesting.iBank.BroadcastServer.Timing.NextPeriodCalculators
{
    [TestClass]
    public class RunBiWeeklyCalculatorTests
    {
        [TestMethod]
        public void CalculateNextReportPeriod_SameYear()
        {
            var timing = new Mock<IRecordTimingDetails>();
            timing.Setup(x => x.NextReportPeriodStart).Returns(new DateTime(2017, 1, 23));
            timing.Setup(x => x.NextReportPeriodEnd).Returns(new DateTime(2017, 2, 5));
            var sut = new RunBiWeeklyCalculator(timing.Object);

            var output = sut.CalculateNextReportPeriod();

            Assert.AreEqual(new DateTime(2017, 2, 6), output.ReportPeriodStart);
            Assert.AreEqual(new DateTime(2017, 2, 19), output.ReportPeriodEnd);
        }

        [TestMethod]
        public void CalculateNextReportPeriod_RolloverYear()
        {
            var timing = new Mock<IRecordTimingDetails>();
            timing.Setup(x => x.NextReportPeriodStart).Returns(new DateTime(2016, 12, 12));
            timing.Setup(x => x.NextReportPeriodEnd).Returns(new DateTime(2016, 12, 25));
            var sut = new RunBiWeeklyCalculator(timing.Object);

            var output = sut.CalculateNextReportPeriod();

            Assert.AreEqual(new DateTime(2016, 12, 26), output.ReportPeriodStart);
            Assert.AreEqual(new DateTime(2017, 1, 8), output.ReportPeriodEnd);
        }
    }
}
