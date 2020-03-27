using iBank.Services.Implementation.ReportPrograms.HotelActivity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.HotelActivity
{
    [TestClass]
    public class HotelActivityTest
    {
        // Unit test naming convention:
        // [UnitOfWork_StateUnderTest_ExpectedBehavior]
        // Example: public void Sun_NegativeNumberAsFirstParam_ExceptionThrown

        [TestMethod]
        public void SetCrystalReportNameAndSetIsReservationVariable_ReportValue80_ReturnHotelCityReportName()
        {
            // Arrange
            var hotelActivity = new HotelActivityCalculator();
            bool isReservation = false;
            string result = string.Empty;

            // Act
            result = hotelActivity.GetCrystalReportNameAndSetIsReservationVariable("80", ref isReservation);

            // Assert
            Assert.AreEqual("ibHotelCity", result);
            Assert.IsFalse(isReservation);
        }

        [TestMethod]
        public void SetCrystalReportNameAndSetIsReservationVariable_ReportValue82_ReturnHotelAdvResReportName()
        {
            // Arrange
            var hotelActivity = new HotelActivityCalculator();
            bool isReservation = false;
            string result = string.Empty;

            // Act
            result = hotelActivity.GetCrystalReportNameAndSetIsReservationVariable("82", ref isReservation);

            // Assert
            Assert.AreEqual("ibHotelAdvRes", result);
            Assert.IsTrue(isReservation);
        }

        [TestMethod]
        public void SetCrystalReportNameAndSetIsReservationVariable_ReportValue84_ReturnHotelVendorReportName()
        {
            // Arrange
            var hotelActivity = new HotelActivityCalculator();
            bool isReservation = false;
            string result = string.Empty;

            // Act
            result = hotelActivity.GetCrystalReportNameAndSetIsReservationVariable("84", ref isReservation);

            // Assert
            Assert.AreEqual("ibHotelVendor", result);
            Assert.IsFalse(isReservation);
        }

        [TestMethod]
        public void SetCrystalReportNameAndSetIsReservationVariable_ReportValue86_ReturnHotelActivityReportName()
        {
            // Arrange
            var hotelActivity = new HotelActivityCalculator();
            bool isReservation = false;
            string result = string.Empty;

            // Act
            result = hotelActivity.GetCrystalReportNameAndSetIsReservationVariable("86", ref isReservation);

            // Assert
            Assert.AreEqual("ibHotelActivity", result);
            Assert.IsFalse(isReservation);
        }

        [TestMethod]
        public void SetCrystalReportNameAndSetIsReservationVariable_ReportValue87_ReturnStringEmptyReportName()
        {
            // Arrange
            var hotelActivity = new HotelActivityCalculator();
            bool isReservation = false;
            string result = string.Empty;

            // Act
            result = hotelActivity.GetCrystalReportNameAndSetIsReservationVariable("87", ref isReservation);

            // Assert
            Assert.AreEqual(result, string.Empty);
            Assert.IsFalse(isReservation);
        }


        [TestMethod]
        public void GetSundayDate_PassTodaysDate_ShouldGetSundayOfTheWeekReturned()
        {
            // Arrange
            var hotelActivity = new HotelActivityCalculator();
            DateTime result;

            // Act
            result = hotelActivity.GetSundayDate(DateTime.Today);

            // Assert
            Assert.AreEqual(DayOfWeek.Sunday, result.DayOfWeek);
        }

        [TestMethod]
        public void GetSaturdayDate_PassTodaysDate_ShouldGetSaturdayOfTheWeekReturned()
        {
            // HotelActivityCalculator
            var hotelActivity = new HotelActivityCalculator();
            DateTime result;

            // Act
            result = hotelActivity.GetSaturdayDate(DateTime.Today);

            // Assert
            Assert.AreEqual(DayOfWeek.Saturday, result.DayOfWeek);
        }


        [TestMethod]
        public void GetWeekNum_PassTodaysDateForReportDateAndThisWeekSunday_ShouldGetTheNumber1Returned()
        {
            // Arrange
            var hotelActivity = new HotelActivityCalculator();
            decimal result;

            // Act
            result = hotelActivity.GetWeekNum(DateTime.Today, hotelActivity.GetSundayDate(DateTime.Today));

            // Assert
            Assert.AreEqual(1, result);
        }

    }
}
