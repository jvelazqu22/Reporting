using Domain.Models;

namespace iBank.Services.Implementation.ReportPrograms.Market
{
    public class MarketSqlCreator
    {
        public SqlScript Create(string existingWhereClause, int udid, bool isReservationReport)
        {
            var sql = new SqlScript();

            if (udid != 0)
            {
                sql.FromClause = isReservationReport
                                     ? "ibtrips T1, iblegs T2, ibudids T3"
                                     : "hibtrips T1, hiblegs T2, hibudids T3";
                sql.KeyWhereClause = "T1.reckey = T2.reckey and T1.reckey = T3.reckey and valcarr not in ('ZZ','$$') and ";
                sql.WhereClause = sql.KeyWhereClause + existingWhereClause;
            }
            else
            {
                sql.FromClause = isReservationReport ? "ibtrips T1, iblegs T2"
                    : "hibtrips T1, hiblegs T2";
                sql.KeyWhereClause = "T1.reckey = T2.reckey and valcarr not in ('ZZ','$$') and ";
                sql.WhereClause = sql.KeyWhereClause + existingWhereClause;
            }

            sql.FieldList = @"T1.recloc, T1.reckey, valcarr, airchg, basefare, faretax, convert(int,0) as seg_cntr, acct, space(7) as flt_mkt, space(7) as flt_mkt2, space(6) as orgdestemp, convert(int,0) as RecordNo";

            sql.FieldList += isReservationReport
                ? @", convert(int,1) as plusmin "
                : @", convert(int,plusmin) as plusmin ";

            sql.OrderBy = "";

            return sql;
        }
    }
}
