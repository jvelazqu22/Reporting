using iBank.Services.Implementation.ReportPrograms.AirFareSavings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.AirFareSavings.Calculations
{
    [TestClass]
    public class AirFareSavingsCalculationsGetSavingsTest
    {
        // Unit test naming convention:
        // [UnitOfWork_StateUnderTest_ExpectedBehavior]
        // Example: public void Sun_NegativeNumberAsFirstParam_ExceptionThrown

        [TestMethod]
        public void GetSavings_UseBaseFareIsTrue_ReturnStndChgMinusBaseFare()
        {
            // Arrange
            var airFareSavingCalculations = new AirFareSavingsCalculations();
            decimal stndChg = 2, airChg = 3, baseFare = 1;
            decimal result = 0;

            // Act
            result = airFareSavingCalculations.GetSavings(stndChg, airChg, baseFare, true);

            // Assert
            Assert.AreEqual((stndChg - baseFare), result);
        }

        [TestMethod]
        public void GetSavings_UseBaseFareIsFalseSavingsGreaterThanZero_ReturnStndChgMinusAirChg()
        {
            // Arrange
            var airFareSavingCalculations = new AirFareSavingsCalculations();
            decimal stndChg = 4, airChg = 3, baseFare = 1;
            decimal result = 0;

            // Act
            result = airFareSavingCalculations.GetSavings(stndChg, airChg, baseFare, false);

            // Assert
            Assert.AreEqual((stndChg - airChg), result);
        }
    }
}
