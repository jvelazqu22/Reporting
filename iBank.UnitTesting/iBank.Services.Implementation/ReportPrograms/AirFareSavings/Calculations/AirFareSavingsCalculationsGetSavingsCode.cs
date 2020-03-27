using iBank.Services.Implementation.ReportPrograms.AirFareSavings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.AirFareSavings.Calculations
{
    [TestClass]
    public class AirFareSavingsCalculationsGetSavingsCode
    {
        // Unit test naming convention:
        // [UnitOfWork_StateUnderTest_ExpectedBehavior]
        // Example: public void Sun_NegativeNumberAsFirstParam_ExceptionThrown

        [TestMethod]
        public void GetSavingsCode_DeriveSvgCodeIsTrueSavingsCodeIsEmptyAndReasonCodeIsEmptyAndLostAmtGreaterThanZeroAndSavingsGreaterThanZero_ReturnSavingsCode()
        {
            // Arrange
            var airFareSavingCalculations = new AirFareSavingsCalculations();
            decimal lostAmt = 1, savings = 2;
            bool deriveSvgCode = true;
            string savingsCode = string.Empty, reasonCode = string.Empty;
            string result = string.Empty;

            // Act
            result = airFareSavingCalculations.GetSavingsCode(savingsCode, deriveSvgCode, reasonCode, lostAmt, savings);

            // Assert
            Assert.AreEqual(savingsCode, result);
        }

        [TestMethod]
        public void GetSavingsCode_DeriveSvgCodeIsTrueSavingsCodeIsNotEmpty_ReturnSavingsCode()
        {
            // Arrange
            var airFareSavingCalculations = new AirFareSavingsCalculations();
            decimal lostAmt = 1, savings = 2;
            bool deriveSvgCode = true;
            string savingsCode = "some code", reasonCode = string.Empty;
            string result = string.Empty;

            // Act
            result = airFareSavingCalculations.GetSavingsCode(savingsCode, deriveSvgCode, reasonCode, lostAmt, savings);

            // Assert
            Assert.AreEqual(savingsCode, result);
        }

        [TestMethod]
        public void GetSavingsCode_DeriveSvgCodeIsTrueSavingsCodeIsNotEmptyReasonCodeIsEmtpy_ReturnSavingsCode()
        {
            // Arrange
            var airFareSavingCalculations = new AirFareSavingsCalculations();
            decimal lostAmt = 1, savings = 2;
            bool deriveSvgCode = true;
            string savingsCode = "some code", reasonCode = "second code";
            string result = string.Empty;

            // Act
            result = airFareSavingCalculations.GetSavingsCode(savingsCode, deriveSvgCode, reasonCode, lostAmt, savings);

            // Assert
            Assert.AreEqual(savingsCode, result);
        }

        [TestMethod]
        public void GetSavingsCode_DeriveSvgCodeIsFalseSavingsCodeIsEmpty_ReturnStringWithTwoWhiteSpaces()
        {
            // Arrange
            var airFareSavingCalculations = new AirFareSavingsCalculations();
            decimal lostAmt = 1, savings = 2;
            bool deriveSvgCode = false;
            string savingsCode = string.Empty, reasonCode = string.Empty;
            string result = string.Empty;

            // Act
            result = airFareSavingCalculations.GetSavingsCode(savingsCode, deriveSvgCode, reasonCode, lostAmt, savings);

            // Assert
            Assert.AreEqual("  ", result);
        }

        [TestMethod]
        public void GetSavingsCode_DeriveSvgCodeIsFalseSavingsCodeIsNotEmpty_ReturnPassedSavingsCode()
        {
            // Arrange
            var airFareSavingCalculations = new AirFareSavingsCalculations();
            decimal lostAmt = 1, savings = 2;
            bool deriveSvgCode = false;
            string savingsCode = "some test", reasonCode = string.Empty;
            string result = string.Empty;

            // Act
            result = airFareSavingCalculations.GetSavingsCode(savingsCode, deriveSvgCode, reasonCode, lostAmt, savings);

            // Assert
            Assert.AreEqual(savingsCode, result);
        }
    }
}
