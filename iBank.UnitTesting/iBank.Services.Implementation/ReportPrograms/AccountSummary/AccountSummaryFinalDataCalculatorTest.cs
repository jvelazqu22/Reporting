using Microsoft.VisualStudio.TestTools.UnitTesting;

using Domain.Constants;
using iBank.Services.Implementation.ReportPrograms.AccountSummary;
using System.Collections.Generic;
using System.Linq;

using Domain.Models.ReportPrograms.AccountSummary;
using Domain.Orm.Classes;

using Moq;

using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.Client;
using iBankDomain.RepositoryInterfaces;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.AccountSummary
{
    // Unit test naming convention:
    // [UnitOfWork_StateUnderTest_ExpectedBehavior]
    // Example: public void Sun_NegativeNumberAsFirstParam_ExceptionThrown
    [TestClass]
    public class AccountSummaryFinalDataCalculatorTest
    {
        [TestMethod]
        public void AddFinalDataRecordsByPseudoCityBranchOrAgenIdForXlsOrCvs_RawDataListWith4Records_ReturnFinalListOf10Records()
        {
            // Arrange
            List<RawData> rawDataList = new List<RawData>()
            {
                new RawData() { PyAmt = 1, PyTrips = 1, CyAmt = 1, CyTrips = 1, Acct = "acct", Pseudocity = "TBD" },
                new RawData() { PyAmt = 1, PyTrips = 1, CyAmt = 1, CyTrips = 1, Acct = "acct", Pseudocity = "TBD" },
                new RawData() { PyAmt = 1, PyTrips = 1, CyAmt = 1, CyTrips = 1, Acct = "acct", Pseudocity = "TBD" },
                new RawData() { PyAmt = 1, PyTrips = 1, CyAmt = 1, CyTrips = 1, Acct = "acct", Pseudocity = "TBD" },
            };
            List<IGrouping<string, RawData>> groupedRecs = rawDataList.GroupBy(g => g.Acct).ToList();

            string group = ReportFilters.ACCOUNT_SUMMARY_RPT_GROUP_BY_PSEUDOCITY_PARAM_VALUE;
            FinalData expectedResult = null;
            List<FinalData> finalDataList = new List<FinalData>();

            // Act
            new AccountSummaryFinalDataCalculator().AddFinalDataRecordsByPseudoCityBranchOrAgenIdForXlsOrCvs(group, groupedRecs, finalDataList);
            expectedResult = finalDataList.FirstOrDefault();

            // Assert
            Assert.AreEqual(1, finalDataList.Count);
            Assert.AreEqual(4, expectedResult.CyAmt);
            Assert.AreEqual(4, expectedResult.CyTrips);
            Assert.AreEqual(4, expectedResult.PyAmt);
            Assert.AreEqual(4, expectedResult.PyTrips);
            Assert.AreNotEqual(string.Empty, expectedResult.Acct);
            Assert.AreEqual(string.Empty, expectedResult.AcctDesc);
        }

        [TestMethod]
        public void AddFinalDataRecordsNogGroupByPseudoCityBranchOrAgenIdForXlsOrCvs_RawDataListWith4Records_ReturnFinalListOf10Records()
        {
            // Arrange
            List<RawData> rawDataList = new List<RawData>()
            {
                new RawData() { PyAmt = 1, PyTrips = 1, CyAmt = 1, CyTrips = 1, Acct = "acct" },
                new RawData() { PyAmt = 1, PyTrips = 1, CyAmt = 1, CyTrips = 1, Acct = "acct" },
                new RawData() { PyAmt = 1, PyTrips = 1, CyAmt = 1, CyTrips = 1, Acct = "acct" },
                new RawData() { PyAmt = 1, PyTrips = 1, CyAmt = 1, CyTrips = 1, Acct = "acct" },
            };
            List<IGrouping<string, RawData>> groupedRecs = rawDataList.GroupBy(g => g.Acct).ToList();

            string group = ReportFilters.ACCOUNT_SUMMARY_RPT_GROUP_BY_ACCOUNT_PARAM_VALUE;
            FinalData expectedResult = null;
            List<FinalData> finalDataList = new List<FinalData>();

            // Act
            new AccountSummaryFinalDataCalculator().AddFinalDataRecordsNogGroupByPseudoCityBranchOrAgenIdForXlsOrCvs(group, groupedRecs, finalDataList);
            expectedResult = finalDataList.FirstOrDefault();

            // Assert
            Assert.AreEqual(1, finalDataList.Count);
            Assert.AreEqual(4, expectedResult.CyAmt);
            Assert.AreEqual(4, expectedResult.CyTrips);
            Assert.AreEqual(4, expectedResult.PyAmt);
            Assert.AreEqual(4, expectedResult.PyTrips);
            Assert.AreEqual(string.Empty, expectedResult.Acct);
            Assert.AreEqual(string.Empty, expectedResult.AcctDesc);
        }

        [TestMethod]
        public void AddFinalDataRecordsUsingAccountDescription_RawDataListWith4Records_Return1FinalORecord()
        {
            // Arrange
            List<RawData> rawDataList = new List<RawData>()
            {
                new RawData() { PyAmt = 1, PyTrips = 1, CyAmt = 1, CyTrips = 1, Acct = "acct" },
                new RawData() { PyAmt = 1, PyTrips = 1, CyAmt = 1, CyTrips = 1, Acct = "acct" },
                new RawData() { PyAmt = 1, PyTrips = 1, CyAmt = 1, CyTrips = 1, Acct = "acct" },
                new RawData() { PyAmt = 1, PyTrips = 1, CyAmt = 1, CyTrips = 1, Acct = "acct" },
            };
            List<IGrouping<string, RawData>> group = rawDataList.GroupBy(g => g.Acct).ToList();
            var getAllMasterAccountsQuery = new Mock<IQuery<IList<MasterAccountInformation>>>();
            getAllMasterAccountsQuery.Setup(r => r.ExecuteQuery()).Returns(new List<MasterAccountInformation>());

            FinalData expectedResult = null;
            List<FinalData> finalDataList = new List<FinalData>();
            ReportGlobals globals = new ReportGlobals() { Agency = "something" };
            ClientFunctions clientFunctions = new ClientFunctions();

            // Act
            new AccountSummaryFinalDataCalculator().AddFinalDataRecordsUsingAccountDescription(getAllMasterAccountsQuery.Object, group.First(), clientFunctions, finalDataList, rawDataList.First(), globals);
            expectedResult = finalDataList.FirstOrDefault();

            // Assert
            Assert.AreEqual(1, finalDataList.Count);
            Assert.AreEqual(4, expectedResult.CyAmt);
            Assert.AreEqual(4, expectedResult.CyTrips);
            Assert.AreEqual(4, expectedResult.PyAmt);
            Assert.AreEqual(4, expectedResult.PyTrips);
            Assert.AreEqual("acct", expectedResult.Acct);
            Assert.AreEqual("acct NOT FOUND", expectedResult.AcctDesc);
        }

    }
}
