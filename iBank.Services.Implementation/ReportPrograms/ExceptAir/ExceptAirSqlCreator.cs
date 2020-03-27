using Domain.Models;

namespace iBank.Services.Implementation.ReportPrograms.ExceptAir
{
    public class ExceptAirSqlCreator
    {
        public SqlScript Create(string existingWhereClause, int udidNumber, string reasonExclude, bool isReservationReport)
        {
            var sql = new SqlScript();

            if (udidNumber > 0)
            {
                sql.FromClause = isReservationReport ? "ibtrips T1, iblegs T2, ibudids T3"
                                            : "hibtrips T1, hiblegs T2, hibudids T3";
                sql.KeyWhereClause = string.Format(@"T1.reckey = T2.reckey AND T1.reckey = T3.reckey AND " +
                                              "airline != 'ZZ' AND valcarr NOT IN ('ZZ', '$$') AND " +
                                              "reascode IS NOT NULL AND reascode != ' ' AND " +
                                              "LTRIM(RTRIM(reascode)) NOT IN ({0}) AND ",
                                              reasonExclude);

                sql.WhereClause = sql.KeyWhereClause + existingWhereClause;
            }
            else
            {
                sql.FromClause = isReservationReport ? "ibtrips T1, iblegs T2" : "hibtrips T1, hiblegs T2";

                sql.KeyWhereClause = string.Format(@"T1.reckey = T2.reckey AND airline != 'ZZ'  " +
                                              "AND valcarr NOT IN ('ZZ','$$') AND " +
                                              "reascode IS NOT NULL AND reascode != ' ' AND " +
                                              "LTRIM(RTRIM(reascode)) NOT IN ({0}) AND ",
                                              reasonExclude);

                sql.WhereClause = sql.KeyWhereClause + existingWhereClause;
            }

            sql.FieldList = @"T1.recloc, T1.reckey, invoice, ticket, acct, break1, break2, break3, 
                              passlast, passfrst, reascode, airchg, baseFare, offrdchg, depdate, 
                              origin, destinat, mode, connect, rdepdate, airline, fltno, class, convert(int,seqno) as seqno";

            sql.OrderBy = "";

            return sql;
        }
    }
}
