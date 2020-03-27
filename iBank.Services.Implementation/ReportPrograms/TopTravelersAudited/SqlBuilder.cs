using Domain.Models;

namespace iBank.Services.Implementation.ReportPrograms.TopTravelersAudited
{
    public static class SqlBuilder
    {
        public static SqlScript GetSql(bool hasUdid, string whereClause)
        {
            var script = new SqlScript();

            if (hasUdid)
            {
                script.FromClause = "ibtrips T1, ibudids T3, ibTravAuth T7 ";
                script.WhereClause = "T1.reckey = T3.reckey and T1.reckey = T7.reckey and T1.agency = T7.agency and " + whereClause.Replace("bookdate", "bookedgmt");
                script.KeyWhereClause = "T1.reckey = T3.reckey and T1.reckey = T7.reckey and T1.agency = T7.agency and ";
            }
            else
            {
                script.FromClause = "ibtrips T1, ibTravAuth T7 ";
                script.WhereClause = "T1.reckey = T7.reckey and T1.agency = T7.agency and " + whereClause.Replace("bookdate", "bookedgmt");
                script.KeyWhereClause = "T1.reckey = T7.reckey and T1.agency = T7.agency and ";
            }
            script.FieldList = "T1.AirChg, T1.passlast, T1.passfrst, T1.reckey, T7.statustime, T7.outPolCods, T7.TravAuthNo, T7.authStatus, T7.SGroupNbr ";
            script.OrderBy = "order by T7.TravAuthNo, T1.reckey";
            return script;
        }

        public static SqlScript GetSqlLegs(bool hasUdid, string whereClause)
        {
            var script = new SqlScript();

            if (hasUdid)
            {
                script.FromClause = "ibtrips T1, ibLegs T2, ibudids T3, ibTravAuth T7 ";
                script.WhereClause = "T1.reckey = T2.reckey and T1.reckey = T3.reckey and T1.reckey = T7.reckey and T1.agency = T7.agency and T1.ValCarr != 'ZZ' and " + whereClause;
                script.KeyWhereClause = "T1.reckey = T2.reckey and T1.reckey = T3.reckey and T1.reckey = T7.reckey and T1.agency = T7.agency and T1.ValCarr != 'ZZ' and ";
            }
            else
            {
                script.FromClause = "ibtrips T1, ibLegs T2, ibTravAuth T7 ";
                script.WhereClause = "T1.reckey = T2.reckey and T1.reckey = T7.reckey and T1.agency = T7.agency and T1.ValCarr != 'ZZ' and " + whereClause;
                script.KeyWhereClause = "T1.reckey = T2.reckey and T1.reckey = T7.reckey and T1.agency = T7.agency and T1.ValCarr != 'ZZ' and ";
            }
            script.FieldList = @"T1.reckey, T7.TravAuthNo, T2.origin, " +
                               "T2.destinat, T2.airline, T2.rdepdate, T2.deptime, T2.class, " +
                               "T2.connect, T2.mode,convert(int,T2.seqno) as seqno, convert(int,T2.segnum) as segnum, T2.actfare, T2.miscamt, " +
                               "convert(int,T2.miles) as miles, T2.fltno, T2.ditcode ";
            script.OrderBy = "order by T7.TravAuthNo, T1.reckey, T2.seqno ";
            return script;
        }

        public static SqlScript GetSqlHotel(bool hasUdid, string whereClause)
        {
            var script = new SqlScript();

            if (hasUdid)
            {
                script.FromClause = "ibtrips T1, ibHotel T5, ibudids T3, ibTravAuth T7 ";
                script.WhereClause = "T1.reckey = T5.reckey and T1.reckey = T3.reckey and T1.reckey = T7.reckey and T1.agency = T7.agency and " + whereClause;
                script.KeyWhereClause = "T1.reckey = T5.reckey and T1.reckey = T3.reckey and T1.reckey = T7.reckey and T1.agency = T7.agency and ";
            }
            else
            {
                script.FromClause = "ibtrips T1, ibHotel T5, ibTravAuth T7 ";
                script.WhereClause = "T1.reckey = T5.reckey and T1.reckey = T7.reckey and T1.agency = T7.agency and " + whereClause;
                script.KeyWhereClause = "T1.reckey = T5.reckey and T1.reckey = T7.reckey and T1.agency = T7.agency and ";
            }
            script.FieldList = "T1.passlast, T1.passfrst, T1.reckey, T5.nights, T5.rooms,T5.bookrate, T7.TravAuthNo, airchg as BookVolume, T5.datein, T1.invdate, T1.bookdate ";
            script.OrderBy = "order by T7.TravAuthNo, T1.reckey";
            return script;
        }

        public static SqlScript GetSqlCar(bool hasUdid, string whereClause)
        {
            var script = new SqlScript();

            if (hasUdid)
            {
                script.FromClause = "ibtrips T1, ibCar T4, ibudids T3, ibTravAuth T7 ";
                script.WhereClause = "T1.reckey = T4.reckey and T1.reckey = T3.reckey and T1.reckey = T7.reckey and T1.agency = T7.agency and " + whereClause;
                script.KeyWhereClause = "T1.reckey = T4.reckey and T1.reckey = T3.reckey and T1.reckey = T7.reckey and T1.agency = T7.agency and ";
            }
            else
            {
                script.FromClause = "ibtrips T1, ibCar T4, ibTravAuth T7 ";
                script.WhereClause = "T1.reckey = T4.reckey and T1.reckey = T7.reckey and T1.agency = T7.agency and " + whereClause;
                script.KeyWhereClause = "T1.reckey = T4.reckey and T1.reckey = T7.reckey and T1.agency = T7.agency and ";
            }
            script.FieldList = "T1.passlast, T1.passfrst, T1.reckey, T4.days, T4.numcars, T4.abookrat, T7.TravAuthNo ";
            script.OrderBy = "order by T7.TravAuthNo, T1.reckey";
            return script;
        }

    }
}
