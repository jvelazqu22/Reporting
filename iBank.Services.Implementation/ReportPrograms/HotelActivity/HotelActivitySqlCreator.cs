using Domain.Models;
using iBank.Services.Implementation.Shared.AdvancedClause;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using iBank.Services.Implementation.Utilities;

namespace iBank.Services.Implementation.ReportPrograms.HotelActivity
{
    public class HotelActivitySqlCreator
    {
        private readonly WhereClauseWithAdvanceParamsHandler _whereClauseWithAdvanceParamsHandler = new WhereClauseWithAdvanceParamsHandler();

        public SqlScript CreateScript(BuildWhere buildWhere, int udid, bool isReservationReport)
        {
            var sql = new SqlScript { WhereClause = buildWhere.WhereClauseFull };
            var transformer = new SqlTransformer();

            if (udid != 0)
            {
                sql.FromClause = isReservationReport ? "ibtrips T1, ibhotel T5, ibudids T3" : "hibtrips T1, hibhotel T5, hibudids T3";
                sql.KeyWhereClause = "T1.reckey = T5.reckey and T1.reckey = T3.reckey and  ";
                sql.WhereClause = sql.KeyWhereClause + sql.WhereClause;
            }
            else
            {
                sql.FromClause = isReservationReport ? "ibtrips T1, ibhotel T5" : "hibtrips T1, hibhotel T5";
                sql.KeyWhereClause = "T1.reckey = T5.reckey and ";
                sql.WhereClause = sql.KeyWhereClause + sql.WhereClause;
            }

            sql.FieldList = @"T1.reckey, acct, break1, break2, break3, T1.recloc, passlast, " +
                               "passfrst, hotcity, hotstate, RTRIM(hotcity) + ', ' + hotstate as hotcityst, datein, " +
                               "hotelnam, chaincod, roomtype, convert(int,nights) as nights, convert(int, rooms) rooms, bookrate,";
            sql.FieldList += (isReservationReport) ? "convert(int, 1) as hplusmin " : " convert(int,hplusmin) hplusmin";

            sql.OrderBy = string.Empty;

            sql.WhereClause = _whereClauseWithAdvanceParamsHandler.GetWhereClauseWithAdvancedParametersAndUpdateSqlScriptIfNeeded(sql, buildWhere, false);
            return sql;
        }
    }
}
