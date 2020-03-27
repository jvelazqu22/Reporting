using Domain.Constants;

using iBank.Services.Implementation.ReportPrograms.CarTopBottomAccounts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

using Domain.Models.ReportPrograms.CarTopBottomAccountsReport;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.CarTopBottomAccounts
{
    [TestClass]
    public class CarTopBottomAccountsDataTest
    {
        // Unit test naming convention:
        // [UnitOfWork_StateUnderTest_ExpectedBehavior]
        // Example: public void Sun_NegativeNumberAsFirstParam_ExceptionThrown

        [TestMethod]
        public void SortList_OrderByVolumeBooked_ReturnInDescOrder()
        {
            // Arrange
            string sortedBy = ReportFilters.CAR_TOP_BOTTOM_ACCOUNTS_RPT_SORT_BY_CAR_COST, sortOrder = ReportFilters.SORT_ORDER_DECENDING, howManyString = "4";
            List<FinalData> finalList = new List<FinalData>()
            {
                new FinalData() { VolumeBooked = 1, Carcost = 1 },
                new FinalData() { VolumeBooked = 2, Carcost = 2 },
                new FinalData() { VolumeBooked = 3, Carcost = 3 },
                new FinalData() { VolumeBooked = 4, Carcost = 4 },
            };
            List<FinalData> results = new List<FinalData>();

            // Act
            results = new CarTopBottomAccountsData().SortList(finalList, sortedBy, sortOrder, howManyString);

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
            string sortedBy = ReportFilters.CAR_TOP_BOTTOM_ACCOUNTS_RPT_SORT_BY_RENTALS, sortOrder = ReportFilters.SORT_ORDER_DECENDING, howManyString = "4";
            List<FinalData> finalList = new List<FinalData>()
            {
                new FinalData() { VolumeBooked = 1, Carcost = 1, Rentals = 1 },
                new FinalData() { VolumeBooked = 2, Carcost = 2, Rentals = 2 },
                new FinalData() { VolumeBooked = 3, Carcost = 3, Rentals = 3 },
                new FinalData() { VolumeBooked = 4, Carcost = 4, Rentals = 4 },
            };
            List<FinalData> results = new List<FinalData>();

            // Act
            results = new CarTopBottomAccountsData().SortList(finalList, sortedBy, sortOrder, howManyString);

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
            string sortedBy = ReportFilters.CAR_TOP_BOTTOM_ACCOUNTS_RPT_SORT_BY_DAYS, sortOrder = ReportFilters.SORT_ORDER_ASCENDING, howManyString = "4";
            List<FinalData> finalList = new List<FinalData>()
            {
                new FinalData() { VolumeBooked = 1, Carcost = 1, Rentals = 1, Days = 4, },
                new FinalData() { VolumeBooked = 2, Carcost = 2, Rentals = 2, Days = 3, },
                new FinalData() { VolumeBooked = 3, Carcost = 3, Rentals = 3, Days = 2, },
                new FinalData() { VolumeBooked = 4, Carcost = 4, Rentals = 4, Days = 1, },
            };
            List<FinalData> results = new List<FinalData>();

            // Act
            results = new CarTopBottomAccountsData().SortList(finalList, sortedBy, sortOrder, howManyString);

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
            string sortedBy = ReportFilters.CAR_TOP_BOTTOM_ACCOUNTS_RPT_SORT_BY_AVG_BOOK, sortOrder = ReportFilters.SORT_ORDER_ASCENDING, howManyString = "4";
            List<FinalData> finalList = new List<FinalData>()
            {
                new FinalData() { VolumeBooked = 1, Carcost = 1, Rentals = 1, Days = 4, avgbook = 4 },
                new FinalData() { VolumeBooked = 2, Carcost = 2, Rentals = 2, Days = 3, avgbook = 3 },
                new FinalData() { VolumeBooked = 3, Carcost = 3, Rentals = 3, Days = 2, avgbook = 2 },
                new FinalData() { VolumeBooked = 4, Carcost = 4, Rentals = 4, Days = 1, avgbook = 1 },
            };
            List<FinalData> results = new List<FinalData>();

            // Act
            results = new CarTopBottomAccountsData().SortList(finalList, sortedBy, sortOrder, howManyString);

            // Assert
            Assert.AreEqual(1, results[0].avgbook);
            Assert.AreEqual(2, results[1].avgbook);
            Assert.AreEqual(3, results[2].avgbook);
            Assert.AreEqual(4, results[3].avgbook);
        }

        [TestMethod]
        public void SortList_OrderBySourceAbbrAscOrder_ReturnBySourceAbbrInAscOrder()
        {
            // Arrange
            string sortedBy = ReportFilters.CAR_TOP_BOTTOM_ACCOUNTS_RPT_SORT_BY_SOURCE_ABBR, sortOrder = ReportFilters.SORT_ORDER_ASCENDING, howManyString = "4";
            List<FinalData> finalList = new List<FinalData>()
            {
                new FinalData() { VolumeBooked = 1, Carcost = 1, Rentals = 1, Days = 4, avgbook = 4, SourceAbbr = "z" },
                new FinalData() { VolumeBooked = 2, Carcost = 2, Rentals = 2, Days = 3, avgbook = 3, SourceAbbr = "w" },
                new FinalData() { VolumeBooked = 3, Carcost = 3, Rentals = 3, Days = 2, avgbook = 2, SourceAbbr = "x" },
                new FinalData() { VolumeBooked = 4, Carcost = 4, Rentals = 4, Days = 1, avgbook = 1, SourceAbbr = "y" },
            };
            List<FinalData> results = new List<FinalData>();

            // Act
            results = new CarTopBottomAccountsData().SortList(finalList, sortedBy, sortOrder, howManyString);

            // Assert
            Assert.AreEqual(results[0].SourceAbbr, "w");
            Assert.AreEqual(results[1].SourceAbbr, "x");
            Assert.AreEqual(results[2].SourceAbbr, "y");
            Assert.AreEqual(results[3].SourceAbbr, "z");
        }

        [TestMethod]
        public void GetColumnName_GroupByParentAccount_ReturnParentAccountColumnNameValue()
        {
            // Arrange
            var groupBy = ReportFilters.CAR_TOP_BOTTOM_ACCOUNTS_RPT_GROUP_BY_PARENT_ACCOUNT_PARAM_VALUE;
            var results = string.Empty;

            // Act
            results = new CarTopBottomAccountsData().GetColumnName(groupBy);

            // Assert
            Assert.AreEqual(ReportNames.CAR_TOP_BOTTOM_ACCOUNTS_PARENT_ACCOUNT_COLUMN_NAME, results);
        }

        [TestMethod]
        public void GetColumnName_GroupByDataSource_ReturnDataSourceColumnNameValue()
        {
            // Arrange
            var groupBy = ReportFilters.CAR_TOP_BOTTOM_ACCOUNTS_RPT_GROUP_BY_DATA_SOURCE_PARAM_VALUE;
            var results = string.Empty;

            // Act
            results = new CarTopBottomAccountsData().GetColumnName(groupBy);

            // Assert
            Assert.AreEqual(ReportNames.CAR_TOP_BOTTOM_ACCOUNTS_DATA_SOURCE_COLUMN_NAME, results);
        }

        [TestMethod]
        public void GetColumnName_GroupByAccount_ReturnAccountColumnNameValue()
        {
            // Arrange
            var groupBy = ReportFilters.CAR_TOP_BOTTOM_ACCOUNTS_RPT_GROUP_BY_ACCOUNT_PARAM_VALUE;
            var results = string.Empty;

            // Act
            results = new CarTopBottomAccountsData().GetColumnName(groupBy);

            // Assert
            Assert.AreEqual(ReportNames.CAR_TOP_BOTTOM_ACCOUNTS_ACCOUNT_COLUMN_NAME, results);
        }

    }
}
