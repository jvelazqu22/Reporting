using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iBank.Services.Implementation.ReportPrograms.QuickSummaryByMonth;
using iBank.Services.Implementation.ReportPrograms.QuickSummaryByMonth.Calculations;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.QuickSummaryByMonth.Calculations
{
    [TestClass]
    public class AirDataCalculationsTests
    {
        [TestMethod]
        public void CalculateOfferedCharge_OfferedChargeGreaterThanZeroAirChargeLessThanZero_OutputZeroMinusOfferedCharge()
        {
            var offeredCharge = 1;
            var airCharge = -5;
            var calculations = new AirDataCalculations();
            var expectedOutput = 0 - offeredCharge;

            var actual = calculations.CalculateOfferedCharge(offeredCharge, airCharge);

            Assert.AreEqual(expectedOutput, actual);
        }
        [TestMethod]
        public void CalculateOfferedCharge_OfferedChargeEqualZero_OutputAirCharge()
        {
            var offeredCharge = 0;
            var airCharge = -5;
            var calculations = new AirDataCalculations();

            var actual = calculations.CalculateOfferedCharge(offeredCharge, airCharge);

            Assert.AreEqual(airCharge, actual);
        }

        [TestMethod]
        public void CalculateOfferedCharge_OfferedChargeAndAirChargeLessThanZero_OutputOfferedCharge()
        {
            var offeredCharge = -10;
            var airCharge = -5;
            var calculations = new AirDataCalculations();

            var actual = calculations.CalculateOfferedCharge(offeredCharge, airCharge);

            Assert.AreEqual(offeredCharge, actual);
        }

        [TestMethod]
        public void CalculateStandardCharge_StandardChargeLessThanAirCharge_OutputAirCharge()
        {
            var standardCharge = 1;
            var airCharge = 5;

            var calculations = new AirDataCalculations();

            var actualCharge = calculations.CalculateStandardCharge(standardCharge, airCharge);

            Assert.AreEqual(airCharge, actualCharge);
        }

        [TestMethod]
        public void CalculateStandardCharge_StandardChargeEqualZero_OutputAirCharge()
        {
            var standardCharge = 0;
            var airCharge = 5;

            var calculations = new AirDataCalculations();

            var actualCharge = calculations.CalculateStandardCharge(standardCharge, airCharge);

            Assert.AreEqual(airCharge, actualCharge);
        }

        [TestMethod]
        public void CalculateStandardCharge_StandardChargeGreaterThanZeroAndAirChargeLessThanZero_OutputAirCharge()
        {
            var standardCharge = 5;
            var airCharge = -1;

            var calculations = new AirDataCalculations();

            var actualCharge = calculations.CalculateStandardCharge(standardCharge, airCharge);

            Assert.AreEqual(airCharge, actualCharge);
        }

        [TestMethod]
        public void CalculateStandardCharge_BothChargesNegativeAbsoluteValueOfAirChargeIsGreaterThan_OutputAirCharge()
        {
            var standardCharge = -1;
            var airCharge = -5;

            var calculations = new AirDataCalculations();

            var actualCharge = calculations.CalculateStandardCharge(standardCharge, airCharge);

            Assert.AreEqual(airCharge, actualCharge);
        }

        [TestMethod]
        public void CalculateStandardCharge_AllConditionalsFalse_OutputStandardCharge()
        {
            var standardCharge = 5;
            var airCharge = 1;

            var calculations = new AirDataCalculations();

            var actualCharge = calculations.CalculateStandardCharge(standardCharge, airCharge);

            Assert.AreEqual(standardCharge, actualCharge);
        }

        [TestMethod]
        public void CalculateLostAmount()
        {
            var airCharge = 7.5M;
            var offeredCharge = 2M;
            var expectedOutput = 5.5M;

            var calc = new AirDataCalculations();
            var actualOuput = calc.CalculateLostAmount(airCharge, offeredCharge);

            Assert.AreEqual(expectedOutput, actualOuput);
        }

        [TestMethod]
        public void CalculateSavings()
        {
            var airCharge = 2M;
            var standardCharge = 7.5M;
            var expectedOutput = 5.5M;

            var calc = new AirDataCalculations();
            var actualOutput = calc.CalculateSavings(airCharge, standardCharge);

            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [TestMethod]
        public void UpdateReasCode_NullReasCode_ReturnsEmptyString()
        {
            string reasCode = null;
            var reasExclude = new List<string>();

            var calc = new AirDataCalculations();
            var actualOutput = calc.UpdateReasCode(reasCode, reasExclude);

            Assert.AreEqual("", actualOutput);
        }

        [TestMethod]
        public void UpdateReasCode_ReasCodeInReasExclude_ReturnsEmptyString()
        {
            string reasCode = "foo";
            var reasExclude = new List<string> { "foo" };

            var calc = new AirDataCalculations();
            var actualOutput = calc.UpdateReasCode(reasCode, reasExclude);

            Assert.AreEqual("", actualOutput);
        }

        [TestMethod]
        public void UpdateReasCode_ReasCodeNotEmptyNotInReasExclude_ReturnsReasCode()
        {
            string reasCode = "foo";
            var reasExclude = new List<string> { "bar" };

            var calc = new AirDataCalculations();
            var actualOutput = calc.UpdateReasCode(reasCode, reasExclude);

            Assert.AreEqual("foo", actualOutput);
        }

        [TestMethod]
        public void IsOkToUpdateSavingCode_NullSavingCodeNotNullReasCodeZeroLostAmountGreaterThanZeroSavings_ReturnTrue()
        {
            string savingCode = null;
            var reasCode = "foo";
            var lostAmount = 0;
            var savings = 1;

            var calc = new AirDataCalculations();
            var actualOutput = calc.IsOkToUpdateSavingCode(savingCode, reasCode, lostAmount, savings);

            Assert.AreEqual(true, actualOutput);
        }

        [TestMethod]
        public void IsOkToUpdateSavingCode_EmptyNullSavingCodeNotNullReasCodeZeroLostAmountGreaterThanZeroSavings_ReturnTrue()
        {
            string savingCode = "";
            var reasCode = "foo";
            var lostAmount = 0;
            var savings = 1;

            var calc = new AirDataCalculations();
            var actualOutput = calc.IsOkToUpdateSavingCode(savingCode, reasCode, lostAmount, savings);

            Assert.AreEqual(true, actualOutput);
        }

        [TestMethod]
        public void IsOkToUpdateSavingCode_NotNullSavingCodeNotNullReasCodeZeroLostAmountGreaterThanZeroSavings_ReturnFalse()
        {
            string savingCode = "foo";
            var reasCode = "foo";
            var lostAmount = 0;
            var savings = 1;

            var calc = new AirDataCalculations();
            var actualOutput = calc.IsOkToUpdateSavingCode(savingCode, reasCode, lostAmount, savings);

            Assert.AreEqual(false, actualOutput);
        }

        [TestMethod]
        public void IsOkToUpdateSavingCode_EmptySavingCodeNullReasCodeZeroLostAmountGreaterThanZeroSavings_ReturnFalse()
        {
            string savingCode = "";
            string reasCode = null;
            var lostAmount = 0;
            var savings = 1;

            var calc = new AirDataCalculations();
            var actualOutput = calc.IsOkToUpdateSavingCode(savingCode, reasCode, lostAmount, savings);

            Assert.AreEqual(false, actualOutput);
        }

        [TestMethod]
        public void IsOkToUpdateSavingCode_EmptySavingCodeNotNullReasCodeNonZeroLostAmountGreaterThanZeroSavings_ReturnFalse()
        {
            string savingCode = "";
            var reasCode = "foo";
            var lostAmount = 1;
            var savings = 1;

            var calc = new AirDataCalculations();
            var actualOutput = calc.IsOkToUpdateSavingCode(savingCode, reasCode, lostAmount, savings);

            Assert.AreEqual(false, actualOutput);
        }

        [TestMethod]
        public void IsOkToUpdateSavingCode_EmptySavingCodeNotNullReasCodeZeroLostAmountLessThanZeroSavings_ReturnFalse()
        {
            string savingCode = "";
            var reasCode = "foo";
            var lostAmount = 0;
            var savings = -1;

            var calc = new AirDataCalculations();
            var actualOutput = calc.IsOkToUpdateSavingCode(savingCode, reasCode, lostAmount, savings);

            Assert.AreEqual(false, actualOutput);
        }
    }
}
