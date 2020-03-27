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
    public class TopTravAllFinalDataCalculatorGetCrystalReportNameTest
    {
        [TestMethod]
        public void GetCrystalReportName_IncludeCarbonAndHomeCountry_ReturnCarbon2Report()
        {
            // Arrange
            TopTravAllFinalDataCalculator topTravAllFinalDataCalculator = new TopTravAllFinalDataCalculator(new ClientFunctions());
            bool includeCarbonEmissions = true, homeCountry = true;
            string result = string.Empty;

            // Act
            result = topTravAllFinalDataCalculator.GetCrystalReportName(includeCarbonEmissions, homeCountry);

            // Assert
            Assert.AreEqual("ibTopTravAllCarb2", result);
        }

        [TestMethod]
        public void GetCrystalReportName_IncludeCarbonButNotHomeCountry_ReturnCarbonReport()
        {
            // Arrange
            TopTravAllFinalDataCalculator topTravAllFinalDataCalculator = new TopTravAllFinalDataCalculator(new ClientFunctions());
            bool includeCarbonEmissions = true, homeCountry = false;
            string result = string.Empty;

            // Act
            result = topTravAllFinalDataCalculator.GetCrystalReportName(includeCarbonEmissions, homeCountry);

            // Assert
            Assert.AreEqual("ibTopTravAllCarb", result);
        }

        [TestMethod]
        public void GetCrystalReportName_DontIncludeCarbonNorHomeCountry_ReturnibTopTravAllReport()
        {
            // Arrange
            TopTravAllFinalDataCalculator topTravAllFinalDataCalculator = new TopTravAllFinalDataCalculator(new ClientFunctions());
            bool includeCarbonEmissions = false, homeCountry = false;
            string result = string.Empty;

            // Act
            result = topTravAllFinalDataCalculator.GetCrystalReportName(includeCarbonEmissions, homeCountry);

            // Assert
            Assert.AreEqual("ibTopTravAll", result);
        }

        [TestMethod]
        public void GetCrystalReportName_DontIncludeCarbonButIncludeHomeCountry_ReturnibTopTravAll2Report()
        {
            // Arrange
            TopTravAllFinalDataCalculator topTravAllFinalDataCalculator = new TopTravAllFinalDataCalculator(new ClientFunctions());
            bool includeCarbonEmissions = false, homeCountry = true;
            string result = string.Empty;

            // Act
            result = topTravAllFinalDataCalculator.GetCrystalReportName(includeCarbonEmissions, homeCountry);

            // Assert
            Assert.AreEqual("ibTopTravAll2", result);
        }
    }
}
