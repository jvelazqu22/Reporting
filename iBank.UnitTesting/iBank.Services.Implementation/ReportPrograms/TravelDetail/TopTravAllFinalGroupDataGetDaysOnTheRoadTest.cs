using iBank.Services.Implementation.ReportPrograms.TravelDetail;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.TravelDetail
{
    // Unit test naming convention:
    // [UnitOfWork_StateUnderTest_ExpectedBehavior]
    // Example: public void Sun_NegativeNumberAsFirstParam_ExceptionThrown
    [TestClass]
    public class TopTravAllFinalGroupDataGetDaysOnTheRoadTest
    {
        [TestMethod]
        public void GetDaysOnTheRoad_TripStartDateHasValue_ReturnDaysOnTheRoadUsingTripStart()
        {
            // Arrange
            DateTime? TripStart = new DateTime(2016, 10, 10);
            DateTime? TripEnd = new DateTime(2016, 10, 14);
            DateTime? DepDate = new DateTime(2016, 10, 10);
            DateTime? ArrDate = new DateTime(2016, 10, 14);
            int PlusMin = 1, result = 0, expectedResult = 5;

            // Act
            result = TopTravAllFinalGroupData.GetDaysOnTheRoad(TripStart, TripEnd, DepDate, ArrDate, PlusMin);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void GetDaysOnTheRoad_TripStartDateIsNull_ReturnDaysOnTheRoadUsingTripStart()
        {
            // Arrange
            DateTime? TripStart = null;
            DateTime? TripEnd = new DateTime(2016, 10, 14);
            DateTime? DepDate = new DateTime(2016, 10, 10);
            DateTime? ArrDate = new DateTime(2016, 10, 14);
            int PlusMin = 1, result = 0, expectedResult = 5;

            // Act
            result = TopTravAllFinalGroupData.GetDaysOnTheRoad(TripStart, TripEnd, DepDate, ArrDate, PlusMin);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void GetDaysOnTheRoad_TripStartAndDepDateAreNull_ReturnDaysOnTheRoadUsingTripStart()
        {
            // Arrange
            DateTime? TripStart = null;
            DateTime? TripEnd = new DateTime(2016, 10, 14);
            DateTime? DepDate = new DateTime(2016, 10, 10);
            DateTime? ArrDate = null;
            int PlusMin = 1, result = -1, expectedResult = 0;

            // Act
            result = TopTravAllFinalGroupData.GetDaysOnTheRoad(TripStart, TripEnd, DepDate, ArrDate, PlusMin);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }
    }
}
