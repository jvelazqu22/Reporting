using Domain.Models;
using iBank.Services.Implementation.Utilities;

namespace iBank.Services.Implementation.ReportPrograms.RailActivity
{
    public class RailActivitySqlCreator
    {
        public SqlScript CreateScript(string existingWhereClause, int udid, bool isReservationReport)
        {
            var sql = new SqlScript { WhereClause = existingWhereClause };
            var transformer = new SqlTransformer();

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

            sql.FieldList = @"T1.recloc, T1.reckey, invoice, ticket, acct, break1, break2, break3, passlast, passfrst, airchg, offrdchg, 0.0 as svcfee, 'I' as SFTranType, depdate, invdate, bookdate, cardnum ";
            sql.FieldList += (isReservationReport) 
                ? ", 'I' as trantype, exchange, PseudoCity, origticket, convert(int,1) as plusmin" 
                : ", ' ' as PseudoCity, trantype, exchange, origticket, convert(int,plusmin) as plusmin, Space(13) as flt_mkt, Space(13) as flt_mkt2 ";

            sql.OrderBy = "order by T1.reckey, seqno ";

            return sql;
        }
    }
}
