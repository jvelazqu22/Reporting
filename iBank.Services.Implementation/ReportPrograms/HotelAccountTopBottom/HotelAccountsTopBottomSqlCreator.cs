using Domain.Models;
using iBank.Services.Implementation.Utilities;

namespace iBank.Services.Implementation.ReportPrograms.HotelAccountTopBottom
{
    public class HotelAccountsTopBottomSqlCreator
    {
        public SqlScript CreateScript(string existingWhereClause, int udid, bool isReservationReport)
        {
            var sql = new SqlScript { WhereClause = existingWhereClause };
            var transformer = new SqlTransformer();

            if (udid != 0)
            {
                sql.FromClause = isReservationReport ? "ibtrips T1, ibHotel T5, ibudids T3 " : "hibtrips T1, hibHotel T5, hibudids T3";
                sql.KeyWhereClause = "T1.reckey = T5.reckey and T1.reckey = T3.reckey and acct is not null and ";
                sql.WhereClause = sql.KeyWhereClause + sql.WhereClause;
            }
            else
            {
                sql.FromClause = isReservationReport ? "ibtrips T1, ibHotel T5 " : "hibtrips T1, hibHotel T5 ";
                sql.KeyWhereClause = "T1.reckey = T5.reckey and acct is not null and ";
                sql.WhereClause = sql.KeyWhereClause + sql.WhereClause;
            }

            sql.FieldList = (isReservationReport) 
                ? "T1.SourceAbbr, Acct, BookRate, nights, rooms" 
                : "T1.SourceAbbr, Acct, BookRate, convert(int,nights) nights, convert(int,rooms) rooms, Hplusmin";

            sql.OrderBy = string.Empty;

            return sql;
        }
    }
}
