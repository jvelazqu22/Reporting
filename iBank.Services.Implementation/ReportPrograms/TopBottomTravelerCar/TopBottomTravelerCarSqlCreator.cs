using Domain.Models;
using iBank.Services.Implementation.Utilities;

namespace iBank.Services.Implementation.ReportPrograms.TopBottomTravelerCar
{
    public class TopBottomTravelerCarSqlCreator
    {
        public SqlScript CreateScript(string existingWhereClause, int udid, bool isReservationReport)
        {
            var sql = new SqlScript { WhereClause = existingWhereClause };
            var transformer = new SqlTransformer();

            if (udid != 0)
            {
                sql.FromClause = isReservationReport ? "ibtrips T1, ibcar T4, ibudids T3 " : "hibtrips T1, hibcars T4, hibudids T3";
                sql.KeyWhereClause = "T1.reckey = T3.reckey and T1.reckey = T4.reckey and passlast is not null and  ";
                sql.WhereClause = sql.KeyWhereClause + sql.WhereClause;
            }
            else
            {
                sql.FromClause = isReservationReport ? "ibtrips T1, ibcar T4 " : "hibtrips T1, hibcars T4 ";
                sql.KeyWhereClause = "T1.reckey = T4.reckey and passlast is not null and  ";
                sql.WhereClause = sql.KeyWhereClause + sql.WhereClause;
            }

            sql.FieldList = (isReservationReport) ? "passlast, passfrst, aBookRat, Days" : "passlast, passfrst, aBookRat, CPlusMin, Days";

            sql.OrderBy = string.Empty;

            return sql;
        }

    }
}
