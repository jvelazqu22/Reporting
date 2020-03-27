using iBank.Services.Implementation.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.Utilities
{
    [TestClass]
    public class SqlTransformerTests
    {
        [TestMethod]
        public void TranslateLegsAndMktSegsColumnNames_SeparateWordLegsToSegs_ReturnsNewWord()
        {
            var clause = "where rdepdate = foo";
            var expected = "where sdepdate = foo";

            clause = new SqlTransformer().TranslateLegsAndMktSegsColumnNames(clause, true);

            Assert.AreEqual(expected, clause);
        }

        [TestMethod]
        public void TranslateLegsAndMktSegsColumnNames_WordIsSubstringOfAnotherWordLegsToSegs_ReturnsNewWordAsSubstring()
        {
            var clause = "where (rdepdate = foo)";
            var expected = "where (sdepdate = foo)";

            clause = new SqlTransformer().TranslateLegsAndMktSegsColumnNames(clause, true);

            Assert.AreEqual(expected, clause);
        }

        [TestMethod]
        public void TranslateLegsAndMktSegsColumnNames_WordNotInListLegsToSegs_SameStringReturned()
        {
            var clause = "where (bar = foo)";
            var expected = "where (bar = foo)";

            clause = new SqlTransformer().TranslateLegsAndMktSegsColumnNames(clause, true);

            Assert.AreEqual(expected, clause);
        }

        [TestMethod]
        public void TranslateLegsAndMktSegsColumnNames_LegsToSegs_DontDuplicateName()
        {
            var clause = "where sarrtime = foo";
            var expected = "where sarrtime = foo";

            clause = new SqlTransformer().TranslateLegsAndMktSegsColumnNames(clause, true);

            Assert.AreEqual(expected, clause);
        }

        [TestMethod]
        public void TranslateLegsAndMktSegsColumnNames_SeparateWordSegsToLegs_ReturnsNewWord()
        {
            var clause = "where sdepdate = foo";
            var expected = "where rdepdate = foo";

            clause = new SqlTransformer().TranslateLegsAndMktSegsColumnNames(clause, false);

            Assert.AreEqual(expected, clause);
        }

        [TestMethod]
        public void TranslateLegsAndMktSegsColumnNames_WordIsSubstringOfAnotherWordSegsToLegs_ReturnsNewWordAsSubstring()
        {
            var clause = "where (sdepdate = foo)";
            var expected = "where (rdepdate = foo)";

            clause = new SqlTransformer().TranslateLegsAndMktSegsColumnNames(clause, false);

            Assert.AreEqual(expected, clause);
        }

        [TestMethod]
        public void TranslateLegsAndMktSegsColumnNames_WordNotInListSegsToLegs_SameStringReturned()
        {
            var clause = "where (bar = foo)";
            var expected = "where (bar = foo)";

            clause = new SqlTransformer().TranslateLegsAndMktSegsColumnNames(clause, false);

            Assert.AreEqual(expected, clause);
        }

        [TestMethod]
        public void TranslateLegsAndMktSegsColumnNames_SegsToLegs_DontDuplicateName()
        {
            var clause = "where rarrtime = foo";
            var expected = "where rarrtime = foo";

            clause = new SqlTransformer().TranslateLegsAndMktSegsColumnNames(clause, false);

            Assert.AreEqual(expected, clause);
        }

    }
}
