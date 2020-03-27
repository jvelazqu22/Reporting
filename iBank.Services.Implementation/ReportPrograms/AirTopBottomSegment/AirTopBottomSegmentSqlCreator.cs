using Domain.Models;

namespace iBank.Services.Implementation.ReportPrograms.AirTopBottomSegment
{
    public class AirTopBottomSegmentSqlCreator
    {
        public SqlScript CreateScript(string existingWhereClause, int udid, bool isReservationReport, string udidWhere)
        {
            var sql = new SqlScript { WhereClause = existingWhereClause };

            if (udid != 0)
            {
                sql.FromClause = isReservationReport ? "ibtrips T1, iblegs T2, ibudids T3 " : "hibtrips T1, hiblegs T2, hibudids T3";
                sql.KeyWhereClause = "T1.reckey = T2.reckey and T1.reckey = T3.reckey and airline not in ('$$','ZZ') and airline is not null and ";
                sql.WhereClause = sql.KeyWhereClause + sql.WhereClause;
            }
            else
            {
                sql.FromClause = isReservationReport ? "ibtrips T1, iblegs T2 " : "hibtrips T1, hiblegs T2 ";
                sql.KeyWhereClause = "T1.reckey = T2.reckey and airline not in ('$$','ZZ') and airline is not null and ";
                sql.WhereClause = sql.KeyWhereClause + sql.WhereClause;
            }

            sql.FieldList = (isReservationReport) ? "T1.reckey, convert(smallint, 1) as plusmin " : " T1.reckey, plusmin";


            sql.OrderBy = "";

            return sql;
        }

    }
}
