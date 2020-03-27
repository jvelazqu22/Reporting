using Domain.Models.ReportPrograms.TopBottomTravelersCar;
using iBank.Services.Implementation.ReportPrograms.TopBottomTravelerCar;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.TopBottomTravelerCar
{
    // Unit test naming convention:
    // [UnitOfWork_StateUnderTest_ExpectedBehavior]
    // Example: public void Sun_NegativeNumberAsFirstParam_ExceptionThrown

    [TestClass]
    public class CarTopBottomTravelerRawDataCalculatorTest
    {
        [TestMethod]
        public void GetRawListTotalRentals_RawDatListWithRentalValues_ReturnsSumOfRentals()
        {
            // Arrange
            var rawDataList = new List<RawData>()
            {
                new RawData() { Rentals = 1 },
                new RawData() { Rentals = 1 },
                new RawData() { Rentals = 1 },
                new RawData() { Rentals = 1 },
            };

            int result = 0;
            int expectedResult = 4;

            // Act
            result = new CarTopBottomTravelerRawDataCalculator().GetRawListTotalRentals(rawDataList);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void GetRawListTotalDays_RawDatListWithDayValues_ReturnsSumOfDays()
        {
            // Arrange
            var rawDataList = new List<RawData>()
            {
                new RawData() { Rentals = 1, Days = 1 },
                new RawData() { Rentals = 1, Days = 1 },
                new RawData() { Rentals = 1, Days = 1 },
                new RawData() { Rentals = 1, Days = 1 },
            };

            int result = 0;
            int expectedResult = 4;

            // Act
            result = new CarTopBottomTravelerRawDataCalculator().GetRawListTotalDays(rawDataList);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }


        [TestMethod]
        public void GetRawListTotalCarCosts_RawDatListWithCarCostValues_ReturnsSumOfCarCosts()
        {
            // Arrange
            var rawDataList = new List<RawData>()
            {
                new RawData() { Rentals = 1, Days = 1, CarCost = 1 },
                new RawData() { Rentals = 1, Days = 1, CarCost = 1 },
                new RawData() { Rentals = 1, Days = 1, CarCost = 1 },
                new RawData() { Rentals = 1, Days = 1, CarCost = 1 },
            };

            decimal result = 0;
            decimal expectedResult = 4;

            // Act
            result = new CarTopBottomTravelerRawDataCalculator().GetRawListTotalCarCosts(rawDataList);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }
    }
}
