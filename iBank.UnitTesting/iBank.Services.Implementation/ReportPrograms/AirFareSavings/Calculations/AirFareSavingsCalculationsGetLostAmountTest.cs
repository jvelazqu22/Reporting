using iBank.Services.Implementation.ReportPrograms.AirFareSavings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.AirFareSavings.Calculations
{
    [TestClass]
    public class AirFareSavingsCalculationsGetLostAmountTest
    {
        // Unit test naming convention:
        // [UnitOfWork_StateUnderTest_ExpectedBehavior]
        // Example: public void Sun_NegativeNumberAsFirstParam_ExceptionThrown

        [TestMethod]
        public void GetLostAmount_UseBaseFareAndBaseFareGreaterThanOffRdChgLostAmtGreaterThanZeroPlusMinGreaterThanZero_ReturnBaseFareMinusOffRdChg()
        {
            // Arrange
            var airFareSavingCalculations = new AirFareSavingsCalculations();
            decimal airChg = 2, offRdChg = 3, baseFare = 4;
            bool useBaseFare = true;
            int plusMin = 1;
            decimal result = 0;

            // Act
            result = airFareSavingCalculations.GetLostAmount(airChg, offRdChg, plusMin, baseFare, useBaseFare);

            // Assert
            Assert.AreEqual(baseFare - offRdChg, result);
        }

        [TestMethod]
        public void GetLostAmount_UseBaseFareAndBaseFareLessThanOffRdChgLostAmtGreaterThanZeroPlusMinGreaterThanZero_ReturnZero()
        {
            // Arrange
            var airFareSavingCalculations = new AirFareSavingsCalculations();
            decimal airChg = 2, offRdChg = 5, baseFare = 4;
            bool useBaseFare = true;
            int plusMin = 1;
            decimal result = 0;

            // Act
            result = airFareSavingCalculations.GetLostAmount(airChg, offRdChg, plusMin, baseFare, useBaseFare);

            // Assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void GetLostAmount_DoNotUseBaseFareLostAmtGreaterThanZeroPlusMinGreaterThanZero_ReturnAirCchMinusOffRdChg()
        {
            // Arrange
            var airFareSavingCalculations = new AirFareSavingsCalculations();
            decimal airChg = 2, offRdChg = 1, baseFare = 4;
            bool useBaseFare = false;
            int plusMin = 1;
            decimal result = 0;

            // Act
            result = airFareSavingCalculations.GetLostAmount(airChg, offRdChg, plusMin, baseFare, useBaseFare);

            // Assert
            Assert.AreEqual(airChg - offRdChg, result);
        }

        [TestMethod]
        public void GetLostAmount_LostAmtLessThanZeroPlusMinGreaterThanZero_ReturnZero()
        {
            // Arrange
            var airFareSavingCalculations = new AirFareSavingsCalculations();
            decimal airChg = 2, offRdChg = 3, baseFare = 4;
            bool useBaseFare = false;
            int plusMin = 1;
            decimal result = 0;

            // Act
            result = airFareSavingCalculations.GetLostAmount(airChg, offRdChg, plusMin, baseFare, useBaseFare);

            // Assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void GetLostAmount_LostAmtGreaterThanZeroPlusMinLessThanZero_ReturnZero()
        {
            // Arrange
            var airFareSavingCalculations = new AirFareSavingsCalculations();
            decimal airChg = 2, offRdChg = 1, baseFare = 4;
            bool useBaseFare = false;
            int plusMin = -1;
            decimal result = 0;

            // Act
            result = airFareSavingCalculations.GetLostAmount(airChg, offRdChg, plusMin, baseFare, useBaseFare);

            // Assert
            Assert.AreEqual(0, result);
        }
    }
}
