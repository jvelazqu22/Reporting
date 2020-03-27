using System.Collections.Generic;

using Domain.Constants;
using Domain.Models.ReportPrograms.HotelAccountTopBottom;
using Domain.Orm.Classes;

using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.ReportPrograms.HotelAccountTopBottom;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.Client;
using iBankDomain.RepositoryInterfaces;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.HotelAccountTopBottom
{
    // Unit test naming convention:
    // [UnitOfWork_StateUnderTest_ExpectedBehavior]
    // Example: public void Sun_NegativeNumberAsFirstParam_ExceptionThrown
    [TestClass]
    public class HotelAccountTopBottomRawDataCalculatorTest
    {

        [TestMethod]
        public void GetAccount_GroupByParentAccountAndAccountNumberField_ReturnsSourceAbbreviation()
        {
            // Arrange
            var acct = "123";
            var accountName = "acct-name";
            var parentAcctName = "parent-acct-name";
            var globals = new ReportGlobals();
            var groupBy = ReportFilters.HOTEL_TOP_BOTTOM_ACCOUNTS_RPT_GROUP_BY_PARENT_ACCOUNT_PARAM_VALUE;
            var sourceAbbreviation = "Abbr";
            var agency = "Demo";
            var groupByField = ReportFilters.HOTEL_TOP_BOTTOM_ACCOUNTS_RPT_GROUP_BY_ACCOUNT_PARAM_VALUE;
            var excpectedResult = "test1";
            var result = string.Empty;
            var masterAccountInformation = new List<MasterAccountInformation>() { new MasterAccountInformation() { AccountId = acct, AccountName = accountName, ParentAccount = parentAcctName } };
            var parentMasterAccountInformation = new List<MasterAccountInformation>() { new MasterAccountInformation() { AccountId = parentAcctName, AccountName = parentAcctName } };
            var masterSourceInfoList = new List<MasterSourceInformation>() { new MasterSourceInformation() { Agency = agency, SourceAbbreviation = sourceAbbreviation, SourceDescription = excpectedResult } };

            var getAllMasterAccountsQuery = new Mock<IQuery<IList<MasterAccountInformation>>>();
            getAllMasterAccountsQuery.Setup(r => r.ExecuteQuery()).Returns(masterAccountInformation);

            var getMasterAgencySourcesQuery = new Mock<IQuery<IList<MasterSourceInformation>>>();
            getMasterAgencySourcesQuery.Setup(r => r.ExecuteQuery()).Returns(masterSourceInfoList);

            var getAllParentAccountsQuery = new Mock<IQuery<IList<MasterAccountInformation>>>();
            getAllParentAccountsQuery.Setup(r => r.ExecuteQuery()).Returns(parentMasterAccountInformation);

            // Act 
            result = new HotelAccountTopBottomRawDataCalculator(new ClientFunctions()).GetAcct(getAllMasterAccountsQuery.Object, groupBy, acct, sourceAbbreviation, getAllParentAccountsQuery.Object);

            // Assert
            Assert.AreEqual(result, parentAcctName);
        }

        [TestMethod]
        public void GetAccount_GroupByDataSourceAndAccountNumberField_ReturnsSourceAbbreviation()
        {
            // Arrange
            var acct = string.Empty;
            var globals = new ReportGlobals();
            var groupBy = ReportFilters.CAR_TOP_BOTTOM_ACCOUNTS_RPT_GROUP_BY_DATA_SOURCE_PARAM_VALUE;
            var sourceAbbreviation = "DEMO";
            var agency = "DEMO";
            var groupByField = ReportFilters.CAR_TOP_BOTTOM_ACCOUNTS_RPT_GROUPING_FIELD_ACCT_NUM_PARAM_VALUE;
            var result = string.Empty;
            var getAllMasterAccountsQuery = new Mock<IQuery<IList<MasterAccountInformation>>>();
            getAllMasterAccountsQuery.Setup(r => r.ExecuteQuery()).Returns(new List<MasterAccountInformation>());

            var getMasterAgencySourcesQuery = new Mock<IQuery<IList<MasterSourceInformation>>>();
            getMasterAgencySourcesQuery.Setup(r => r.ExecuteQuery()).Returns(new List<MasterSourceInformation>());

            var getAllParentAccountsQuery = new Mock<IQuery<IList<MasterAccountInformation>>>();
            getAllParentAccountsQuery.Setup(r => r.ExecuteQuery()).Returns(new List<MasterAccountInformation>());

            // Act 
            result = new HotelAccountTopBottomRawDataCalculator(new ClientFunctions()).GetAcct(getAllMasterAccountsQuery.Object, groupBy, acct, sourceAbbreviation, getAllParentAccountsQuery.Object);

            // Assert
            Assert.AreEqual(sourceAbbreviation, result);
        }

        [TestMethod]
        public void GetRawListTotalStays_RawDatListWithStayValues_ReturnsSumOfStays()
        {
            // Arrange
            var rawDataList = new List<RawData>()
            {
                new RawData() { Stays = 1 },
                new RawData() { Stays = 1 },
                new RawData() { Stays = 1 },
                new RawData() { Stays = 1 },
            };

            int result = 0;
            int expectedResult = 4;

            // Act
            result = new HotelAccountTopBottomRawDataCalculator(new ClientFunctions()).GetRawListTotalStays(rawDataList);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void GetRawListTotalNights_RawDatListWithNightValues_ReturnsSumONights()
        {
            // Arrange
            var rawDataList = new List<RawData>()
            {
                new RawData() { Stays = 1, nights = 1 },
                new RawData() { Stays = 1, nights = 1 },
                new RawData() { Stays = 1, nights = 1 },
                new RawData() { Stays = 1, nights = 1 },
            };

            int result = 0;
            int expectedResult = 4;

            // Act
            result = new HotelAccountTopBottomRawDataCalculator(new ClientFunctions()).GetRawListTotalNights(rawDataList);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void GetRawListTotalCosts_RawDatListWithCostValues_ReturnsSumOfCosts()
        {
            // Arrange
            var rawDataList = new List<RawData>()
            {
                new RawData() { Stays = 1, nights = 1, hotelcost = 1 },
                new RawData() { Stays = 1, nights = 1, hotelcost = 1 },
                new RawData() { Stays = 1, nights = 1, hotelcost = 1 },
                new RawData() { Stays = 1, nights = 1, hotelcost = 1 },
            };

            decimal result = 0;
            decimal expectedResult = 4;

            // Act
            result = new HotelAccountTopBottomRawDataCalculator(new ClientFunctions()).GetRawListTotalHotelCosts(rawDataList);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }
    }
}
