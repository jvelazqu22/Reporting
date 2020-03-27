using Domain.Models;

namespace iBank.Services.Implementation.ReportPrograms.AdvanceBookAir
{
    public class AdvanceBookAirSqlCreator
    {
        public SqlScript Create(string existingWhereClause, bool isReservationReport, bool isRbInAdvanceEqualTwo, int numberOfDays, int udid)
        {
            var sql = new SqlScript();

            if (isRbInAdvanceEqualTwo)
            {
                existingWhereClause += "and (depdate-bookdate) < " + numberOfDays;
            }
            //else
            //{
            //    existingWhereClause += "and (depdate-invdate) < " + numberOfDays;
            //}

            if (udid > 0)
            {
                sql.FromClause = isReservationReport ? "ibtrips T1, iblegs T2, ibudids T3" : "hibtrips T1, hiblegs T2, hibudids T3";
                sql.KeyWhereClause = "T1.reckey = T2.reckey and T1.reckey = T3.reckey and airline != 'ZZ' and valcarr not in ('ZZ','$$') and ";
                sql.WhereClause = sql.KeyWhereClause + existingWhereClause;
                sql.FieldList = "T1.recloc, T1.reckey, bookdate, depdate, invdate, passlast, passfrst, acct, break1, break2, break3, airchg, convert(int,plusmin) as plusmin, convert(int,seqno) as seqno";
            }
            else
            {
                sql.FromClause = isReservationReport ? "ibtrips T1, iblegs T2" : "hibtrips T1, hiblegs T2";
                sql.KeyWhereClause = "T1.reckey = T2.reckey and airline != 'ZZ' and valcarr not in ('ZZ','$$') and ";
                sql.WhereClause = sql.KeyWhereClause + existingWhereClause;
                sql.FieldList = "T1.recloc, T1.reckey, bookdate, depdate, invdate, passlast, passfrst, acct, break1, break2, break3, airchg, convert(int,1) as plusmin, convert(int,seqno) as seqno";
            }

            sql.OrderBy = "";

            return sql;
        }
    }
}
