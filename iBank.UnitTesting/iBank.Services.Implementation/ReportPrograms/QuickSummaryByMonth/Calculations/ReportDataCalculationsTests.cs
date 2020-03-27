using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iBank.Services.Implementation.ReportPrograms.QuickSummaryByMonth.Calculations;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.QuickSummaryByMonth.Calculations
{
    [TestClass]
    public class ReportDataCalculationsTests
    {
        [TestMethod]
        public void NeedToSumAirExcepts_OfferedChargeNonZeroLostAmountNonZero_ReturnTrue()
        {
            var offeredCharge = 1;
            var lostAmount = 1;

            var calc = new ReportDataCalculations();
            var actualOutput = calc.NeedToSumAirExcepts(offeredCharge, lostAmount);

            Assert.AreEqual(true, actualOutput);
        }

        [TestMethod]
        public void NeedToSumAirExcepts_OfferedChargeZeroLostAmountNonZero_ReturnFalse()
        {
            var offeredCharge = 0;
            var lostAmount = 1;

            var calc = new ReportDataCalculations();
            var actualOutput = calc.NeedToSumAirExcepts(offeredCharge, lostAmount);

            Assert.AreEqual(false, actualOutput);
        }

        [TestMethod]
        public void NeedToSumAirExcepts_OfferedChargeNonZeroLostAmountZero_ReturnFalse()
        {
            var offeredCharge = 1;
            var lostAmount = 0;

            var calc = new ReportDataCalculations();
            var actualOutput = calc.NeedToSumAirExcepts(offeredCharge, lostAmount);

            Assert.AreEqual(false, actualOutput);
        }
    }
}
