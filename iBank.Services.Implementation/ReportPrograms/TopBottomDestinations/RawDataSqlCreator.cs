using Domain.Models;

namespace iBank.Services.Implementation.ReportPrograms.TopBottomDestinations
{
    public static class RawDataSqlCreator
    {
        public static SqlScript CreateScript(string existingWhereClause, int udid, bool isReservationReport)
        {
            var sql = new SqlScript { WhereClause = existingWhereClause };

            if (udid > 0)
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

            sql.FieldList =
                @"T1.recloc, T1.reckey, invoice, ticket, acct, break1, break2, break3, passlast, passfrst, airchg, offrdchg, 00000000.0000 as svcfee, 'I' as SFTranType, depdate, invdate, bookdate, cardnum ";

            sql.FieldList = isReservationReport
                ? sql.FieldList = @"T1.reckey, T1.SourceAbbr, T1.ValCarr, airchg, convert(int,0) as seg_cntr, convert(int,1) as plusmin, '      ' as FirstOrg"
                : sql.FieldList = "T1.reckey, T1.SourceAbbr, T1.ValCarr, airchg, convert(int,0) as seg_cntr, convert(int,plusmin) as plusmin, '      ' as FirstOrg";

            sql.OrderBy = "order by T1.reckey, seqno";

            return sql;
        }
    }
}
