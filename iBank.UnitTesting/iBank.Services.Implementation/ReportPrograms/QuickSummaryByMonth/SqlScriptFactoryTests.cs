using iBank.Services.Implementation.ReportPrograms.QuickSummaryByMonth;
using iBank.Services.Implementation.ReportPrograms.QuickSummaryByMonth.Factories;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.QuickSummaryByMonth
{
    [TestClass]
    public class SqlScriptFactoryTests
    {
        [TestMethod]
        public void BuildAirRawDataScript_ReservationReportValidUdid()
        {
            var dateRangeType = "fooType";
            var whereClause = "foo clause";

            var expectedFieldList = "fooType as datecomp, invdate, depdate, BookDate, convert(int,1) as plusmin, reascode, savingcode, airchg, stndchg, offrdchg ";
            var expectedFromClause = "ibtrips T1, ibudids T3";
            var expectedWhereClause = "T1.reckey = T3.reckey and valcarr not in ('ZZ','$$') and foo clause";

            var factory = new SqlScriptFactory(DataTypes.DataType.Air, true, whereClause, dateRangeType, true);
            var sqlScript = factory.Build();

            Assert.AreEqual(expectedFieldList, sqlScript.FieldList);
            Assert.AreEqual(expectedFromClause, sqlScript.FromClause);
            Assert.AreEqual(expectedWhereClause, sqlScript.WhereClause);
        }

        [TestMethod]
        public void BuildAirRawDataScript_ReservationReportInValidUdid()
        {
            var dateRangeType = "fooType";
            var whereClause = "foo clause";

            var factory = new SqlScriptFactory(DataTypes.DataType.Air, true, whereClause, dateRangeType, false);

            var expectedFieldList = "fooType as datecomp, invdate, depdate, BookDate, convert(int,1) as plusmin, reascode, savingcode, airchg, stndchg, offrdchg ";
            var expectedFromClause = "ibtrips T1";
            var expectedWhereClause = "valcarr not in ('ZZ','$$') and foo clause";

            var sqlScript = factory.Build();

            Assert.AreEqual(expectedFieldList, sqlScript.FieldList);
            Assert.AreEqual(expectedFromClause, sqlScript.FromClause);
            Assert.AreEqual(expectedWhereClause, sqlScript.WhereClause);
        }

        [TestMethod]
        public void BuildAirRawDataScript_HistoryReportValidUdid()
        {
            var dateRangeType = "fooType";
            var whereClause = "foo clause";
            var udid = "1";

            var expectedFieldList = "fooType as datecomp, invdate, depdate, BookDate, convert(int,plusmin) as plusmin, reascode, savingcode, airchg, stndchg, offrdchg ";
            var expectedFromClause = "hibtrips T1, hibudids T3";
            var expectedWhereClause = "T1.reckey = T3.reckey and valcarr not in ('ZZ','$$') and foo clause";

            var factory = new SqlScriptFactory(DataTypes.DataType.Air, false, whereClause, dateRangeType, true);
            var sqlScript = factory.Build();

            Assert.AreEqual(expectedFieldList, sqlScript.FieldList);
            Assert.AreEqual(expectedFromClause, sqlScript.FromClause);
            Assert.AreEqual(expectedWhereClause, sqlScript.WhereClause);
        }

        [TestMethod]
        public void BuildAirRawDataScript_HistoryReportInValidUdid()
        {
            var dateRangeType = "fooType";
            var whereClause = "foo clause";

            var expectedFieldList = "fooType as datecomp, invdate, depdate, BookDate, convert(int,plusmin) as plusmin, reascode, savingcode, airchg, stndchg, offrdchg ";
            var expectedFromClause = "hibtrips T1";
            var expectedWhereClause = "valcarr not in ('ZZ','$$') and foo clause";

            var factory = new SqlScriptFactory(DataTypes.DataType.Air, false, whereClause, dateRangeType, false);
            var sqlScript = factory.Build();

            Assert.AreEqual(expectedFieldList, sqlScript.FieldList);
            Assert.AreEqual(expectedFromClause, sqlScript.FromClause);
            Assert.AreEqual(expectedWhereClause, sqlScript.WhereClause);
        }

        [TestMethod]
        public void BuildHotelRawDataScript_ReservationReportValidUdid()
        {
            var dateRangeType = "fooType";
            var whereClause = "foo clause";

            var expectedFieldList = "fooType as datecomp, invdate, depdate, BookDate, convert(int,1) as hplusmin, nights, rooms, bookrate, reascodh, hexcprat";
            var expectedFromClause = "ibtrips T1, ibudids T3, ibhotel T5";
            var expectedWhereClause = "T1.reckey = T3.reckey and T1.reckey = T5.reckey and foo clause";

            var factory = new SqlScriptFactory(DataTypes.DataType.Hotel, true, whereClause, dateRangeType, true);
            var sqlScript = factory.Build();

            Assert.AreEqual(expectedFieldList, sqlScript.FieldList);
            Assert.AreEqual(expectedFromClause, sqlScript.FromClause);
            Assert.AreEqual(expectedWhereClause, sqlScript.WhereClause);
        }

        [TestMethod]
        public void BuildHotelRawDataScript_ReservationReportInValidUdid()
        {
            var dateRangeType = "fooType";
            var whereClause = "foo clause";

            var expectedFieldList = "fooType as datecomp, invdate, depdate, BookDate, convert(int,1) as hplusmin, nights, rooms, bookrate, reascodh, hexcprat";
            var expectedFromClause = "ibtrips T1, ibhotel T5";
            var expectedWhereClause = "T1.reckey = T5.reckey and foo clause";

            var factory = new SqlScriptFactory(DataTypes.DataType.Hotel, true, whereClause, dateRangeType, false);
            var sqlScript = factory.Build();

            Assert.AreEqual(expectedFieldList, sqlScript.FieldList);
            Assert.AreEqual(expectedFromClause, sqlScript.FromClause);
            Assert.AreEqual(expectedWhereClause, sqlScript.WhereClause);
        }

        [TestMethod]
        public void BuildHotelRawDataScript_HistoryReportValidUdid()
        {
            var dateRangeType = "fooType";
            var whereClause = "T1.trantype";

            var expectedFieldList = "fooType as datecomp, invdate, depdate, BookDate, convert(int,hplusmin) as hplusmin, convert(int,nights) as nights, convert(int,rooms) as rooms, bookrate, reascodh, hexcprat";
            var expectedFromClause = "hibtrips T1, hibudids T3, hibhotel T5";
            var expectedWhereClause = "T1.reckey = T3.reckey and T1.reckey = T5.reckey and T5.HotTranTyp";

            var factory = new SqlScriptFactory(DataTypes.DataType.Hotel, false, whereClause, dateRangeType, true);
            var sqlScript = factory.Build();

            Assert.AreEqual(expectedFieldList, sqlScript.FieldList);
            Assert.AreEqual(expectedFromClause, sqlScript.FromClause);
            Assert.AreEqual(expectedWhereClause, sqlScript.WhereClause);
        }

        [TestMethod]
        public void BuildHotelRawDataScript_HistoryReportInValidUdid()
        {
            var dateRangeType = "fooType";
            var whereClause = "T1.trantype";

            var expectedFieldList = "fooType as datecomp, invdate, depdate, BookDate, convert(int,hplusmin) as hplusmin, convert(int,nights) as nights, convert(int,rooms) as rooms, bookrate, reascodh, hexcprat";
            var expectedFromClause = "hibtrips T1, hibhotel T5";
            var expectedWhereClause = "T1.reckey = T5.reckey and T5.HotTranTyp";

            var factory = new SqlScriptFactory(DataTypes.DataType.Hotel, false, whereClause, dateRangeType, false);
            var sqlScript = factory.Build();

            Assert.AreEqual(expectedFieldList, sqlScript.FieldList);
            Assert.AreEqual(expectedFromClause, sqlScript.FromClause);
            Assert.AreEqual(expectedWhereClause, sqlScript.WhereClause);
        }

        [TestMethod]
        public void BuildCarRawDataScript_ReservationReportValidUdid()
        {
            var dateRangeType = "fooType";
            var whereClause = "foo clause";

            var expectedFieldList = "fooType as datecomp, invdate, depdate, BookDate, convert(int,1) as cplusmin, days, abookrat, reascoda, aexcprat";
            var expectedFromClause = "ibtrips T1, ibudids T3, ibcar T4";
            var expectedWhereClause = "T1.reckey = T3.reckey and T1.reckey = T4.reckey and foo clause";

            var factory = new SqlScriptFactory(DataTypes.DataType.Car, true, whereClause, dateRangeType, true);
            var sqlScript = factory.Build();

            Assert.AreEqual(expectedFieldList, sqlScript.FieldList);
            Assert.AreEqual(expectedFromClause, sqlScript.FromClause);
            Assert.AreEqual(expectedWhereClause, sqlScript.WhereClause);
        }

        [TestMethod]
        public void BuildCarRawDataScript_ReservationReportInValidUdid()
        {
            var dateRangeType = "fooType";
            var whereClause = "foo clause";

            var expectedFieldList = "fooType as datecomp, invdate, depdate, BookDate, convert(int,1) as cplusmin, days, abookrat, reascoda, aexcprat";
            var expectedFromClause = "ibtrips T1, ibcar T4";
            var expectedWhereClause = "T1.reckey = T4.reckey and foo clause";

            var factory = new SqlScriptFactory(DataTypes.DataType.Car, true, whereClause, dateRangeType, false);
            var sqlScript = factory.Build();

            Assert.AreEqual(expectedFieldList, sqlScript.FieldList);
            Assert.AreEqual(expectedFromClause, sqlScript.FromClause);
            Assert.AreEqual(expectedWhereClause, sqlScript.WhereClause);
        }

        [TestMethod]
        public void BuildCarRawDataScript_HistoryReportValidUdid()
        {
            var dateRangeType = "fooType";
            var whereClause = "T1.trantype";

            var expectedFieldList = "fooType as datecomp, invdate, depdate, BookDate, convert(int,cplusmin) as cplusmin, days, abookrat, reascoda, aexcprat";
            var expectedFromClause = "hibtrips T1, hibudids T3, hibcars T4";
            var expectedWhereClause = "T1.reckey = T3.reckey and T1.reckey = T4.reckey and T4.CarTranTyp";

            var factory = new SqlScriptFactory(DataTypes.DataType.Car, false, whereClause, dateRangeType, true);
            var sqlScript = factory.Build();

            Assert.AreEqual(expectedFieldList, sqlScript.FieldList);
            Assert.AreEqual(expectedFromClause, sqlScript.FromClause);
            Assert.AreEqual(expectedWhereClause, sqlScript.WhereClause);
        }

        [TestMethod]
        public void BuildCarRawDataScript_HistoryReportInValidUdid()
        {
            var dateRangeType = "fooType";
            var whereClause = "T1.trantype";

            var expectedFieldList = "fooType as datecomp, invdate, depdate, BookDate, convert(int,cplusmin) as cplusmin, days, abookrat, reascoda, aexcprat";
            var expectedFromClause = "hibtrips T1, hibcars T4";
            var expectedWhereClause = "T1.reckey = T4.reckey and T4.CarTranTyp";

            var factory = new SqlScriptFactory(DataTypes.DataType.Car, false, whereClause, dateRangeType, false);
            var sqlScript = factory.Build();

            Assert.AreEqual(expectedFieldList, sqlScript.FieldList);
            Assert.AreEqual(expectedFromClause, sqlScript.FromClause);
            Assert.AreEqual(expectedWhereClause, sqlScript.WhereClause);
        }
    }
}
