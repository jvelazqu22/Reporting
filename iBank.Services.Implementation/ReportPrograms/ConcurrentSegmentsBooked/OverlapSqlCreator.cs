using Domain.Models;

namespace iBank.Services.Implementation.ReportPrograms.ConcurrentSegmentsBooked
{
    public class OverlapSqlCreator
    {
        public SqlScript Create(string existingWhereClause, int udidNumber)
        {
            var sql = new SqlScript();

            if (udidNumber > 0)
            {
                sql.FromClause = "ibtrips T1, iblegs T2, ibudids T3";
                sql.KeyWhereClause = "T1.reckey = T2.reckey and T1.reckey = T3.reckey and airline != 'ZZ' and valcarr not in ('ZZ','$$') and mode = 'A' and ";
            }
            else
            {
                sql.FromClause = "ibtrips T1, iblegs T2";
                sql.KeyWhereClause = "T1.reckey = T2.reckey and airline != 'ZZ' and valcarr not in ('ZZ','$$') and mode = 'A' and ";
            }

            sql.WhereClause = sql.KeyWhereClause + existingWhereClause;
            sql.FieldList = "T1.recloc, T1.reckey, ticket, acct, passlast, passfrst, bktool, gds, airchg, depdate, bookdate, airline, origin, destinat, T2.class as classcode, T2.classcat, convert(int, T2.SeqNo) SeqNo, T2.rarrdate, T2.rdepdate";
            sql.OrderBy = "";
            
            return sql;
        }
    }
}
