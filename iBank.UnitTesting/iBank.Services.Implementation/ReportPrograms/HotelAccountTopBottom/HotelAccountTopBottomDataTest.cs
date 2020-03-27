using Domain.Constants;
using Domain.Models.ReportPrograms.HotelAccountTopBottom;
using iBank.Services.Implementation.ReportPrograms.HotelAccountTopBottom;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.CarTopBottomAccounts
{
    [TestClass]
    public class HotelAccountTopBottomDataTest
    {
        // Unit test naming convention:
        // [UnitOfWork_StateUnderTest_ExpectedBehavior]
        // Example: public void Sun_NegativeNumberAsFirstParam_ExceptionThrown

        [TestMethod]
        public void SortList_OrderByCost_ReturnInDescOrder()
        {
            // Arrange
            string sortedBy = ReportFilters.HOTEL_TOP_BOTTOM_ACCOUNTS_RPT_SORT_BY_COST, sortOrder = ReportFilters.SORT_ORDER_DECENDING, howManyString = "4";
            List<FinalData> finalList = new List<FinalData>()
            {
                new FinalData() { Nights = 1, Hotelcost = 1 },
                new FinalData() { Nights = 2, Hotelcost = 2 },
                new FinalData() { Nights = 3, Hotelcost = 3 },
                new FinalData() { Nights = 4, Hotelcost = 4 },
            };
            List<FinalData> results = new List<FinalData>();

            // Act
            results = new HotelAccountTopBottomData().SortList(finalList, sortedBy, sortOrder, howManyString);

            // Assert
            Assert.AreEqual(4, results[0].Hotelcost);
            Assert.AreEqual(3, results[1].Hotelcost);
            Assert.AreEqual(2, results[2].Hotelcost);
            Assert.AreEqual(1, results[3].Hotelcost);
        }

        [TestMethod]
        public void SortList_OrderByStaysDescOrder_ReturnByRentalsInDescOrder()
        {
            // Arrange
            string sortedBy = ReportFilters.HOTEL_TOP_BOTTOM_ACCOUNTS_RPT_SORT_BY_STAYS, sortOrder = ReportFilters.SORT_ORDER_DECENDING, howManyString = "4";
            List<FinalData> finalList = new List<FinalData>()
            {
                new FinalData() { Nights = 1, Hotelcost = 1, Stays = 1, },
                new FinalData() { Nights = 2, Hotelcost = 2, Stays = 2, },
                new FinalData() { Nights = 3, Hotelcost = 3, Stays = 3, },
                new FinalData() { Nights = 4, Hotelcost = 4, Stays = 4, },
            };
            List<FinalData> results = new List<FinalData>();

            // Act
            results = new HotelAccountTopBottomData().SortList(finalList, sortedBy, sortOrder, howManyString);

            // Assert
            Assert.AreEqual(4, results[0].Stays);
            Assert.AreEqual(3, results[1].Stays);
            Assert.AreEqual(2, results[2].Stays);
            Assert.AreEqual(1, results[3].Stays);
        }

        [TestMethod]
        public void SortList_OrderByNightsAscOrder_ReturnByDaysInAscOrder()
        {
            // Arrange
            string sortedBy = ReportFilters.HOTEL_TOP_BOTTOM_ACCOUNTS_RPT_SORT_BY_NIGHTS, sortOrder = ReportFilters.SORT_ORDER_ASCENDING, howManyString = "4";
            List<FinalData> finalList = new List<FinalData>()
            {
                new FinalData() { Nights = 1, Hotelcost = 1, Stays = 1, },
                new FinalData() { Nights = 2, Hotelcost = 2, Stays = 2, },
                new FinalData() { Nights = 3, Hotelcost = 3, Stays = 3, },
                new FinalData() { Nights = 4, Hotelcost = 4, Stays = 4, },
            };
            List<FinalData> results = new List<FinalData>();

            // Act
            results = new HotelAccountTopBottomData().SortList(finalList, sortedBy, sortOrder, howManyString);

            // Assert
            Assert.AreEqual(1, results[0].Nights);
            Assert.AreEqual(2, results[1].Nights);
            Assert.AreEqual(3, results[2].Nights);
            Assert.AreEqual(4, results[3].Nights);
        }


        [TestMethod]
        public void SortList_OrderBySourceAbbrAscOrder_ReturnBySourceAbbrInAscOrder()
        {
            // Arrange
            string sortedBy = ReportFilters.HOTEL_TOP_BOTTOM_ACCOUNTS_RPT_SORT_BY_SOURCE_ABBR, sortOrder = ReportFilters.SORT_ORDER_ASCENDING, howManyString = "4";
            List<FinalData> finalList = new List<FinalData>()
            {
                new FinalData() { Nights = 1, Hotelcost = 1, Stays = 1, SourceAbbr = "z" },
                new FinalData() { Nights = 1, Hotelcost = 1, Stays = 1, SourceAbbr = "w" },
                new FinalData() { Nights = 1, Hotelcost = 1, Stays = 1, SourceAbbr = "x" },
                new FinalData() { Nights = 1, Hotelcost = 1, Stays = 1, SourceAbbr = "y" },
            };
            List<FinalData> results = new List<FinalData>();

            // Act
            results = new HotelAccountTopBottomData().SortList(finalList, sortedBy, sortOrder, howManyString);

            // Assert
            Assert.AreEqual("w", results[0].SourceAbbr);
            Assert.AreEqual("x", results[1].SourceAbbr);
            Assert.AreEqual("y", results[2].SourceAbbr);
            Assert.AreEqual("z", results[3].SourceAbbr);
        }

        [TestMethod]
        public void GetColumnName_GroupByParentAccount_ReturnParentAccountColumnNameValue()
        {
            // Arrange
            var groupBy = ReportFilters.HOTEL_TOP_BOTTOM_ACCOUNTS_RPT_GROUP_BY_PARENT_ACCOUNT_PARAM_VALUE;
            var results = string.Empty;

            // Act
            results = new HotelAccountTopBottomData().GetColumnName(groupBy);

            // Assert
            Assert.AreEqual(ReportNames.HOTEL_TOP_BOTTOM_ACCOUNTS_PARENT_ACCOUNT_COLUMN_NAME, results);
        }

        [TestMethod]
        public void GetColumnName_GroupByDataSource_ReturnDataSourceColumnNameValue()
        {
            // Arrange
            var groupBy = ReportFilters.HOTEL_TOP_BOTTOM_ACCOUNTS_RPT_GROUP_BY_DATA_SOURCE_PARAM_VALUE;
            var results = string.Empty;

            // Act
            results = new HotelAccountTopBottomData().GetColumnName(groupBy);

            // Assert
            Assert.AreEqual(ReportNames.HOTEL_TOP_BOTTOM_ACCOUNTS_DATA_SOURCE_COLUMN_NAME, results);
        }

        [TestMethod]
        public void GetColumnName_GroupByAccount_ReturnAccountColumnNameValue()
        {
            // Arrange
            var groupBy = ReportFilters.HOTEL_TOP_BOTTOM_ACCOUNTS_RPT_GROUP_BY_ACCOUNT_PARAM_VALUE;
            var results = string.Empty;

            // Act
            results = new HotelAccountTopBottomData().GetColumnName(groupBy);

            // Assert
            Assert.AreEqual(ReportNames.HOTEL_TOP_BOTTOM_ACCOUNTS_ACCOUNT_COLUMN_NAME, results);
        }

    }
}
