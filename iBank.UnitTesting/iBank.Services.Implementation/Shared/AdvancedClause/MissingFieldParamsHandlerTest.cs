using System.Collections.Generic;
using iBank.Services.Implementation.Shared.AdvancedClause;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.Shared.AdvancedClause
{
    [TestClass]
    public class MissingFieldParamsHandlerTest
    {
        [TestMethod]
        public void ReplaceMissingFieldsAndGetUpdatedAdvanceWhereClause_NoMissingFields_ReturnOriginalAdvanceWhereClause()
        {
            // Arrange
            const string advanceWhereClause = "(ditcode = 'd')";
            var missingFields = new List<string>();

            // Act
            var result = new MissingFieldParamsHandler().ReplaceMissingFieldsAndGetUpdatedAdvanceWhereClause(advanceWhereClause, missingFields);

            // Assert
            Assert.AreEqual(advanceWhereClause, result);
        }

        [TestMethod]
        public void ReplaceMissingFieldsAndGetUpdatedAdvanceWhereClause_1MissingParam_1TotalParams_ReturnUpdatedAdvanceWhereClause()
        {
            // Arrange
            const string advanceWhereClause = "(seat = '4d')";
            var missingFields = new List<string>() { "seat" };

            // Act
            var result = new MissingFieldParamsHandler().ReplaceMissingFieldsAndGetUpdatedAdvanceWhereClause(advanceWhereClause, missingFields);

            // Assert
            Assert.AreEqual("(1 = 0)", result);
        }

        [TestMethod]
        public void ReplaceMissingFieldsAndGetUpdatedAdvanceWhereClause_1MissingParam_2TotalParams_ReturnUpdatedAdvanceWhereClause()
        {
            // Arrange
            const string advanceWhereClause = "(seat = '4d' and fltno = '1113')";
            var missingFields = new List<string>() { "seat" };

            // Act
            var result = new MissingFieldParamsHandler().ReplaceMissingFieldsAndGetUpdatedAdvanceWhereClause(advanceWhereClause, missingFields);

            // Assert
            Assert.AreEqual("(1 = 0 and fltno = '1113')", result);
        }

    }
}
