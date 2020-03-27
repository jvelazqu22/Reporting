using Domain.Constants;
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
    public class CarTopBottomTravelerDataTest
    {
        [TestMethod]
        public void SortList_OrderByCarCost_ReturnInDescOrder()
        {
            // Arrange
            string sortedBy = ReportFilters.CAR_TOP_BOTTOM_TRAVELER_RPT_SORT_BY_CAR_COST, sortOrder = ReportFilters.SORT_ORDER_DECENDING, howManyString = "4";
            List<FinalData> finalList = new List<FinalData>()
            {
                new FinalData() { Carcost = 1 },
                new FinalData() { Carcost = 2 },
                new FinalData() { Carcost = 3 },
                new FinalData() { Carcost = 4 },
            };
            List<FinalData> results = new List<FinalData>();

            // Act
            results = new CarTopBottomTravelerData().SortList(finalList, sortedBy, sortOrder, howManyString);

            // Assert
            Assert.AreEqual(4, results[0].Carcost);
            Assert.AreEqual(3, results[1].Carcost);
            Assert.AreEqual(2, results[2].Carcost);
            Assert.AreEqual(1, results[3].Carcost);
        }

        [TestMethod]
        public void SortList_OrderByRentalsDescOrder_ReturnByRentalsInDescOrder()
        {
            // Arrange
            string sortedBy = ReportFilters.CAR_TOP_BOTTOM_TRAVELER_RPT_SORT_BY_RENTALS, sortOrder = ReportFilters.SORT_ORDER_DECENDING, howManyString = "4";
            List<FinalData> finalList = new List<FinalData>()
            {
                new FinalData() { Carcost = 1, Rentals = 1 },
                new FinalData() { Carcost = 2, Rentals = 2 },
                new FinalData() { Carcost = 3, Rentals = 3 },
                new FinalData() { Carcost = 4, Rentals = 4 },
            };
            List<FinalData> results = new List<FinalData>();

            // Act
            results = new CarTopBottomTravelerData().SortList(finalList, sortedBy, sortOrder, howManyString);

            // Assert
            Assert.AreEqual(4, results[0].Rentals);
            Assert.AreEqual(3, results[1].Rentals);
            Assert.AreEqual(2, results[2].Rentals);
            Assert.AreEqual(1, results[3].Rentals);
        }

        [TestMethod]
        public void SortList_OrderByDaysAscOrder_ReturnByDaysInAscOrder()
        {
            // Arrange
            string sortedBy = ReportFilters.CAR_TOP_BOTTOM_TRAVELER_RPT_SORT_BY_DAYS, sortOrder = ReportFilters.SORT_ORDER_ASCENDING, howManyString = "4";
            List<FinalData> finalList = new List<FinalData>()
            {
                new FinalData() { Carcost = 1, Rentals = 1, Days = 4, },
                new FinalData() { Carcost = 2, Rentals = 2, Days = 3, },
                new FinalData() { Carcost = 3, Rentals = 3, Days = 2, },
                new FinalData() { Carcost = 4, Rentals = 4, Days = 1, },
            };
            List<FinalData> results = new List<FinalData>();

            // Act
            results = new CarTopBottomTravelerData().SortList(finalList, sortedBy, sortOrder, howManyString);

            // Assert
            Assert.AreEqual(1, results[0].Days);
            Assert.AreEqual(2, results[1].Days);
            Assert.AreEqual(3, results[2].Days);
            Assert.AreEqual(4, results[3].Days);
        }

        [TestMethod]
        public void SortList_OrderAvgBookAscOrder_ReturnByAvgBooksInAscOrder()
        {
            // Arrange
            string sortedBy = ReportFilters.CAR_TOP_BOTTOM_TRAVELER_RPT_SORT_BY_AVG_BOOK, sortOrder = ReportFilters.SORT_ORDER_ASCENDING, howManyString = "4";
            List<FinalData> finalList = new List<FinalData>()
            {
                new FinalData() { Carcost = 1, Rentals = 1, Days = 4, Avgbook = 4 },
                new FinalData() { Carcost = 2, Rentals = 2, Days = 3, Avgbook = 3 },
                new FinalData() { Carcost = 3, Rentals = 3, Days = 2, Avgbook = 2 },
                new FinalData() { Carcost = 4, Rentals = 4, Days = 1, Avgbook = 1 },
            };
            List<FinalData> results = new List<FinalData>();

            // Act
            results = new CarTopBottomTravelerData().SortList(finalList, sortedBy, sortOrder, howManyString);

            // Assert
            Assert.AreEqual(1, results[0].Avgbook);
            Assert.AreEqual(2, results[1].Avgbook);
            Assert.AreEqual(3, results[2].Avgbook);
            Assert.AreEqual(4, results[3].Avgbook);
        }
    }
}
