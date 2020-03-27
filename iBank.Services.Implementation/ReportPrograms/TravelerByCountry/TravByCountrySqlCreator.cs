using Domain.Models;

namespace iBank.Services.Implementation.ReportPrograms.TravelerByCountry
{
    public class TravByCountrySqlCreator
    {
        public SqlScript Create(string existingWhereClause, int udidNumber, bool isReservationReport)
        {
            var sql = new SqlScript();

            if (udidNumber > 0)
            {
                sql.FromClause = isReservationReport ? "ibtrips T1, iblegs T2, ibudids T3" : "hibtrips T1, hiblegs T2, hibudids T3";

                sql.KeyWhereClause = "T1.reckey = T2.reckey and T1.reckey = T3.reckey and valcarr not in ('ZZ','$$') and airline != 'ZZ' and ";

                sql.WhereClause = sql.KeyWhereClause + existingWhereClause;
            }
            else
            {
                sql.FromClause = isReservationReport ? "ibtrips T1, iblegs T2" : "hibtrips T1, hiblegs T2";

                sql.KeyWhereClause = "T1.reckey = T2.reckey and valcarr not in ('ZZ','$$') and airline != 'ZZ' and ";

                sql.WhereClause = sql.KeyWhereClause + existingWhereClause;
            }

            sql.FieldList = isReservationReport
                ? "T1.reckey, passlast, passfrst, convert(int, 1) as plusmin"
                : "T1.reckey, passlast, passfrst, convert(int, plusmin) as plusmin";

            sql.OrderBy = "";

            return sql;
        }
    }
}