using Domain.Constants;
using iBank.Services.Implementation.ReportPrograms.TravelDetail;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

using Domain.Models.ReportPrograms.TravelDetail;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.TravelDetail
{
    // Unit test naming convention:
    // [UnitOfWork_StateUnderTest_ExpectedBehavior]
    // Example: public void Sun_NegativeNumberAsFirstParam_ExceptionThrown
    [TestClass]
    public class TopTravAllDataSortTest
    {
        [TestMethod]
        public void SortList_OrderByTripCountDescOrder_ReturnByTripCountInDescOrder()
        {
            // Arrange
            string sortedBy = ReportFilters.TOP_TRAVELER_COMBINED_RPT_SORT_BY_TRIP_COUNT, sortOrder = ReportFilters.SORT_ORDER_DECENDING, howManyString = "4";
            var finalList = new List<TopTravAllFinalData>
            {
                new TopTravAllFinalData { tripcount = 5, daysonroad = 2, railcount = 3},
                new TopTravAllFinalData { tripcount = 6, daysonroad = 2, railcount = 3},
                new TopTravAllFinalData { tripcount = 7, daysonroad = 2, railcount = 3},
                new TopTravAllFinalData { tripcount = 8, daysonroad = 2, railcount = 3}
            };
            
            // Act
            var results = TopTravAllData.SortList(finalList, sortedBy, sortOrder, howManyString);

            // Assert
            Assert.AreEqual(8, results[0].tripcount);
            Assert.AreEqual(7, results[1].tripcount);
            Assert.AreEqual(6, results[2].tripcount);
            Assert.AreEqual(5, results[3].tripcount);
        }

        [TestMethod]
        public void SortList_OrderByTripCountAscOrder_ReturnByTripCountInAscOrder()
        {
            // Arrange
            string sortedBy = ReportFilters.TOP_TRAVELER_COMBINED_RPT_SORT_BY_TRIP_COUNT, sortOrder = ReportFilters.SORT_ORDER_ASCENDING, howManyString = "4";
            var finalList = new List<TopTravAllFinalData>
            {
                new TopTravAllFinalData { tripcount = 5, daysonroad = 2, railcount = 3 },
                new TopTravAllFinalData { tripcount = 6, daysonroad = 2, railcount = 3 },
                new TopTravAllFinalData { tripcount = 7, daysonroad = 2, railcount = 3 },
                new TopTravAllFinalData { tripcount = 8, daysonroad = 2, railcount = 3 }
            };


            // Act
            var results = TopTravAllData.SortList(finalList, sortedBy, sortOrder, howManyString);

            // Assert
            Assert.AreEqual(5, results[0].tripcount);
            Assert.AreEqual(6, results[1].tripcount);
            Assert.AreEqual(7, results[2].tripcount);
            Assert.AreEqual(8, results[3].tripcount);
        }

        [TestMethod]
        public void SortList_OrderByDaysOnTheRoadDescOrder_ReturnByTripCountInDescOrder()
        {
            // Arrange
            string sortedBy = ReportFilters.TOP_TRAVELER_COMBINED_RPT_SORT_BY_DAYS_ON_THE_ROAD, sortOrder = ReportFilters.SORT_ORDER_DECENDING, howManyString = "4";
            var finalList = new List<TopTravAllFinalData>
            {
                new TopTravAllFinalData { tripcount = 5, daysonroad = 1, railcount = 3},
                new TopTravAllFinalData { tripcount = 6, daysonroad = 2, railcount = 3},
                new TopTravAllFinalData { tripcount = 7, daysonroad = 3, railcount = 3},
                new TopTravAllFinalData { tripcount = 8, daysonroad = 4, railcount = 3},
            };

            // Act
            var results = TopTravAllData.SortList(finalList, sortedBy, sortOrder, howManyString);

            // Assert
            Assert.AreEqual(4, results[0].daysonroad);
            Assert.AreEqual(3, results[1].daysonroad);
            Assert.AreEqual(2, results[2].daysonroad);
            Assert.AreEqual(1, results[3].daysonroad);
        }

        [TestMethod]
        public void SortList_OrderByDaysOnTheRoadAscOrder_ReturnByTripCountInAscOrder()
        {
            // Arrange
            string sortedBy = ReportFilters.TOP_TRAVELER_COMBINED_RPT_SORT_BY_DAYS_ON_THE_ROAD, sortOrder = ReportFilters.SORT_ORDER_ASCENDING, howManyString = "4";
            var finalList = new List<TopTravAllFinalData>
            {
                new TopTravAllFinalData { tripcount = 5, daysonroad = 4, railcount = 3 },
                new TopTravAllFinalData { tripcount = 6, daysonroad = 3, railcount = 3 },
                new TopTravAllFinalData { tripcount = 7, daysonroad = 2, railcount = 3 },
                new TopTravAllFinalData { tripcount = 8, daysonroad = 1, railcount = 3 }
            };

            // Act
            var results = TopTravAllData.SortList(finalList, sortedBy, sortOrder, howManyString);

            // Assert
            Assert.AreEqual(1, results[0].daysonroad);
            Assert.AreEqual(2, results[1].daysonroad);
            Assert.AreEqual(3, results[2].daysonroad);
            Assert.AreEqual(4, results[3].daysonroad);
        }

        [TestMethod]
        public void SortList_OrderByRailCountDescOrder_ReturnByTripCountInDescOrder()
        {
            // Arrange
            string sortedBy = ReportFilters.TOP_TRAVELER_COMBINED_RPT_SORT_BY_RAIL_COUNT, sortOrder = ReportFilters.SORT_ORDER_DECENDING, howManyString = "4";
            var finalList = new List<TopTravAllFinalData>
            {
                new TopTravAllFinalData { tripcount = 5, daysonroad = 1, railcount = 9},
                new TopTravAllFinalData { tripcount = 6, daysonroad = 2, railcount = 10 },
                new TopTravAllFinalData { tripcount = 7, daysonroad = 3, railcount = 11 },
                new TopTravAllFinalData { tripcount = 8, daysonroad = 4, railcount = 12 }
            };

            // Act
            var results = TopTravAllData.SortList(finalList, sortedBy, sortOrder, howManyString);

            // Assert
            Assert.AreEqual(12, results[0].railcount);
            Assert.AreEqual(11, results[1].railcount);
            Assert.AreEqual(10, results[2].railcount);
            Assert.AreEqual(9, results[3].railcount);
        }

        [TestMethod]
        public void SortList_OrderByRailCountAscOrder_ReturnByTripCountInAscOrder()
        {
            // Arrange
            string sortedBy = ReportFilters.TOP_TRAVELER_COMBINED_RPT_SORT_BY_RAIL_COUNT, sortOrder = ReportFilters.SORT_ORDER_ASCENDING, howManyString = "4";
            var finalList = new List<TopTravAllFinalData>
            {
                new TopTravAllFinalData{ tripcount = 5, daysonroad = 4, railcount = 9},
                new TopTravAllFinalData{ tripcount = 6, daysonroad = 3, railcount = 10 },
                new TopTravAllFinalData{ tripcount = 7, daysonroad = 2, railcount = 11 },
                new TopTravAllFinalData{ tripcount = 8, daysonroad = 1, railcount = 12 }
            };

            // Act
            var results = TopTravAllData.SortList(finalList, sortedBy, sortOrder, howManyString);

            // Assert
            Assert.AreEqual(9, results[0].railcount);
            Assert.AreEqual(10, results[1].railcount);
            Assert.AreEqual(11, results[2].railcount);
            Assert.AreEqual(12, results[3].railcount);
        }

        [TestMethod]
        public void SortList_OrderByTripCostDescOrder_ReturnByTripCountInDescOrder()
        {
            // Arrange
            string sortedBy = ReportFilters.TOP_TRAVELER_COMBINED_RPT_SORT_BY_TRIP_COST, sortOrder = ReportFilters.SORT_ORDER_DECENDING, howManyString = "4";
            var finalList = new List<TopTravAllFinalData>
            {
                new TopTravAllFinalData { tripcount = 5, daysonroad = 1, railcount = 9, carcost = 1, hotelcost = 2, airchg = 3, railchg = 4 },
                new TopTravAllFinalData { tripcount = 6, daysonroad = 2, railcount = 10, carcost = 2, hotelcost = 2, airchg = 3, railchg = 4 },
                new TopTravAllFinalData { tripcount = 7, daysonroad = 3, railcount = 11, carcost = 3, hotelcost = 2, airchg = 3, railchg = 4 },
                new TopTravAllFinalData { tripcount = 8, daysonroad = 4, railcount = 12, carcost = 4, hotelcost = 2, airchg = 3, railchg = 4 },
            };

            // Act
            var results = TopTravAllData.SortList(finalList, sortedBy, sortOrder, howManyString);

            // Assert
            Assert.AreEqual(13, results[0].tripcost);
            Assert.AreEqual(12, results[1].tripcost);
            Assert.AreEqual(11, results[2].tripcost);
            Assert.AreEqual(10, results[3].tripcost);
        }

        [TestMethod]
        public void SortList_OrderByTripCostAscOrder_ReturnByTripCountInAscOrder()
        {
            // Arrange
            string sortedBy = ReportFilters.TOP_TRAVELER_COMBINED_RPT_SORT_BY_TRIP_COST, sortOrder = ReportFilters.SORT_ORDER_ASCENDING, howManyString = "4";
            var finalList = new List<TopTravAllFinalData>
            {
                new TopTravAllFinalData { tripcount = 5, daysonroad = 1, railcount = 9, carcost = 1, hotelcost = 2, airchg = 3, railchg = 4 },
                new TopTravAllFinalData { tripcount = 6, daysonroad = 2, railcount = 10, carcost = 2, hotelcost = 2, airchg = 3, railchg = 4 },
                new TopTravAllFinalData { tripcount = 7, daysonroad = 3, railcount = 11, carcost = 3, hotelcost = 2, airchg = 3, railchg = 4 },
                new TopTravAllFinalData { tripcount = 8, daysonroad = 4, railcount = 12, carcost = 4, hotelcost = 2, airchg = 3, railchg = 4 },
            };

            // Act
            var results = TopTravAllData.SortList(finalList, sortedBy, sortOrder, howManyString);

            // Assert
            Assert.AreEqual(10, results[0].tripcost);
            Assert.AreEqual(11, results[1].tripcost);
            Assert.AreEqual(12, results[2].tripcost);
            Assert.AreEqual(13, results[3].tripcost);
        }

    }
}
