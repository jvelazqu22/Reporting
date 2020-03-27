using Domain.Constants;
using iBank.Services.Implementation.ReportPrograms.AccountSummary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.AccountSummary
{
    // Unit test naming convention:
    // [UnitOfWork_StateUnderTest_ExpectedBehavior]
    // Example: public void Sun_NegativeNumberAsFirstParam_ExceptionThrown
    [TestClass]
    public class AccountSummaryRawDataCalculatorGetFieldNameBaseOnGroupByTest
    {
        [TestMethod]
        public void GetFieldNameBaseOnGroupBy_GroupByPseudoCity_ReturnPseudoCity()
        {
            // Arrange
            string group = ReportFilters.ACCOUNT_SUMMARY_RPT_GROUP_BY_PSEUDOCITY_PARAM_VALUE;
            string expectedResult = "pseudocity";
            string result;

            // Act
            result = new AccountSummaryRawDataCalculator().GetFieldNameBaseOnGroupBy(group);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void GetFieldNameBaseOnGroupBy_GroupByBranch_ReturnBranch()
        {
            // Arrange
            string group = ReportFilters.ACCOUNT_SUMMARY_RPT_GROUP_BY_BRANCH_PARAM_VALUE;
            string expectedResult = "branch";
            string result;

            // Act
            result = new AccountSummaryRawDataCalculator().GetFieldNameBaseOnGroupBy(group);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void GetFieldNameBaseOnGroupBy_GroupByAgentId_ReturnAgentId()
        {
            // Arrange
            string group = ReportFilters.ACCOUNT_SUMMARY_RPT_GROUP_BY_AGEND_ID_PARAM_VALUE;
            string expectedResult = "agentid";
            string result;

            // Act
            result = new AccountSummaryRawDataCalculator().GetFieldNameBaseOnGroupBy(group);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void GetFieldNameBaseOnGroupBy_GroupByAccount_ReturnAccount()
        {
            // Arrange
            string group = ReportFilters.ACCOUNT_SUMMARY_RPT_GROUP_BY_ACCOUNT_PARAM_VALUE;
            string expectedResult = "acct";
            string result;

            // Act
            result = new AccountSummaryRawDataCalculator().GetFieldNameBaseOnGroupBy(group);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }
    }
}
