using System;

using iBank.Server.Utilities.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Server.Utilities.Helpers
{
    [TestClass]
    public class SharedProceduresTests
    {
        //[TestMethod]
        public void FixWildcards_ContainsSqlWildcard_ReturnClauseWithLike()
        {
            var clause = "foo%";

            var output = SharedProcedures.FixWildcard(clause);

            Assert.AreEqual($" LIKE 'FOO%'", output);
        }

        //[TestMethod]
        public void FixWildcards_NoWildCards_ReturnClauseWithEqual()
        {
            var clause = "foo";

            var output = SharedProcedures.FixWildcard(clause);

            Assert.AreEqual($" = 'FOO'", output);
        }

        //[TestMethod]
        public void FixWildcards_ApostropheInDataValue_EscapeApostrophe()
        {
            var clause = @"foo's";

            var output = SharedProcedures.FixWildcard(clause);

            Assert.AreEqual(@" = 'FOO''S'", output);
        }

       // [TestMethod]
        public void FixWildcards_AsteriskInClause_ReplaceWithPercentSign()
        {
            var clause = "foo*";

            var output = SharedProcedures.FixWildcard(clause);

            Assert.AreEqual(@" LIKE 'FOO%'", output);
        }

        //[TestMethod]
        public void FixWildcards_QuestionMarkInClause_ReplaceWithUnderscore()
        {
            var clause = "foo?";

            var output = SharedProcedures.FixWildcard(clause);

            Assert.AreEqual(@" LIKE 'FOO_'", output);
        }

        //[TestMethod]
        public void FixWildcards_ClauseHasBeenProcessedForLIKE_ReturnString()
        {
            var clause = " LIKE 'FOO%'";

            var output = SharedProcedures.FixWildcard(clause);

            Assert.AreEqual(clause, output);
        }

       // [TestMethod]
        public void FixWildcards_ClauseHasBeenProcessedForEquals_ReturnString()
        {
            var clause = " = 'FOO'";

            var output = SharedProcedures.FixWildcard(clause);

            Assert.AreEqual(clause, output);
        }
    }
}
