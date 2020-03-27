using Domain.Models;

namespace iBank.Services.Implementation.ReportPrograms.WtsFareSavings
{
    public static class SqlBuilder
    {

        public static SqlScript GetSql(bool isPreview, string whereClause)
        {
            var script = new SqlScript();
            if (isPreview)
            {
                script.FromClause = "ibtrips T1, iblegs T2";
                script.FieldList = "T1.reckey,convert(int,seqno) as  seqno, acct, break1, break2, break3, " +
                                   "valcarr, 'A' as valcarMode, reascode, invoice, bookdate, invdate, depdate, ticket, " +
                                   "passlast, passfrst, origin, destinat, rdepdate, airline, mode, " +
                                   "origin as origOrigin, destinat as origDest, connect, class, " +
                                   "airchg, offrdchg, stndchg, convert(int,1) as  plusmin, exchange, origticket";
            }
            else
            {
                script.FromClause = "hibtrips T1, hiblegs T2";
                script.FieldList = "T1.reckey,convert(int,seqno) as seqno, acct, break1, break2, break3, " +
                                   "valcarr, valcarMode,reascode, invoice, bookdate, invdate, depdate, ticket, " +
                                   "passlast, passfrst, origin, destinat, rdepdate, airline, mode, " +
                                   "origOrigin, origDest, connect, class, airchg, offrdchg, stndchg, convert(int,plusmin) as plusmin, exchange, origticket";
            }
            script.WhereClause = "T1.reckey = T2.reckey and airline != 'ZZ' and " +
                                 "valcarr not in ('ZZ','$$') and " + whereClause;
            return script;
        }

        public static SqlScript GetSqlUdids(bool isPreview, string whereClause)
        {
            var script = new SqlScript
            {
                FromClause = isPreview ? "ibtrips T1, ibudids T3" : "hibtrips T1, hibudids T3",
                FieldList = "T1.reckey, udidtext",
                WhereClause = "T1.reckey = T3.reckey and valcarr not in ('ZZ','$$') and udidno = 1 and " + whereClause
            };

            return script;
        }
    }
}
