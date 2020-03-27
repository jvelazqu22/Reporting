using Domain.Models;

namespace iBank.Services.Implementation.ReportPrograms.TravelAuditReasonsbyMonth
{
    public static class SqlBuilder
    {

        public static SqlScript GetSql(bool hasUdid, string whereClause)
        {
            var script = new SqlScript();
            if (hasUdid)
            {
                script.FromClause = "ibtrips T1, iblegs T2, ibudids T3, ibTravAuth T7 ";
                script.KeyWhereClause = "T1.reckey = T3.reckey and T1.reckey = T7.reckey, and T1.reckey = T2.reckey and T1.agency = T7.agency and ";
                script.WhereClause = script.KeyWhereClause + whereClause;
            }
            else
            {
                script.FromClause = "ibtrips T1, iblegs T2, ibTravAuth T7 ";
                script.KeyWhereClause = "T1.reckey = T7.reckey and T1.reckey = T2.reckey and T1.agency = T7.agency and ";
                script.WhereClause = script.KeyWhereClause + whereClause;
            }
            script.FieldList = "T1.reckey, T1.acct, T1.depdate, T7.statusTime, T7.bookedGMT as bookdate, T7.outPolCods, T7.TravAuthNo, T7.authStatus, T7.SGroupNbr ";
            script.OrderBy = "order by T7.TravAuthNo, T1.reckey";
            return script;
        }

        public static SqlScript GetSqlLegs(bool hasUdid, string whereClause)
        {
            var script = new SqlScript();
            if (hasUdid)
            {
                script.FromClause = "ibtrips T1, ibLegs T2, ibudids T3, ibTravAuth T7 ";
                script.KeyWhereClause = "T1.reckey = T2.reckey and T1.reckey = T3.reckey and T1.reckey = T7.reckey and T1.agency = T7.agency and ";
                script.WhereClause = script.KeyWhereClause + "T1.ValCarr != 'ZZ' and " + whereClause;
            }
            else
            {
                script.FromClause = "ibtrips T1, ibLegs T2, ibTravAuth T7 ";
                script.KeyWhereClause = "T1.reckey = T2.reckey and T1.reckey = T7.reckey and T1.agency = T7.agency and ";
                script.WhereClause = script.KeyWhereClause + "T1.ValCarr != 'ZZ' and " + whereClause;
            }
            script.FieldList = "T1.reckey, T7.TravAuthNo, T2.origin, " +
                "T2.destinat, T2.airline, T2.rdepdate, T2.deptime, T2.class, " +
                "T2.connect, T2.mode, convert(int,T2.seqno) as seqno,convert(int,T2.segnum) as segnum, T2.actfare, T2.miscamt, " +
                "convert(int,T2.miles) as miles, T2.fltno, T2.ditcode ";
            script.OrderBy = "order by T7.TravAuthNo, T1.reckey, T2.seqno ";
            return script;
        }
    }
}
