using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using iBank.Services.Implementation.ReportPrograms.TopBottomTravelerAir;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.TopBottomTravelerAir
{
    [TestClass]
    public class TopTravAirHelpersTests
    {
        [TestMethod]
        public void GetCrystalReportName_useHomeCountry_ibTopTravs2()
        {
            // Arrange
            bool _useHomeCountry = true;

            // Act
            var outcome = TopTravAirHelpers.GetCrystalReportName(_useHomeCountry);

            // Assert
            Assert.AreEqual("ibTopTravs2", outcome);
        }

        [TestMethod]
        public void GetCrystalReportName_Default_ibTopTravs2()
        {
            // Arrange
            bool _useHomeCountry = false;

            // Act
            var outcome = TopTravAirHelpers.GetCrystalReportName(_useHomeCountry);

            // Assert
            Assert.AreEqual("ibTopTravs", outcome);
        }
    }
}
