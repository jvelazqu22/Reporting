using Domain.Models;

namespace iBank.Services.Implementation.ReportPrograms.TripDuration
{
    public class TripDurationSqlCreator
    {
        public SqlScript Create(string whereClause, int udidNumber, bool isReservation)
        {
            var sql = new SqlScript();

            if (udidNumber != 0)
            {
                sql.FromClause = isReservation
                    ? "ibtrips T1, iblegs T2, ibudids T3"
                    : "hibtrips T1, hiblegs T2, hibudids T3";
                sql.KeyWhereClause = "T1.reckey = T2.reckey and T1.reckey = T3.reckey and valcarr not in ('ZZ','$$') and airline != 'ZZ' and ";
                sql.WhereClause = sql.KeyWhereClause + whereClause;
            }
            else
            {
                sql.FromClause = isReservation
                    ? "ibtrips T1, iblegs T2"
                    : "hibtrips T1, hiblegs T2";

                sql.KeyWhereClause = "T1.reckey = T2.reckey and valcarr not in ('ZZ','$$') and airline != 'ZZ' and ";
                sql.WhereClause = sql.KeyWhereClause + whereClause;
            }

            sql.FieldList = "T1.recloc, T1.reckey, invoice, ticket, acct, break1, " +
                                     "break2, break3, passlast, passfrst, airchg, depdate, arrdate, " +
                                     "invdate, bookdate";

            sql.OrderBy = "";

            return sql;
        }
    }
}