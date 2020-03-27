using System;

using iBank.Services.Implementation.Shared.BuildWhereHelpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.Shared.BuildWhereHelpers
{
    [TestClass]
    public class BuildWhereTests
    {
        [TestMethod]
        public void Create_the_full_build_where_clause()
        {
            var where = new BuildWhere(null, null, null, null);
            where.WhereClauseDate = "a = b";
            where.WhereClauseTrip = "c = d";
            where.WhereClauseCar = "e = f";
            where.WhereClauseHotel = "g = h";
            where.WhereClauseUdid = "i = j";

            Assert.AreEqual("a = b AND c = d AND e = f AND g = h AND i = j", where.WhereClauseFull);
        }
    }
}
