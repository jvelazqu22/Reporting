using System;

using iBank.OverdueBroadcastMonitor;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.OverdueBroadcastMonitor
{
    [TestClass]
    public class ThresholdCalculatorTests
    {
        [TestMethod]
        public void CalculateThreshold()
        {
            var baseDate = new DateTime(2016, 1, 1, 9, 00, 00);
            var thresholdInMinutes = 30;

            var output = ThresholdCalculator.CalculateThreshold(baseDate, thresholdInMinutes);

            Assert.AreEqual(new DateTime(2016, 1, 1, 8, 30, 00), output);
        }
    }
}
