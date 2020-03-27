using System;

using iBank.Services.Implementation.Shared.BuildWhereHelpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.Shared.BuildWhereHelpers
{
    [TestClass]
    public class WhereClauseBuilderTests
    {
        [TestMethod]
        public void AddToWhereClause_ExistingClauseNotEmpty_NoAndOperatorYet_ReturnExistingAndNewClausesJoinedByAnd()
        {
            var existingClause = "field = 'foo'";
            var newClause = "field = 'bar'";
            var sut = new WhereClauseBuilder();

            var output = sut.AddToWhereClause(existingClause, newClause);

            Assert.AreEqual("field = 'foo' AND field = 'bar'", output);
        }

        [TestMethod]
        public void AddToWhereClause_ExistingClauseNotEmpty_EndsWithAndOperator_ReturnExistingAndNewClausesJoinedByAnd()
        {
            var existingClause = "field = 'foo' AND ";
            var newClause = "field = 'bar'";
            var sut = new WhereClauseBuilder();

            var output = sut.AddToWhereClause(existingClause, newClause);

            Assert.AreEqual("field = 'foo' AND  field = 'bar'", output);
        }

        [TestMethod]
        public void AddToWhereClause_ExistingClauseEmpty_ReturnNewItem()
        {
            var existingClause = "";
            var newClause = "field = 'bar'";
            var sut = new WhereClauseBuilder();

            var output = sut.AddToWhereClause(existingClause, newClause);

            Assert.AreEqual("field = 'bar'", output);
        }
    }
}
