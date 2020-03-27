using Domain.Constants;
using iBank.Services.Implementation.ReportPrograms.AirFareSavings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.AirFareSavings.Calculations
{
    [TestClass]
    public class AirFareSavingsCalculationsGetCrystalReportNameTest
    {
        // Unit test naming convention:
        // [UnitOfWork_StateUnderTest_ExpectedBehavior]
        // Example: public void Sun_NegativeNumberAsFirstParam_ExceptionThrown

        [TestMethod]
        public void GetCrystalReportName_PageSummaryOnlyIsTrue_ReturnFareSaveSummaryReportName()
        {
            // Arrange
            var airFareSavingCalculations = new AirFareSavingsCalculations();
            string result = string.Empty;

            // Act
            result = airFareSavingCalculations.GetCrystalReportName(true, true, true);

            // Assert
            Assert.AreEqual(ReportNames.AIR_FARE_SAVINGS_SUMMARY_RPT, result);
        }

        [TestMethod]
        public void GetCrystalReportName_PageSummaryOnlyIsFalseAndExSavingsAndExNegoSvgsAreTrue_ReturnsReport4()
        {
            // Arrange
            var airFareSavingCalculations = new AirFareSavingsCalculations();
            string result = string.Empty;

            // Act
            result = airFareSavingCalculations.GetCrystalReportName(false, true, true);

            // Assert
            Assert.AreEqual(ReportNames.AIR_FARE_SAVINGS_RPT_4, result);
        }

        [TestMethod]
        public void GetCrystalReportName_PageSummaryOnlyIsFalseAndExSavingsIsTrueAndExNegoSvgsIsFalse_ReturnReport3()
        {
            // Arrange
            var airFareSavingCalculations = new AirFareSavingsCalculations();
            string result = string.Empty;

            // Act
            result = airFareSavingCalculations.GetCrystalReportName(false, true, false);

            // Assert
            Assert.AreEqual(ReportNames.AIR_FARE_SAVINGS_RPT_3, result);
        }

        [TestMethod]
        public void GetCrystalReportName_PageSummaryOnlyIsFalseAndExSavingsIsTrueAndExNegoSvgsIsTrue_ReturnReport2()
        {
            // Arrange
            var airFareSavingCalculations = new AirFareSavingsCalculations();
            string result = string.Empty;

            // Act
            result = airFareSavingCalculations.GetCrystalReportName(false, false, true);

            // Assert
            Assert.AreEqual(ReportNames.AIR_FARE_SAVINGS_RPT_2, result);
        }

        [TestMethod]
        public void GetCrystalReportName_PageSummaryOnlyIsFalseAndExSavingsIsFalseAndExNegoSvgsIsFalse_ReturnReport1()
        {
            // Arrange
            var airFareSavingCalculations = new AirFareSavingsCalculations();
            string result = string.Empty;

            // Act
            result = airFareSavingCalculations.GetCrystalReportName(false, false, false);

            // Assert
            Assert.AreEqual(ReportNames.AIR_FARE_SAVINGS_RPT_1, result);
        }
    }
}
