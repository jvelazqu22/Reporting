using Domain.Models.ReportPrograms.TravelDetail;
using iBank.Services.Implementation.ReportPrograms.TravelDetail;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.TravelDetail
{
    // Unit test naming convention:
    // [UnitOfWork_StateUnderTest_ExpectedBehavior]
    // Example: public void Sun_NegativeNumberAsFirstParam_ExceptionThrown
    [TestClass]
    public class TopTravAllFinalDataCalculatorIsItRailRecordTest
    {
        [TestMethod]
        public void IsItRailRecord_ValCarIs4_ReturnTrue()
        {
            // Arrange
            TopTravAllFinalDataCalculator topTravAllFinalDataCalculator = new TopTravAllFinalDataCalculator(new ClientFunctions());
            TravDetRawData rawData = new TravDetRawData() { ValCarMode = "R" };
            bool result = false;

            // Act
            result = topTravAllFinalDataCalculator.IsItRailRecord(rawData);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsItRailRecord_ValCarIsNot4_ReturnTrue()
        {
            // Arrange
            TopTravAllFinalDataCalculator topTravAllFinalDataCalculator = new TopTravAllFinalDataCalculator(new ClientFunctions());
            TravDetRawData rawData = new TravDetRawData() { ValCarr = "5" };
            bool result = false;

            // Act
            result = topTravAllFinalDataCalculator.IsItRailRecord(rawData);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsItRailRecord_ValCarModeIsR_ReturnTrue()
        {
            // Arrange
            TopTravAllFinalDataCalculator topTravAllFinalDataCalculator = new TopTravAllFinalDataCalculator(new ClientFunctions());
            TravDetRawData rawData = new TravDetRawData() { ValCarMode = "R" };
            bool result = false;

            // Act
            result = topTravAllFinalDataCalculator.IsItRailRecord(rawData);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsItRailRecord_ValCarModeIsNotR_ReturnTrue()
        {
            // Arrange
            TopTravAllFinalDataCalculator topTravAllFinalDataCalculator = new TopTravAllFinalDataCalculator(new ClientFunctions());
            TravDetRawData rawData = new TravDetRawData() { ValCarMode = "Z" };
            bool result = false;

            // Act
            result = topTravAllFinalDataCalculator.IsItRailRecord(rawData);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
