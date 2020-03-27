using Microsoft.VisualStudio.TestTools.UnitTesting;

using Domain.Constants;
using iBank.Services.Implementation.ReportPrograms.AccountSummary;
using System.Collections.Generic;
using System.Linq;

using Domain.Models.ReportPrograms.AccountSummary;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.AccountSummary
{
    // Unit test naming convention:
    // [UnitOfWork_StateUnderTest_ExpectedBehavior]
    // Example: public void Sun_NegativeNumberAsFirstParam_ExceptionThrown
    [TestClass]
    public class AccountSummaryRawDataCalculatorGetGroupRecsBasedOnGroupByTest
    {
        [TestMethod]
        public void GetGroupRecsBasedOnGroupBy_GroupByPseudoCity_ReturnDataGroupByPseudoCity()
        {
            // Arrange
            List<RawData> rawDataList = new List<RawData>()
            {
                new RawData() { Account = "1", Pseudocity = "2" },
                new RawData() { Account = "2", Pseudocity = "2" },
                new RawData() { Account = "3", Pseudocity = "3" },
                new RawData() { Account = "4", Pseudocity = "3" }
            };
            string group = ReportFilters.ACCOUNT_SUMMARY_RPT_GROUP_BY_PSEUDOCITY_PARAM_VALUE;
            List<IGrouping<string, RawData>> expectedResults = null;

            // Act
            expectedResults = new AccountSummaryRawDataCalculator().GetGroupRecsBasedOnGroupBy(group, rawDataList);

            // Assert
            Assert.AreEqual(2, expectedResults.Count);
        }

        [TestMethod]
        public void GetGroupRecsBasedOnGroupBy_GroupByBranch_ReturnDataGroupByBranch()
        {
            // Arrange
            List<RawData> rawDataList = new List<RawData>()
            {
                new RawData() { Account = "1", Pseudocity = "2", Branch = "6" },
                new RawData() { Account = "2", Pseudocity = "2", Branch = "6" },
                new RawData() { Account = "3", Pseudocity = "3", Branch = "6" },
                new RawData() { Account = "4", Pseudocity = "3", Branch = "6" },
                new RawData() { Account = "4", Pseudocity = "3", Branch = "7" },
            };
            string group = ReportFilters.ACCOUNT_SUMMARY_RPT_GROUP_BY_BRANCH_PARAM_VALUE;
            List<IGrouping<string, RawData>> expectedResults = null;

            // Act
            expectedResults = new AccountSummaryRawDataCalculator().GetGroupRecsBasedOnGroupBy(group, rawDataList);

            // Assert
            Assert.AreEqual(2, expectedResults.Count);
        }

        [TestMethod]
        public void GetGroupRecsBasedOnGroupBy_GroupByAgentId_ReturnDataGroupByAgentId()
        {
            // Arrange
            List<RawData> rawDataList = new List<RawData>()
            {
                new RawData() { Account = "1", Pseudocity = "2", Branch = "6", AgentId = "8" },
                new RawData() { Account = "2", Pseudocity = "2", Branch = "6", AgentId = "9" },
                new RawData() { Account = "3", Pseudocity = "3", Branch = "6", AgentId = "10" },
                new RawData() { Account = "4", Pseudocity = "3", Branch = "6", AgentId = "11" },
                new RawData() { Account = "4", Pseudocity = "3", Branch = "7", AgentId = "12" },
            };
            string group = ReportFilters.ACCOUNT_SUMMARY_RPT_GROUP_BY_AGEND_ID_PARAM_VALUE;
            List<IGrouping<string, RawData>> expectedResults = null;

            // Act
            expectedResults = new AccountSummaryRawDataCalculator().GetGroupRecsBasedOnGroupBy(group, rawDataList);

            // Assert
            Assert.AreEqual(5, expectedResults.Count);
        }

        [TestMethod]
        public void GetGroupRecsBasedOnGroupBy_GroupByAccount_ReturnDataGroupByAccount()
        {
            // Arrange
            List<RawData> rawDataList = new List<RawData>()
            {
                new RawData() { Account = "1", Pseudocity = "2", Branch = "6", AgentId = "8", Acct = "1" },
                new RawData() { Account = "2", Pseudocity = "2", Branch = "6", AgentId = "9", Acct = "2" },
                new RawData() { Account = "3", Pseudocity = "3", Branch = "6", AgentId = "10", Acct = "3" },
                new RawData() { Account = "4", Pseudocity = "3", Branch = "6", AgentId = "11", Acct = "4" },
                new RawData() { Account = "4", Pseudocity = "3", Branch = "7", AgentId = "12", Acct = "4" },
            };
            string group = ReportFilters.ACCOUNT_SUMMARY_RPT_GROUP_BY_ACCOUNT_PARAM_VALUE;
            List<IGrouping<string, RawData>> expectedResults = null;

            // Act
            expectedResults = new AccountSummaryRawDataCalculator().GetGroupRecsBasedOnGroupBy(group, rawDataList);

            // Assert
            Assert.AreEqual(4, expectedResults.Count);
        }
    }
}
