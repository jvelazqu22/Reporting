using iBank.Services.Implementation.ReportPrograms.AirFareSavings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.AirFareSavings.Calculations
{
    [TestClass]
    public class AirFareSavingsCalculationsGetNegotiatedSavings
    {
        // Unit test naming convention:
        // [UnitOfWork_StateUnderTest_ExpectedBehavior]
        // Example: public void Sun_NegativeNumberAsFirstParam_ExceptionThrown

        [TestMethod]
        public void GetNegotiatedSavings_UseBaseFareLostAmtLessThanZeroAndPlusMinGreaterThanZero_LostAmtEqualBaseFareMinusOffRdcChReturnZeroMinutsBaseFareMinusOffRdChg()
        {
            // Arrange
            var airFareSavingCalculations = new AirFareSavingsCalculations();
            decimal airChg = 2, offRdChg = 3, baseFare = 1;
            int plusmin = 1;
            bool useBaseFare = true;
            decimal result = 0;

            // Act
            result = airFareSavingCalculations.GetNegotiatedSavings(airChg, offRdChg, plusmin, baseFare, useBaseFare);

            // Assert
            Assert.AreEqual((0 - (baseFare - offRdChg)), result);
        }

        [TestMethod]
        public void GetNegotiatedSavings_UseBaseFareLostAmtLessThanZeroAndPlusMinLessThanZero_LostAmtEqualBaseFareMinusOffRdcChReturnZero()
        {
            // Arrange
            var airFareSavingCalculations = new AirFareSavingsCalculations();
            decimal airChg = 2, offRdChg = 3, baseFare = 1;
            int plusmin = -1;
            bool useBaseFare = true;
            decimal result = 0;

            // Act
            result = airFareSavingCalculations.GetNegotiatedSavings(airChg, offRdChg, plusmin, baseFare, useBaseFare);

            // Assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void GetNegotiatedSavings_UseBaseFareLostAmtGreaterThanZeroAndPlusMinLessThanZero_LostAmtEqualBaseFareMinusOffRdcChReturnZeroMinutsBaseFareMinusOffRdChg()
        {
            // Arrange
            var airFareSavingCalculations = new AirFareSavingsCalculations();
            decimal airChg = 2, offRdChg = 3, baseFare = 4;
            int plusmin = -1;
            bool useBaseFare = true;
            decimal result = 0;

            // Act
            result = airFareSavingCalculations.GetNegotiatedSavings(airChg, offRdChg, plusmin, baseFare, useBaseFare);

            // Assert
            Assert.AreEqual((0 - (baseFare - offRdChg)), result);
        }

        [TestMethod]
        public void GetNegotiatedSavings_UseBaseFareLostAmtGreaterThanZeroAndPlusMinGreaterThanZero_LostAmtEqualBaseFareMinusOffRdcChReturnZero()
        {
            // Arrange
            var airFareSavingCalculations = new AirFareSavingsCalculations();
            decimal airChg = 2, offRdChg = 3, baseFare = 4;
            int plusmin = 1;
            bool useBaseFare = true;
            decimal result = 0;

            // Act
            result = airFareSavingCalculations.GetNegotiatedSavings(airChg, offRdChg, plusmin, baseFare, useBaseFare);

            // Assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void GetNegotiatedSavings_DoNotUseBaseFareLostAmtLessThanZeroAndPlusMinGreaterThanZero_LostAmtEqualAirChgMinusOffRdcChReturnZeroMinutsAirChgMinusOffRdChg()
        {
            // Arrange
            var airFareSavingCalculations = new AirFareSavingsCalculations();
            decimal airChg = 2, offRdChg = 3, baseFare = 1;
            int plusmin = 1;
            bool useBaseFare = false;
            decimal result = 0;

            // Act
            result = airFareSavingCalculations.GetNegotiatedSavings(airChg, offRdChg, plusmin, baseFare, useBaseFare);

            // Assert
            Assert.AreEqual(result, (0 - (airChg - offRdChg)));
        }

        [TestMethod]
        public void GetNegotiatedSavings_DoNotUseBaseFareLostAmtGreaterThanZeroAndPlusMinLessThanZero_LostAmtEqualAirChgMinusOffRdcChReturnZeroMinutsAirMinusOffRdChg()
        {
            // Arrange
            var airFareSavingCalculations = new AirFareSavingsCalculations();
            decimal airChg = 5, offRdChg = 3, baseFare = 4;
            int plusmin = -1;
            bool useBaseFare = false;
            decimal result = 0;

            // Act
            result = airFareSavingCalculations.GetNegotiatedSavings(airChg, offRdChg, plusmin, baseFare, useBaseFare);

            // Assert
            Assert.AreEqual((0 - (airChg - offRdChg)), result);
        }

    }
}
