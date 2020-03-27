using Domain.Models;

namespace iBank.Services.Implementation.ReportPrograms.ExecutiveSummaryYearToYear
{
    public class ExecutiveSummaryYearToYearSqlCreator
    {
        private ExecSummaryWhereClauseTransformer _transformer = new ExecSummaryWhereClauseTransformer();
        public SqlScript GetAirSql(string useDate, int udidNumber, string existingWhereClause)
        {
            var sql = new SqlScript();

            sql.FieldList = "T1.reckey,convert(int,plusmin) as plusmin, reascode, airchg, stndchg, mktfare, offrdchg, basefare, svcfee, invdate, depdate, ArrDate, Exchange, savingcode, " + useDate + " as UseDate, valCarMode ";

            sql.FromClause = udidNumber > 0 ? "hibtrips T1, hibudids T3" : "hibtrips T1";

            sql.KeyWhereClause = udidNumber > 0 ? "T1.reckey = T3.reckey and valcarr not in ('ZZ','$$') and " : "valcarr not in ('ZZ','$$') and ";
            sql.WhereClause = sql.KeyWhereClause + existingWhereClause;

            return sql;
        }

        public SqlScript GetLegSql(string useDate, int udidNumber, string existingWhereClause)
        {
            var sql = new SqlScript();

            sql.FieldList = useDate + " as UseDate,T1.reckey, convert(int,plusmin) as plusmin, exchange, convert(int,T2.miles) as miles, Origin, Destinat ";

            sql.FromClause = udidNumber > 0 ? "hibtrips T1, hibudids T3, hiblegs T2 " : "hibtrips T1, hiblegs T2 ";

            sql.KeyWhereClause = udidNumber > 0
                                     ? "T1.reckey = T3.reckey and T1.reckey = T2.reckey and airline not in ('$$','ZZ') and valcarr not in ('$$','ZZ') and "
                                     : "T1.reckey = T2.reckey and airline not in ('$$','ZZ') and valcarr not in ('$$','ZZ') and ";
            sql.WhereClause = sql.KeyWhereClause + existingWhereClause;

            return sql;
        }

        public SqlScript GetCarSql(string useDate, int udidNumber, string existingWhereClause)
        {
            var sql = new SqlScript();

            sql.FieldList = useDate + " as UseDate,T1.reckey, convert(int,cplusmin) as cplusmin, convert(int,days) as days, abookrat, convert(int,Numcars) as Numcars, carType ";

            sql.FromClause = udidNumber > 0 ? "hibtrips T1, hibudids T3, hibcars T4 " : "hibtrips T1, hibcars T4 ";

            sql.KeyWhereClause = udidNumber > 0
                                     ? "T1.reckey = T3.reckey and T1.reckey = T4.reckey and "
                                     : "T1.reckey = T4.reckey and ";
            sql.WhereClause = sql.KeyWhereClause + _transformer.TransformWhereClause(existingWhereClause, ExecSummaryWhereClauseTransformer.SqlType.Car);

            return sql;
        }

        public SqlScript GetHotelSql(string useDate, int udidNumber, string existingWhereClause)
        {
            var sql = new SqlScript();

            sql.FieldList = useDate + " as UseDate,T1.reckey,convert(int,hplusmin) as hplusmin,convert(int,nights) as nights, convert(int,rooms) as rooms, bookrate ";

            sql.FromClause = udidNumber > 0 ? "hibtrips T1, hibudids T3, hibhotel T5 " : "hibtrips T1, hibhotel T5 ";

            sql.KeyWhereClause = udidNumber > 0
                                     ? "T1.reckey = T3.reckey and T1.reckey = T5.reckey and "
                                     : "T1.reckey = T5.reckey and ";
            sql.WhereClause = sql.KeyWhereClause + _transformer.TransformWhereClause(existingWhereClause, ExecSummaryWhereClauseTransformer.SqlType.Hotel);

            return sql;
        }

        public SqlScript GetSvcFeeSql(string useDate, int udidNumber, string existingWhereClause, bool includeOrphanServiceFees)
        {
            var sql = new SqlScript();

            sql.FieldList = "T1.reckey, T1.recloc, T1.invoice, T1.acct, T1.passlast, T1.passfrst, T6A.svcAmt, " + useDate + " as UseDate, valcarMode ";

            sql.FromClause = udidNumber > 0 ? "hibtrips T1, hibServices T6A, hibudids T3 " : "hibtrips T1, hibServices T6A ";

            sql.KeyWhereClause = udidNumber > 0
                                     ? "T1.reckey = T6A.reckey and T1.agency = T6A.agency and T1.reckey = T3.reckey and T1.agency = T3.agency and T6A.svcCode = 'TSF' and "
                                     : "T1.reckey = T6A.reckey and T1.agency = T6A.agency and T6A.svcCode = 'TSF' and ";

            if (!includeOrphanServiceFees)
            {
                sql.KeyWhereClause += "origValCar not in ('SVCFEEONLY','ZZ:S') and ";
            }
            sql.WhereClause = sql.KeyWhereClause + _transformer.TransformWhereClause(existingWhereClause, ExecSummaryWhereClauseTransformer.SqlType.SvcFee);

            return sql;
        }
    }
}