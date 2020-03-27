using iBank.Services.Implementation.ReportPrograms.AirFareSavings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.AirFareSavings.Calculations
{
    [TestClass]
    public class AirFareSavingsCalculationsGetOffRdChangeTest
    {
        // Unit test naming convention:
        // [UnitOfWork_StateUnderTest_ExpectedBehavior]
        // Example: public void Sun_NegativeNumberAsFirstParam_ExceptionThrown

        [TestMethod]
        public void GetOffRdChange_DeriveSvgCodeIsFalseAndOffRdChgIsGreaterThanZeroAndAirChgIsLessThanZero_ReturnZeroMinusOffRdChg()
        {
            // Arrange
            var airFareSavingCalculations = new AirFareSavingsCalculations();
            decimal offrdchg = 3, airChg = -1;
            decimal result = 0;

            // Act
            result = airFareSavingCalculations.GetOffRdChange(false, offrdchg, airChg);

            // Assert
            Assert.AreEqual((0 - offrdchg), result);
        }

        [TestMethod]
        public void GetOffRdChange_DeriveSvgCodeIsTrueAndOffRdChgIsGreaterThanZeroAndAirChgIsLessThanZero_ReturnZeroMinusOffRdChg()
        {
            // Arrange
            var airFareSavingCalculations = new AirFareSavingsCalculations();
            decimal offrdchg = 3, airChg = -1;
            decimal result = 0;

            // Act
            result = airFareSavingCalculations.GetOffRdChange(true, offrdchg, airChg);

            // Assert
            Assert.AreEqual((0 - offrdchg), result);
        }

        [TestMethod]
        public void GetOffRdChange_DeriveSvgCodeIsFalseAndOffRdChgIsGreaterThanZeroAndAirChgIsGreaterThanZero_ReturnOffRdChg()
        {
            // Arrange
            var airFareSavingCalculations = new AirFareSavingsCalculations();
            decimal offrdchg = 3, airChg = 1;
            decimal result = 0;

            // Act
            result = airFareSavingCalculations.GetOffRdChange(false, offrdchg, airChg);

            // Assert
            Assert.AreEqual(offrdchg, result);
        }

        [TestMethod]
        public void GetOffRdChange_DeriveSvgCodeIsTrueAndOffRdChgIsGreaterThanZeroAndAirChgIsGreaterThanZero_ReturnOffRdChg()
        {
            // Arrange
            var airFareSavingCalculations = new AirFareSavingsCalculations();
            decimal offrdchg = 3, airChg = 1;
            decimal result = 0;

            // Act
            result = airFareSavingCalculations.GetOffRdChange(true, offrdchg, airChg);

            // Assert
            Assert.AreEqual(offrdchg, result);
        }

        [TestMethod]
        public void GetOffRdChange_DeriveSvgCodeIsTrueAndOffRdChgIsZeroAndAirChgIsGreaterThanZero_ReturnAirChg()
        {
            // Arrange
            var airFareSavingCalculations = new AirFareSavingsCalculations();
            decimal offrdchg = 0, airChg = 1;
            decimal result = 0;

            // Act
            result = airFareSavingCalculations.GetOffRdChange(true, offrdchg, airChg);

            // Assert
            Assert.AreEqual(airChg, result);
        }

        [TestMethod]
        public void GetOffRdChange_DeriveSvgCodeIsFalseAndOffRdChgIsLessThanZeroAndAirChgIsGreaterThanZero_ReturnAirChg()
        {
            // Arrange
            var airFareSavingCalculations = new AirFareSavingsCalculations();
            decimal offrdchg = -3, airChg = 1;
            decimal result = 0;

            // Act
            result = airFareSavingCalculations.GetOffRdChange(false, offrdchg, airChg);

            // Assert
            Assert.AreEqual(offrdchg, result);
        }


        [TestMethod]
        public void GetOffRdChange_DeriveSvgCodeIsTrueAndOffRdChgIsLessThanZeroAndAirChgIsGreaterThanZero_ReturnOffRdChg()
        {
            // Arrange
            var airFareSavingCalculations = new AirFareSavingsCalculations();
            decimal offrdchg = -3, airChg = 1;
            decimal result = 0;

            // Act
            result = airFareSavingCalculations.GetOffRdChange(false, offrdchg, airChg);

            // Assert
            Assert.AreEqual(offrdchg, result);
        }

    }
}
