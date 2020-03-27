using Domain.Constants;
using iBank.Services.Implementation.ReportPrograms.AccountSummary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.AccountSummary
{
    // Unit test naming convention:
    // [UnitOfWork_StateUnderTest_ExpectedBehavior]
    // Example: public void Sun_NegativeNumberAsFirstParam_ExceptionThrown
    [TestClass]
    public class AccountSummaryRawDataCalculatorGetColumnHeaderBaseOnGroupByTest
    {
        [TestMethod]
        public void GetColumnHeaderBaseOnGroupBy_GroupByPseudoCity_ReturnPseudoCity()
        {
            // Arrange
            string group = ReportFilters.ACCOUNT_SUMMARY_RPT_GROUP_BY_PSEUDOCITY_PARAM_VALUE;
            string expectedResult = "Pseudocity";
            string result;

            // Act
            result = new AccountSummaryRawDataCalculator().GetColumnHeaderBaseOnGroupBy(group);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void GetColumnHeaderBaseOnGroupBy_GroupByBranch_ReturnBranch()
        {
            // Arrange
            string group = ReportFilters.ACCOUNT_SUMMARY_RPT_GROUP_BY_BRANCH_PARAM_VALUE;
            string expectedResult = "Branch";
            string result;

            // Act
            result = new AccountSummaryRawDataCalculator().GetColumnHeaderBaseOnGroupBy(group);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void GetColumnHeaderBaseOnGroupBy_GroupByAgentId_ReturnAgentId()
        {
            // Arrange
            string group = ReportFilters.ACCOUNT_SUMMARY_RPT_GROUP_BY_AGEND_ID_PARAM_VALUE;
            string expectedResult = "AgentId";
            string result;

            // Act
            result = new AccountSummaryRawDataCalculator().GetColumnHeaderBaseOnGroupBy(group);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void GetColumnHeaderBaseOnGroupBy_GroupByParentAccount_ReturnParentAccount()
        {
            // Arrange
            string group = ReportFilters.ACCOUNT_SUMMARY_RPT_GROUP_BY_PARENT_ACCOUNT_PARAM_VALUE;
            string expectedResult = "Parent Account";
            string result;

            // Act
            result = new AccountSummaryRawDataCalculator().GetColumnHeaderBaseOnGroupBy(group);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void GetColumnHeaderBaseOnGroupBy_GroupByAccount_ReturnAccount()
        {
            // Arrange
            string group = ReportFilters.ACCOUNT_SUMMARY_RPT_GROUP_BY_ACCOUNT_PARAM_VALUE;
            string expectedResult = "Account";
            string result;

            // Act
            result = new AccountSummaryRawDataCalculator().GetColumnHeaderBaseOnGroupBy(group);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }
    }
}
