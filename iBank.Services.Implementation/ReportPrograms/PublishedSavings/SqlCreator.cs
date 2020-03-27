using Domain.Models;

namespace iBank.Services.Implementation.ReportPrograms.PublishedSavings
{
    public class SqlCreator
    {
        public SqlScript CreateScript(string existingWhereClause, int udid, bool isReservationReport)
        {
            var sql = new SqlScript { WhereClause = existingWhereClause };

            if (udid != 0)
            {
                sql.FromClause = isReservationReport ? "ibtrips T1, iblegs T2, ibudids T3" : "hibtrips T1, hiblegs T2, hibudids T3";
                sql.KeyWhereClause = "T1.reckey = T2.reckey and T1.reckey = T3.reckey and airline != 'ZZ' and valcarr not in ('ZZ','$$') and ";
                sql.WhereClause = sql.KeyWhereClause + sql.WhereClause;
            }
            else
            {
                sql.FromClause = isReservationReport ? "ibtrips T1, iblegs T2" : "hibtrips T1, hiblegs T2";
                sql.KeyWhereClause = "T1.reckey = T2.reckey and airline != 'ZZ' and valcarr not in ('ZZ','$$') and ";
                sql.WhereClause = sql.KeyWhereClause + sql.WhereClause;
            }

            sql.FieldList = "T1.recloc, T1.reckey, invoice, ticket, acct, break1, break2, break3, passlast, passfrst, reascode, savingcode, stndchg, airchg";

            sql.OrderBy = string.Empty;

            return sql;
        }

    }
}
