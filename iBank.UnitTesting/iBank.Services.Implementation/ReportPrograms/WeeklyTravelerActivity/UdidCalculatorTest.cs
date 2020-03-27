using Domain.Models.ReportPrograms.WeeklyTravelerActivity;
using iBank.Services.Implementation.ReportPrograms.WeeklyTravelerActivity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Configuration;


namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.WeeklyTravelerActivity
{
    // Unit test naming convention:
    // [UnitOfWork_StateUnderTest_ExpectedBehavior]
    // Example: public void Sun_NegativeNumberAsFirstParam_ExceptionThrown
    [TestClass]
    public class UdidCalculatorTest
    {
        [TestMethod]
        public void AddReportSettingsUdidItem_udidOnRptIsZero_NoItemsAdded()
        {
            // Arrange
            int udidOnRpt = 0;
            string udidLbl = "test";
            List<UdidData> ReportSettingsUdidInfoList = new List<UdidData>();
            CommaDelimitedStringCollection ReportSettingsUdidsString = new CommaDelimitedStringCollection();
            UdidCalculator udidCalculator = new UdidCalculator();

            // Act
            udidCalculator.AddReportSettingsUdidItem(udidOnRpt, udidLbl, ReportSettingsUdidInfoList, ReportSettingsUdidsString);

            // Assert
            Assert.AreEqual(0, ReportSettingsUdidInfoList.Count);
            Assert.AreEqual(0, ReportSettingsUdidsString.Count);
        }

        [TestMethod]
        public void AddReportSettingsUdidItem_udidOnRptIsNotZero_ItemsAdded()
        {
            // Arrange
            int udidOnRpt = 1;
            string udidLbl = "test";
            List<UdidData> ReportSettingsUdidInfoList = new List<UdidData>();
            CommaDelimitedStringCollection ReportSettingsUdidsString = new CommaDelimitedStringCollection();
            UdidCalculator udidCalculator = new UdidCalculator();

            // Act
            udidCalculator.AddReportSettingsUdidItem(udidOnRpt, udidLbl, ReportSettingsUdidInfoList, ReportSettingsUdidsString);

            // Assert
            Assert.AreEqual(1, ReportSettingsUdidInfoList.Count);
            Assert.AreEqual(1, ReportSettingsUdidsString.Count);
        }
    }
}
