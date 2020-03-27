using iBank.Services.Implementation.Shared.AdvancedClause;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace iBank.UnitTesting.iBank.Services.Implementation.Shared.AdvancedClause
{
    [TestClass]
    public class ClauseParserTests
    {
        private readonly List<string> _reservationTables = new List<string> { "ibfoo", "ibfoo2" };

        private readonly List<string> _backOfficeTables = new List<string> { "hibfoo", "hibfoo2" };
        private readonly ClauseParser _clauseParser = new ClauseParser();

        [TestMethod]
        public void ParseFromClause_ReservationTables_ContainsTable_ReturnTable()
        {
            var fromClause = "from T1.ibfoo AND T2.ibar ";

            var output = _clauseParser.GetTablesThatExistInClause(fromClause, _reservationTables).ToList();

            Assert.AreEqual(1, output.Count);
            Assert.AreEqual("ibfoo", output[0]);
        }

        [TestMethod]
        public void ParseFromClause_ReservationTables_DoesNotContainTable_ReturnEmpty()
        {
            var fromClause = "from T1.ibblah AND T2.ibar ";

            var output = _clauseParser.GetTablesThatExistInClause(fromClause, _reservationTables).ToList();

            Assert.AreEqual(0, output.Count);
        }

        [TestMethod]
        public void ParseFromClause_ReservationTables_ContainsMultipleTables_ReturnMultipleTables()
        {
            var fromClause = "from T1.ibfoo AND T2.ibfoo2 ";

            var output = _clauseParser.GetTablesThatExistInClause(fromClause, _reservationTables).ToList();

            Assert.AreEqual(2, output.Count);
            Assert.AreEqual(true, output.Any(x => x.Equals("ibfoo")));
            Assert.AreEqual(true, output.Any(x => x.Equals("ibfoo2")));
        }

        [TestMethod]
        public void ParseFromClause_ReservationTables_ContainsTableInDifferentCase_ReturnTable()
        {
            var fromClause = "from T1.IBFOO AND T2.ibar ";

            var output = _clauseParser.GetTablesThatExistInClause(fromClause, _reservationTables).ToList();

            Assert.AreEqual(1, output.Count);
            Assert.AreEqual("ibfoo", output[0]);
        }

        [TestMethod]
        public void ParseFromClause_BackOfficeTables_ContainsTable_ReturnTable()
        {
            var fromClause = "from T1.hibfoo AND T2.hibar ";

            var output = _clauseParser.GetTablesThatExistInClause(fromClause, _backOfficeTables).ToList();

            Assert.AreEqual(1, output.Count);
            Assert.AreEqual("hibfoo", output[0]);
        }

        [TestMethod]
        public void ParseFromClause_BackOfficeTables_DoesNotContainTable_ReturnEmpty()
        {
            var fromClause = "from T1.hibblah AND T2.hibar ";

            var output = _clauseParser.GetTablesThatExistInClause(fromClause, _backOfficeTables).ToList();

            Assert.AreEqual(0, output.Count);
        }

        [TestMethod]
        public void ParseFromClause_BackOfficeTables_ContainsMultipleTables_ReturnMultipleTables()
        {
            var fromClause = "from T1.hibfoo AND T2.hibfoo2 ";

            var output = _clauseParser.GetTablesThatExistInClause(fromClause, _backOfficeTables).ToList();

            Assert.AreEqual(2, output.Count);
            Assert.AreEqual(true, output.Any(x => x.Equals("hibfoo")));
            Assert.AreEqual(true, output.Any(x => x.Equals("hibfoo2")));
        }

        [TestMethod]
        public void ParseFromClause_BackOfficeTables_ContainsTableInDifferentCase_ReturnTable()
        {
            var fromClause = "from T1.HIBFOO AND T2.Hibar ";

            var output = _clauseParser.GetTablesThatExistInClause(fromClause, _backOfficeTables).ToList();

            Assert.AreEqual(1, output.Count);
            Assert.AreEqual("hibfoo", output[0]);
        }

        [TestMethod]
        public void GetExistingReckeyPrefix_ReckeyExists_PrefixExists_ReturnPrefix()
        {
            var clause = "T1.reckey ";

            var output = _clauseParser.GetExistingReckeyPrefix(clause);

            Assert.AreEqual("T1", output);
        }

        [TestMethod]
        public void GetExistingReckeyPrefix_ReckeyExists_FunkyPrefixExists_ReturnPrefix()
        {
            var clause = "T6A.reckey ";

            var output = _clauseParser.GetExistingReckeyPrefix(clause);
            
            Assert.AreEqual("T6A", output);
        }

        //[ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void GetExistingReckeyPrefix_NoReckeyExists_EmptyString()
        {
            var clause = "foo ";

            var output = _clauseParser.GetExistingReckeyPrefix(clause);

            Assert.AreEqual("", output);
        }

        //[ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void GetExistingReckeyPrefix_ReckeyExists_NoPrefixExists_EmptyString()
        {
            var clause = "reckey ";

            var output = _clauseParser.GetExistingReckeyPrefix(clause);

            Assert.AreEqual("", output);
        }

        [TestMethod]
        public void GetExistingReckeyPrefix_MultipleTablesExistsSeparatedBySpaces_ReturnFirstPrefix()
        {
            var clause = "T1.reckey = T6A.reckey";

            var output = _clauseParser.GetExistingReckeyPrefix(clause);

            Assert.AreEqual("T1", output);
        }
    }
}
