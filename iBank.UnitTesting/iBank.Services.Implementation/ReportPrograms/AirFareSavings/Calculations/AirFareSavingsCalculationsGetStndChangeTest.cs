using iBank.Services.Implementation.ReportPrograms.AirFareSavings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.AirFareSavings.Calculations
{
    [TestClass]
    public class AirFareSavingsCalculationsGetStndChangeTest
    {
        // Unit test naming convention:
        // [UnitOfWork_StateUnderTest_ExpectedBehavior]
        // Example: public void Sun_NegativeNumberAsFirstParam_ExceptionThrown

        [TestMethod]
        public void GetStndChange_DeriveSvgCodeIsTrueAndStndChgIsLessThanAirChg_ReturnAirChg()
        {
            // Arrange
            var airFareSavingCalculations = new AirFareSavingsCalculations();
            decimal stndChg = 1, airChg = 2;
            decimal result = 0;

            // Act
            result = airFareSavingCalculations.GetStndCharge(true, stndChg, airChg);

            // Assert
            Assert.AreEqual(airChg, result);
        }

        [TestMethod]
        public void GetStndChange_DeriveSvgCodeIsTrueAndStndChgIsGreaterThanAirChg_ReturnStndChg()
        {
            // Arrange
            var airFareSavingCalculations = new AirFareSavingsCalculations();
            decimal stndChg = 3, airChg = 2;
            decimal result = 0;

            // Act
            result = airFareSavingCalculations.GetStndCharge(true, stndChg, airChg);

            // Assert
            Assert.AreEqual(stndChg, result);
        }

        [TestMethod]
        public void GetStndChange_DeriveSvgCodeIsTrueAndStndChgIsZero_ReturnAirChg()
        {
            // Arrange
            var airFareSavingCalculations = new AirFareSavingsCalculations();
            decimal stndChg = 0, airChg = 2;
            decimal result = 0;

            // Act
            result = airFareSavingCalculations.GetStndCharge(true, stndChg, airChg);

            // Assert
            Assert.AreEqual(airChg, result);
        }

        [TestMethod]
        public void GetStndChange_DeriveSvgCodeIsTrueAndStndChgAndAirChgAreEqual_ReturnStndChg()
        {
            // Arrange
            var airFareSavingCalculations = new AirFareSavingsCalculations();
            decimal stndChg = 1, airChg = 1;
            decimal result = 0;

            // Act
            result = airFareSavingCalculations.GetStndCharge(true, stndChg, airChg);

            // Assert
            Assert.AreEqual(stndChg, result);
        }

        [TestMethod]
        public void GetStndChange_DeriveSvgCodeIsTrueAndStndChgGreaterThanZeroAndAirChgLessThanZero_ReturnAirChg()
        {
            // Arrange
            var airFareSavingCalculations = new AirFareSavingsCalculations();
            decimal stndChg = 3, airChg = -1;
            decimal result = 0;

            // Act
            result = airFareSavingCalculations.GetStndCharge(true, stndChg, airChg);

            // Assert
            Assert.AreEqual(airChg, result);
        }

        [TestMethod]
        public void GetStndChange_DeriveSvgCodeIsTrueAndStndChgGreaterThanZeroAndAirChgGreaterThanZero_ReturnStndChg()
        {
            // Arrange
            var airFareSavingCalculations = new AirFareSavingsCalculations();
            decimal stndChg = 3, airChg = 1;
            decimal result = 0;

            // Act
            result = airFareSavingCalculations.GetStndCharge(true, stndChg, airChg);

            // Assert
            Assert.AreEqual(stndChg, result);
        }

        [TestMethod]
        public void GetStndChange_DeriveSvgCodeIsFalseAndStndChgGreaterThanZeroAndAirChgGreaterThanZero_ReturnStndChg()
        {
            // Arrange
            var airFareSavingCalculations = new AirFareSavingsCalculations();
            decimal stndChg = 3, airChg = 1;
            decimal result = 0;

            // Act
            result = airFareSavingCalculations.GetStndCharge(false, stndChg, airChg);

            // Assert
            Assert.AreEqual(stndChg, result);
        }

        [TestMethod]
        public void GetStndChange_DeriveSvgCodeIsFalseAndStndChgGreaterThanZeroAndAirChgLessThanZero_ReturnZeroMinusStndChg()
        {
            // Arrange
            var airFareSavingCalculations = new AirFareSavingsCalculations();
            decimal stndChg = 3, airChg = -1;
            decimal result = 0;

            // Act
            result = airFareSavingCalculations.GetStndCharge(false, stndChg, airChg);

            // Assert
            Assert.AreEqual((0 - stndChg), result);
        }
    }
}
