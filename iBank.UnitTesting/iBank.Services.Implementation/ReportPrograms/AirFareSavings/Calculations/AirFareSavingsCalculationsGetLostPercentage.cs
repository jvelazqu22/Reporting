using iBank.Services.Implementation.ReportPrograms.AirFareSavings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.AirFareSavings.Calculations
{
    [TestClass]
    public class AirFareSavingsCalculationsGetLostPercentage
    {
        // Unit test naming convention:
        // [UnitOfWork_StateUnderTest_ExpectedBehavior]
        // Example: public void Sun_NegativeNumberAsFirstParam_ExceptionThrown

        [TestMethod]
        public void GetLossPercentage_UseBaseFareAirChangeAndOffRecordChangeNotZero_ReturnBaseFareMinusOffRdChangeDivByBaseFare()
        {
            // Arrange
            var airFareSavingCalculations = new AirFareSavingsCalculations();
            decimal airChg = 2, offRcdChng = 3, baseFare = 4;
            bool useBaseFare = true;
            decimal result = 0;

            // Act
            result = airFareSavingCalculations.GetLossPercentage(airChg, offRcdChng, baseFare, useBaseFare);

            // Assert
            Assert.AreEqual(Decimal.Round(100 * ((baseFare - offRcdChng) / baseFare), 2), result);
        }


        [TestMethod]
        public void GetLossPercentage_DoNotUseBaseFareAirChangeAndOffRecordChangeNotZero_ReturnAirChangeMinusOffRdChangeDivByAirChange()
        {
            // Arrange
            var airFareSavingCalculations = new AirFareSavingsCalculations();
            decimal airChg = 2, offRcdChng = 3, baseFare = 4;
            bool useBaseFare = false;
            decimal result = 0;

            // Act
            result = airFareSavingCalculations.GetLossPercentage(airChg, offRcdChng, baseFare, useBaseFare);

            // Assert
            Assert.AreEqual(Decimal.Round(100 * ((airChg - offRcdChng) / airChg), 2), result);
        }


        [TestMethod]
        public void GetLossPercentage_UseBaseFareBaseFareIsZero_ReturnZero()
        {
            // Arrange
            var airFareSavingCalculations = new AirFareSavingsCalculations();
            decimal airChg = 2, offRcdChng = 3, baseFare = 0;
            bool useBaseFare = true;
            decimal result = 0;

            // Act
            result = airFareSavingCalculations.GetLossPercentage(airChg, offRcdChng, baseFare, useBaseFare);

            // Assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void GetLossPercentage_UseBaseFareOffRcdChngIsZero_ReturnZero()
        {
            // Arrange
            var airFareSavingCalculations = new AirFareSavingsCalculations();
            decimal airChg = 2, offRcdChng = 0, baseFare = 4;
            bool useBaseFare = true;
            decimal result = 0;

            // Act
            result = airFareSavingCalculations.GetLossPercentage(airChg, offRcdChng, baseFare, useBaseFare);

            // Assert
            Assert.AreEqual(0, result);
        }

    }
}
