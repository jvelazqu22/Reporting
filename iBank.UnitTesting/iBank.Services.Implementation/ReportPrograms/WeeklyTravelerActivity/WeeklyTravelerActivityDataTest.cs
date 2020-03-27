using iBank.Services.Implementation.ReportPrograms.WeeklyTravelerActivity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

using Domain.Models.ReportPrograms.WeeklyTravelerActivity;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.WeeklyTravelerActivity
{
    // Unit test naming convention:
    // [UnitOfWork_StateUnderTest_ExpectedBehavior]
    // Example: public void Sun_NegativeNumberAsFirstParam_ExceptionThrown
    [TestClass]
    public class WeeklyTravelerActivityDataTest
    {
        [TestMethod]
        public void GetCrystalReportName_DoNotSuppressReportBreaksAndReportSettingsUdidDataListIsEmpty_ReturnibWeeklyActivity2()
        {
            // Arrange
            List<UdidData> reportSettingsUdidDataList = new List<UdidData>();
            bool suppressReportBreaks = false;
            WeeklyTravelerActivityData weeklyTravelerActivityData = new WeeklyTravelerActivityData();
            string result = string.Empty;
            string expectedResult = "ibWeeklyActivity2";

            // Act
            result = weeklyTravelerActivityData.GetCrystalReportName(reportSettingsUdidDataList, suppressReportBreaks);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void GetCrystalReportName_DoNotSuppressReportBreaksAndReportSettingsUdidDataListIsNotEmpty_ReturnibWeeklyActivity()
        {
            // Arrange
            List<UdidData> reportSettingsUdidDataList = new List<UdidData>() { new UdidData() { } };
            bool suppressReportBreaks = false;
            WeeklyTravelerActivityData weeklyTravelerActivityData = new WeeklyTravelerActivityData();
            string result = string.Empty;
            string expectedResult = "ibWeeklyActivity";

            // Act
            result = weeklyTravelerActivityData.GetCrystalReportName(reportSettingsUdidDataList, suppressReportBreaks);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void GetCrystalReportName_SuppressReportBreaksAndReportSettingsUdidDataListIsEmpty_ReturnibWeeklyActivity2()
        {
            // Arrange
            List<UdidData> reportSettingsUdidDataList = new List<UdidData>();
            bool suppressReportBreaks = true;
            WeeklyTravelerActivityData weeklyTravelerActivityData = new WeeklyTravelerActivityData();
            string result = string.Empty;
            string expectedResult = "ibWeeklyActivity2A";

            // Act
            result = weeklyTravelerActivityData.GetCrystalReportName(reportSettingsUdidDataList, suppressReportBreaks);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void GetCrystalReportName_SuppressReportBreaksAndReportSettingsUdidDataListIsNotEmpty_ReturnibWeeklyActivity()
        {
            // Arrange
            List<UdidData> reportSettingsUdidDataList = new List<UdidData>() { new UdidData() { } };
            bool suppressReportBreaks = true;
            WeeklyTravelerActivityData weeklyTravelerActivityData = new WeeklyTravelerActivityData();
            string result = string.Empty;
            string expectedResult = "ibWeeklyActivityA";

            // Act
            result = weeklyTravelerActivityData.GetCrystalReportName(reportSettingsUdidDataList, suppressReportBreaks);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }
    }
}
