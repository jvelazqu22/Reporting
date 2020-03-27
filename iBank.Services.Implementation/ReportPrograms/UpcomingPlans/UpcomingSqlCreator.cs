using Domain.Models;

namespace iBank.Services.Implementation.ReportPrograms.UpcomingPlans
{
    public class UpcomingSqlCreator
    {
        public SqlScript Create(string existingWhereClause, int udid)
        {
            var sql = new SqlScript { WhereClause = existingWhereClause };

            if (udid != 0)
            {
                sql.FromClause = "ibtrips T1, iblegs T2, ibudids T3";
                sql.KeyWhereClause = "T1.reckey = T2.reckey AND T1.reckey = T3.reckey AND airline != 'ZZ' and valcarr NOT IN ('ZZ','$$') AND ";
                sql.WhereClause = sql.KeyWhereClause + existingWhereClause;
            }
            else
            {
                sql.FromClause = "ibtrips T1, iblegs T2";
                sql.KeyWhereClause = "T1.reckey = T2. reckey AND airline != 'ZZ' AND valcarr NOT IN ('ZZ','$$') AND ";
                sql.WhereClause = sql.KeyWhereClause + existingWhereClause;
            }

            sql.OrderBy = "";
            sql.FieldList = @"T1.recloc, T1.reckey, acct, break1, break2, break3, passlast, passfrst, domintl, 'I' as trantype ";

            return sql;
        }
    }
}
