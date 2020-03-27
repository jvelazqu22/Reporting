using Domain.Helper;
using Domain.Models;
using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.ReportPrograms.TripChanges.TripChangesMeetAndGreet
{
    public class MeetGreetSqlCreator
    {
        public SqlScript CreateScriptForIncludeCancelTrips(int udid, bool goodUdid, string whereClauseRawData, string Where2, string CancelStampWhere)
        {
            var sql = new SqlScript();

            if (goodUdid && udid != 0)
                sql.FromClause = "ibCancTrips T1, ibCancLegs T2, ibCancUdids T3";
            else
                sql.FromClause = "ibCancTrips T1, ibCancLegs T2";

            sql.WhereClause = whereClauseRawData + Where2 + CancelStampWhere;

            sql.FieldList = "T1.reckey, acct, passlast, passfrst, mtggrpnbr, bookdate, emailaddr, origin as lastorigin, 000 as seg_cntr, T1.recloc, ticket, changstamp";
            sql.OrderBy = "order by T1.reckey, seqno ";

            return sql;
        }

        public SqlScript CreateScriptForCancelUdidNumber(int UdidNumber, string whereClauseOriginal, string Where2, string CancelStampWhere)
        {
            var sql = new SqlScript();

            sql.FromClause = "ibCancTrips T1, ibCancLegs T2, ibCancUdids T3";
            sql.KeyWhereClause = "T1.reckey = T2.reckey and T1.reckey = T3.reckey and airline != 'ZZ' and valcarr not in ('ZZ','$$') and UdidNo = " + UdidNumber + " and ";
            sql.WhereClause = sql.KeyWhereClause + whereClauseOriginal + Where2 + CancelStampWhere;
            sql.FieldList = "T1.reckey, convert(int,UdidNo) as UdidNbr, UdidText";
            sql.GroupBy = " group by T1.reckey, UdidNo, UdidText";

            return sql;
        }

        public SqlScript CreateScriptForUdidNumber(int UdidNumber1, string whereClauseOriginal)
        {
            var sql = new SqlScript();

            sql.FromClause = "ibtrips T1, iblegs T2, ibudids T3";
            sql.KeyWhereClause = "T1.reckey = T2.reckey and T1.reckey = T3.reckey and airline != 'ZZ' and valcarr not in ('ZZ','$$') and UdidNo = " + UdidNumber1 + " and ";
            sql.WhereClause = sql.KeyWhereClause + whereClauseOriginal;
            sql.FieldList = "T1.reckey, convert(int,UdidNo) as UdidNbr, UdidText";
            sql.GroupBy = " group by T1.reckey, UdidNo, UdidText";

            return sql;
        }

        public SqlScript CreateScriptForRawData(ReportGlobals Globals, int udid, bool goodUdid, string whereClauseOriginal)
        {
            var sql = new SqlScript();

            if (goodUdid && udid != 0)
            {
                sql.FromClause = "ibtrips T1, iblegs T2, ibudids T3";
                sql.KeyWhereClause = "T1.reckey = T2.reckey and T1.reckey = T3.reckey and airline != 'ZZ' and valcarr not in ('ZZ','$$') and ";
                sql.WhereClause = sql.KeyWhereClause + whereClauseOriginal;
            }
            else
            {
                sql.FromClause = "ibtrips T1, iblegs T2";
                sql.KeyWhereClause = "T1.reckey = T2.reckey and airline != 'ZZ' and valcarr not in ('ZZ','$$') and ";
                sql.WhereClause = sql.KeyWhereClause + whereClauseOriginal;
            }

            sql.FieldList = "T1.reckey, acct, passlast, passfrst, mtggrpnbr, bookdate, emailaddr, origin as lastorigin, 000 as seg_cntr, T1.recloc, ticket";
            sql.OrderBy = "order by T1.reckey, seqno ";

            return sql;
        }

        public SqlScript CreateScriptForChangesData(ReportGlobals Globals, int udid, bool goodUdid, string SpecWhere, string whereChanges, string Where2, string ChangeStampWhere, string WhereTrip2)
        {
            var sql = new SqlScript();

            if (goodUdid && udid != 0)
            {
                sql.FromClause = "ibtrips T1, ibudids T3, changelog TCL";
                sql.KeyWhereClause = "T1.reckey = T3.reckey and T1.reckey = TCL.reckey and valcarr not in ('ZZ','$$') and ";
                sql.WhereClause = sql.KeyWhereClause + SpecWhere + whereChanges + Where2 + ChangeStampWhere + WhereTrip2;
            }
            else
            {
                sql.FromClause = "ibtrips T1, changelog TCL";
                sql.KeyWhereClause = "T1.reckey = TCL.reckey and valcarr not in ('ZZ','$$') and ";
                sql.WhereClause = sql.KeyWhereClause + SpecWhere + whereChanges + Where2 + ChangeStampWhere + WhereTrip2;
            }
            sql.WhereClause = sql.WhereClause.Replace("rArrDate", "ArrDate");

            if (Globals.ParmValueEquals(WhereCriteria.CANCELCODE, "N"))
            {
                //** 07/14/2014 - MAKE SURE WE'RE NOT PICKING UP CANCELLED TRIP INFO. **
                sql.WhereClause += " and changecode != 101";
            }

            sql.FieldList = "T1.reckey, convert(int, segnum) as segnum, TCL.ChangeCode, TCL.ChangStamp, ChangeFrom, ChangeTo, PriorItin, bookdate";
            sql.OrderBy = "order by T1.reckey ";

            return sql;
        }
    }
}
