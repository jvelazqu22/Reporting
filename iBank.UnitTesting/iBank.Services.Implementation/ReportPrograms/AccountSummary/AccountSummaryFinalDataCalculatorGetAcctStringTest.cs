using Microsoft.VisualStudio.TestTools.UnitTesting;

using Domain.Constants;
using Domain.Models.ReportPrograms.AccountSummary;

using iBank.Services.Implementation.ReportPrograms.AccountSummary;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.AccountSummary
{
    // Unit test naming convention:
    // [UnitOfWork_StateUnderTest_ExpectedBehavior]
    // Example: public void Sun_NegativeNumberAsFirstParam_ExceptionThrown
    [TestClass]
    public class AccountSummaryFinalDataCalculatorGetAcctStringTest
    {
        [TestMethod]
        public void GetAcctString_GroupByPseudoCity_ReturnPseudoCityString()
        {
            // Arrange
            RawData item = new RawData() { Pseudocity = "pseudocity", Branch = "branch", AgentId = "123" };
            string group = ReportFilters.ACCOUNT_SUMMARY_RPT_GROUP_BY_PSEUDOCITY_PARAM_VALUE;
            string result = string.Empty;
            string exptedResult = "pseudocity";

            // Act
            result = new AccountSummaryFinalDataCalculator().GetAcctString(item, group);

            // Assert
            Assert.AreEqual(exptedResult, result);
        }

        [TestMethod]
        public void GetAcctString_GroupByBranch_ReturnBranchString()
        {
            // Arrange
            RawData item = new RawData() { Pseudocity = "pseudocity", Branch = "branch", AgentId = "123" };
            string group = ReportFilters.ACCOUNT_SUMMARY_RPT_GROUP_BY_BRANCH_PARAM_VALUE;
            string result = string.Empty;
            string exptedResult = "branch";

            // Act
            result = new AccountSummaryFinalDataCalculator().GetAcctString(item, group);

            // Assert
            Assert.AreEqual(exptedResult, result);
        }

        [TestMethod]
        public void GetAcctString_GroupByAgentId_ReturnAgentIdString()
        {
            // Arrange
            RawData item = new RawData() { Pseudocity = "pseudocity", Branch = "branch", AgentId = "123" };
            string group = ReportFilters.ACCOUNT_SUMMARY_RPT_GROUP_BY_AGEND_ID_PARAM_VALUE;
            string result = string.Empty;
            string exptedResult = "123";

            // Act
            result = new AccountSummaryFinalDataCalculator().GetAcctString(item, group);

            // Assert
            Assert.AreEqual(exptedResult, result);
        }
    }
}
