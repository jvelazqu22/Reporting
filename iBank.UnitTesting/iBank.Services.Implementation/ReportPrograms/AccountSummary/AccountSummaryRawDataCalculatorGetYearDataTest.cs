using iBank.Services.Implementation.ReportPrograms.AccountSummary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

using Domain.Models.ReportPrograms.AccountSummary;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.AccountSummary
{
    // Unit test naming convention:
    // [UnitOfWork_StateUnderTest_ExpectedBehavior]
    // Example: public void Sun_NegativeNumberAsFirstParam_ExceptionThrown
    [TestClass]
    public class AccountSummaryRawDataCalculatorGetYearDataTest
    {
        [TestMethod]
        public void GetPreviousYearData_ListOfPreviousYearData_ReturnZeroForCyTripsAndCyAmt()
        {
            // Arrange
            List<RawData> rawDataList = new List<RawData>()
            {
                new RawData() { plusmin = 1, airchg = 2, Acct = "acct", CyTrips = 1, CyAmt = 2 },
                new RawData() { plusmin = 1, airchg = 2, Acct = "acct", CyTrips = 1, CyAmt = 2 },
                new RawData() { plusmin = 1, airchg = 2, Acct = "acct", CyTrips = 1, CyAmt = 2 },
                new RawData() { plusmin = 1, airchg = 2, Acct = "acct", CyTrips = 1, CyAmt = 2 },
            };
            List<RawData> expectedResults = null;

            // Act
            expectedResults = new AccountSummaryRawDataCalculator().GetPreviousYearData(rawDataList);

            // Assert
            foreach(var result in expectedResults)
            {
                Assert.AreEqual(2, result.PyAmt);
                Assert.AreEqual(1, result.PyTrips);
                Assert.AreEqual(0, result.CyAmt);
                Assert.AreEqual(0, result.CyTrips);
            }
        }

        [TestMethod]
        public void GetCurrentYearData_ListOfCurrenYearData_ReturnZeroForPyTripsAndPyAmt()
        {
            // Arrange
            List<RawData> rawDataList = new List<RawData>()
            {
                new RawData() { plusmin = 1, airchg = 2, Acct = "acct", CyTrips = 1, CyAmt = 2 },
                new RawData() { plusmin = 1, airchg = 2, Acct = "acct", CyTrips = 1, CyAmt = 2 },
                new RawData() { plusmin = 1, airchg = 2, Acct = "acct", CyTrips = 1, CyAmt = 2 },
                new RawData() { plusmin = 1, airchg = 2, Acct = "acct", CyTrips = 1, CyAmt = 2 },
            };
            List<RawData> expectedResults = null;

            // Act
            expectedResults = new AccountSummaryRawDataCalculator().GetCurrentYearData(rawDataList);

            // Assert
            foreach (var result in expectedResults)
            {
                Assert.AreEqual(0, result.PyAmt);
                Assert.AreEqual(0, result.PyTrips);
                Assert.AreEqual(2, result.CyAmt);
                Assert.AreEqual(1, result.CyTrips);
            }
        }
    }
}
