using iBank.Services.Implementation.ReportPrograms.ExecutiveSummaryYearToYear;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.ExecutiveSummaryYearToYear
{
    [TestClass]
    public class ExecutiveSummaryYearToYearSqlCreatorTests
    {
        private readonly ExecutiveSummaryYearToYearSqlCreator _creator = new ExecutiveSummaryYearToYearSqlCreator();
        private string _existingWhereClause = "foo";

        private string useDate = "invdate";

        [TestMethod]
        public void GetAirSql_NoUdid()
        {
            var expectedFieldList = "T1.reckey,convert(int,plusmin) as plusmin, reascode, airchg, stndchg, mktfare, offrdchg, basefare, svcfee, invdate, depdate, ArrDate, Exchange, savingcode, " + useDate + " as UseDate, valCarMode ";
            var expectedFromClause = "hibtrips T1";
            var expectedKeyWhere = "valcarr not in ('ZZ','$$') and ";
            var expectedWhere = expectedKeyWhere + _existingWhereClause;

            var output = _creator.GetAirSql(useDate, 0, _existingWhereClause);

            Assert.AreEqual(expectedFieldList, output.FieldList);
            Assert.AreEqual(expectedFromClause, output.FromClause);
            Assert.AreEqual(expectedKeyWhere, output.KeyWhereClause);
            Assert.AreEqual(expectedWhere, output.WhereClause);
        }

        [TestMethod]
        public void GetAirSql_UdidExists()
        {
            var expectedFieldList = "T1.reckey,convert(int,plusmin) as plusmin, reascode, airchg, stndchg, mktfare, offrdchg, basefare, svcfee, invdate, depdate, ArrDate, Exchange, savingcode, " + useDate + " as UseDate, valCarMode ";
            var expectedFromClause = "hibtrips T1, hibudids T3";
            var expectedKeyWhere = "T1.reckey = T3.reckey and valcarr not in ('ZZ','$$') and ";
            var expectedWhere = expectedKeyWhere + _existingWhereClause;

            var output = _creator.GetAirSql(useDate, 1, _existingWhereClause);

            Assert.AreEqual(expectedFieldList, output.FieldList);
            Assert.AreEqual(expectedFromClause, output.FromClause);
            Assert.AreEqual(expectedKeyWhere, output.KeyWhereClause);
            Assert.AreEqual(expectedWhere, output.WhereClause);
        }

        [TestMethod]
        public void GetCarSql_NoUdid()
        {
            var expectedFieldList = useDate + " as UseDate,T1.reckey, convert(int,cplusmin) as cplusmin, convert(int,days) as days, abookrat, convert(int,Numcars) as Numcars, carType ";
            var expectedFromClause = "hibtrips T1, hibcars T4 ";
            var expectedKeyWhere = "T1.reckey = T4.reckey and ";
            var expectedWhere = expectedKeyWhere + _existingWhereClause;

            var output = _creator.GetCarSql(useDate, 0, _existingWhereClause);

            Assert.AreEqual(expectedFieldList, output.FieldList);
            Assert.AreEqual(expectedFromClause, output.FromClause);
            Assert.AreEqual(expectedKeyWhere, output.KeyWhereClause);
            Assert.AreEqual(expectedWhere, output.WhereClause);
        }

        [TestMethod]
        public void GetCarSql_UdidExists()
        {
            var expectedFieldList = useDate + " as UseDate,T1.reckey, convert(int,cplusmin) as cplusmin, convert(int,days) as days, abookrat, convert(int,Numcars) as Numcars, carType ";
            var expectedFromClause = "hibtrips T1, hibudids T3, hibcars T4 ";
            var expectedKeyWhere = "T1.reckey = T3.reckey and T1.reckey = T4.reckey and ";
            var expectedWhere = expectedKeyWhere + _existingWhereClause;

            var output = _creator.GetCarSql(useDate, 1, _existingWhereClause);

            Assert.AreEqual(expectedFieldList, output.FieldList);
            Assert.AreEqual(expectedFromClause, output.FromClause);
            Assert.AreEqual(expectedKeyWhere, output.KeyWhereClause);
            Assert.AreEqual(expectedWhere, output.WhereClause);
        }

        [TestMethod]
        public void GetHotelSql_UdidExists()
        {
            var expectedFieldList = useDate + " as UseDate,T1.reckey,convert(int,hplusmin) as hplusmin,convert(int,nights) as nights, convert(int,rooms) as rooms, bookrate ";
            var expectedFromClause = "hibtrips T1, hibudids T3, hibhotel T5 ";
            var expectedKeyWhere = "T1.reckey = T3.reckey and T1.reckey = T5.reckey and ";
            var expectedWhere = expectedKeyWhere + _existingWhereClause;

            var output = _creator.GetHotelSql(useDate, 1, _existingWhereClause);

            Assert.AreEqual(expectedFieldList, output.FieldList);
            Assert.AreEqual(expectedFromClause, output.FromClause);
            Assert.AreEqual(expectedKeyWhere, output.KeyWhereClause);
            Assert.AreEqual(expectedWhere, output.WhereClause);
        }

        [TestMethod]
        public void GetHotelSql_NoUdid()
        {
            var expectedFieldList = useDate + " as UseDate,T1.reckey,convert(int,hplusmin) as hplusmin,convert(int,nights) as nights, convert(int,rooms) as rooms, bookrate ";
            var expectedFromClause = "hibtrips T1, hibhotel T5 ";
            var expectedKeyWhere = "T1.reckey = T5.reckey and ";
            var expectedWhere = expectedKeyWhere + _existingWhereClause;

            var output = _creator.GetHotelSql(useDate, 0, _existingWhereClause);

            Assert.AreEqual(expectedFieldList, output.FieldList);
            Assert.AreEqual(expectedFromClause, output.FromClause);
            Assert.AreEqual(expectedKeyWhere, output.KeyWhereClause);
            Assert.AreEqual(expectedWhere, output.WhereClause);
        }

        [TestMethod]
        public void GetSvcFeeSql_NoUdidIncludeOrphanFees()
        {
            var expectedFieldList = "T1.reckey, T1.recloc, T1.invoice, T1.acct, T1.passlast, T1.passfrst, T6A.svcAmt, " + useDate + " as UseDate, valcarMode ";
            var expectedFromClause = "hibtrips T1, hibServices T6A ";
            var expectedKeyWhere = "T1.reckey = T6A.reckey and T1.agency = T6A.agency and T6A.svcCode = 'TSF' and ";
            var expectedWhere = expectedKeyWhere + _existingWhereClause;

            var output = _creator.GetSvcFeeSql(useDate, 0, _existingWhereClause, true);

            Assert.AreEqual(expectedFieldList, output.FieldList);
            Assert.AreEqual(expectedFromClause, output.FromClause);
            Assert.AreEqual(expectedKeyWhere, output.KeyWhereClause);
            Assert.AreEqual(expectedWhere, output.WhereClause);
        }

        [TestMethod]
        public void GetSvcFeeSql_NoUdidDontIncludeOrphanFees()
        {
            var expectedFieldList = "T1.reckey, T1.recloc, T1.invoice, T1.acct, T1.passlast, T1.passfrst, T6A.svcAmt, " + useDate + " as UseDate, valcarMode ";
            var expectedFromClause = "hibtrips T1, hibServices T6A ";
            var expectedKeyWhere = "T1.reckey = T6A.reckey and T1.agency = T6A.agency and T6A.svcCode = 'TSF' and origValCar not in ('SVCFEEONLY','ZZ:S') and ";
            var expectedWhere = expectedKeyWhere + _existingWhereClause;

            var output = _creator.GetSvcFeeSql(useDate, 0, _existingWhereClause, false);

            Assert.AreEqual(expectedFieldList, output.FieldList);
            Assert.AreEqual(expectedFromClause, output.FromClause);
            Assert.AreEqual(expectedKeyWhere, output.KeyWhereClause);
            Assert.AreEqual(expectedWhere, output.WhereClause);
        }

        [TestMethod]
        public void GetSvcFeeSql_UdidExistsIncludeOrphanFees()
        {
            var expectedFieldList = "T1.reckey, T1.recloc, T1.invoice, T1.acct, T1.passlast, T1.passfrst, T6A.svcAmt, " + useDate + " as UseDate, valcarMode ";
            var expectedFromClause = "hibtrips T1, hibServices T6A, hibudids T3 ";
            var expectedKeyWhere = "T1.reckey = T6A.reckey and T1.agency = T6A.agency and T1.reckey = T3.reckey and T1.agency = T3.agency and T6A.svcCode = 'TSF' and ";
            var expectedWhere = expectedKeyWhere + _existingWhereClause;

            var output = _creator.GetSvcFeeSql(useDate, 1, _existingWhereClause, true);

            Assert.AreEqual(expectedFieldList, output.FieldList);
            Assert.AreEqual(expectedFromClause, output.FromClause);
            Assert.AreEqual(expectedKeyWhere, output.KeyWhereClause);
            Assert.AreEqual(expectedWhere, output.WhereClause);
        }

        [TestMethod]
        public void GetSvcFeeSql_UdidExistsDontIncludeOrphanFees()
        {
            var expectedFieldList = "T1.reckey, T1.recloc, T1.invoice, T1.acct, T1.passlast, T1.passfrst, T6A.svcAmt, " + useDate + " as UseDate, valcarMode ";
            var expectedFromClause = "hibtrips T1, hibServices T6A, hibudids T3 ";
            var expectedKeyWhere = "T1.reckey = T6A.reckey and T1.agency = T6A.agency and T1.reckey = T3.reckey and T1.agency = T3.agency and T6A.svcCode = 'TSF' and origValCar not in ('SVCFEEONLY','ZZ:S') and ";
            var expectedWhere = expectedKeyWhere + _existingWhereClause;

            var output = _creator.GetSvcFeeSql(useDate, 1, _existingWhereClause, false);

            Assert.AreEqual(expectedFieldList, output.FieldList);
            Assert.AreEqual(expectedFromClause, output.FromClause);
            Assert.AreEqual(expectedKeyWhere, output.KeyWhereClause);
            Assert.AreEqual(expectedWhere, output.WhereClause);
        }
    }
}
