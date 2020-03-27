using Domain.Helper;
using Domain.Models.ReportPrograms.TripChangesAir;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using iBank.Services.Implementation.Utilities.ClientData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.TripChanges.TripChangesAir
{
    public class TripChangeDataProcessor
    {
        public ReportGlobals Globals { get; set; }
        public BuildWhere BuildWhere { get; set; }
        public DateTime? BeginDate2 { get; set; }
        public DateTime? EndDate2 { get; set; }
        public string Where2 { get; set; }
        public string WhereClauseOriginal { get; set; }
        public string WhereTrip2 { get; set; }
        public string ChangeStampWhere { get; set; }
        public string CancelStampWhere { get; set; }
        public bool IncludeCancelledTrips { get; set; }
        public bool GoodUdid { get; set; }
        public int UdidNumber { get; set; }

        public TripChangeDataProcessor(ReportGlobals globals, BuildWhere buildWhere, bool goodUdid, int udidNumber)
        {
            Globals = globals;
            BuildWhere = buildWhere;
            GoodUdid = goodUdid;
            UdidNumber = udidNumber;
        }
        public List<RawData> GetRawData(bool IncludeCancelledTrips)
        {
            var tripCancelYn = Globals.GetParmValue(WhereCriteria.CANCELCODE);
            var changeStampFrom = Globals.GetParmValue(WhereCriteria.CHANGESTAMP).ToDateFromiBankFormattedString();
            var changeStampTo = Globals.GetParmValue(WhereCriteria.CHANGESTAMP2).ToDateFromiBankFormattedString();
            var cancelTimeFrom = Globals.GetParmValue(WhereCriteria.CANCELTIME).ToDateFromiBankFormattedString();
            var cancelTimeTo = Globals.GetParmValue(WhereCriteria.CANCELTIME2).ToDateFromiBankFormattedString();

            WhereClauseOriginal = string.IsNullOrWhiteSpace(BuildWhere.WhereClauseAdvanced)
                ? BuildWhere.WhereClauseFull
                : BuildWhere.WhereClauseFull + " AND " + BuildWhere.WhereClauseAdvanced;


            /** 07/14/2014 - IF THEY CHOSE TRIP CANCELLED = N OR Y, IT **
            **OVERRIDES WHATEVER THEY PICKED FOR TYPE OF CHANGE.     * */
            if (tripCancelYn == "Y")
                IncludeCancelledTrips = true;
            if (tripCancelYn == "N")
                IncludeCancelledTrips = false;

            string fromClause, whereClauseRawData;

            if (BeginDate2.HasValue && EndDate2.HasValue)
            {
                Where2 = " and TCL.changstamp <= '" + EndDate2.Value.ToShortDateString() +
                         " 11:59:59 PM' and TCL.changstamp >= '" + BeginDate2.Value.ToShortDateString() + "'";

                if (!string.IsNullOrEmpty(Globals.WhereText))
                    Globals.WhereText = Globals.WhereText + ";";

                Globals.WhereText += "Changes from " + BeginDate2.Value.ToShortDateString() + " to " +
                                     EndDate2.Value.ToShortDateString();
            }

            /** 07/14/2014 - ABILITY TO SPECIFY A RANGE OF CHANGE STAMPS OR CANCEL STAMPS. **
             ** "CANCEL STAMP" IS JUST THE changStamp COLUMN IN THE ibCancTrips TABLE.     **/

            if (changeStampFrom.HasValue && changeStampTo.HasValue)
            {
                ChangeStampWhere = " and TCL.changstamp <= '" + changeStampTo.Value.ToShortDateString() +
                                   "' and TCL.changstamp >= '" + changeStampFrom.Value.ToShortDateString() + "' ";
            }
            else if (changeStampFrom.HasValue)
            {
                ChangeStampWhere = " and TCL.changstamp >= '" + changeStampFrom.Value.ToShortDateString() + "' ";
            }
            else if (changeStampTo.HasValue)
            {
                ChangeStampWhere = " and TCL.changstamp <= '" + changeStampTo.Value.ToShortDateString() + "' ";
            }

            if (cancelTimeFrom.HasValue && cancelTimeTo.HasValue)
            {
                CancelStampWhere = " and T1.changstamp <= '" + cancelTimeTo.Value.ToShortDateString() +
                                   "' and T1.changstamp >= '" + cancelTimeFrom.Value.ToShortDateString() + "' ";
            }
            else if (cancelTimeFrom.HasValue)
            {
                CancelStampWhere = " and T1.changstamp >= '" + cancelTimeFrom.Value.ToShortDateString() + "' ";
            }
            else if (cancelTimeTo.HasValue)
            {
                CancelStampWhere = " and T1.changstamp <= '" + cancelTimeTo.Value.ToShortDateString() + "' ";
            }

            if (string.IsNullOrEmpty(CancelStampWhere) && !string.IsNullOrEmpty(ChangeStampWhere))
                CancelStampWhere = ChangeStampWhere.Replace("TCL.", "T1.");


            var whereChanges = BuildWhere.WhereClauseChanges;
            if (!string.IsNullOrEmpty(whereChanges))
                whereChanges = " and " + whereChanges;

            if (GoodUdid && UdidNumber != 0)
            {
                fromClause = "ibtrips T1, ibudids T3, changelog TCL";
                whereClauseRawData = "T1.reckey = T3.reckey and T1.reckey = TCL.reckey and valcarr not in ('ZZ','$$') and " + WhereClauseOriginal +
                                     Where2 + whereChanges + ChangeStampWhere;
            }
            else
            {
                fromClause = "ibtrips T1, changelog TCL";
                whereClauseRawData = "T1.reckey = TCL.reckey and valcarr not in ('ZZ','$$') and  " + WhereClauseOriginal + Where2 + whereChanges +
                                     ChangeStampWhere;
            }

            /** 07/14/2014 - MAKE SURE WE'RE NOT PICKING UP CANCELLED TRIP INFO. **/
            if (tripCancelYn == "N")
                whereClauseRawData += " and changecode != 101";

            var fieldList =
                "T1.reckey, acct, passlast, passfrst, mtggrpnbr, ticket, T1.recloc, bookdate, depdate, airchg, TCL.ChangeCode, TCL.ChangStamp, ChangeFrom, ChangeTo, PriorItin, convert(int,segnum) as segnum";

            var orderClause = "order by T1.reckey ";

            var fullSql = SqlProcessor.ProcessSql(fieldList, false, fromClause, whereClauseRawData, orderClause, Globals);
            return ClientDataRetrieval.GetOpenQueryData<RawData>(fullSql, Globals, BuildWhere.Parameters).ToList();
        }

        public List<RawData> GetCancelledRawData()
        {
            string fromClause, whereClauseRawData;
            if (!string.IsNullOrEmpty(Where2))
            {
                Where2 = Where2.Replace("TCL.changstamp", "T1.changstamp");
            }

            if (GoodUdid && UdidNumber != 0)
            {
                fromClause = "ibCancTrips T1, ibCancUdids T3";
                whereClauseRawData = "T1.reckey = T3.reckey and valcarr not in ('ZZ','$$') and " + WhereClauseOriginal + Where2 + CancelStampWhere;
            }
            else
            {
                fromClause = "ibCancTrips T1";
                whereClauseRawData = "valcarr not in ('ZZ','$$') and  " + WhereClauseOriginal + Where2 + CancelStampWhere;
            }

            var fieldList = "T1.reckey, acct, passlast, passfrst, mtggrpnbr, ticket, T1.recloc, bookdate, depdate, airchg, ChangStamp";

            var fullSql = SqlProcessor.ProcessSql(fieldList, false, fromClause, whereClauseRawData, string.Empty, Globals);

            return ClientDataRetrieval.GetOpenQueryData<RawData>(fullSql, Globals, BuildWhere.Parameters).ToList();

        }

        public List<RawData> GetRoutingData()
        {
            string fromClause, whereClauseRawData;

            if (GoodUdid && UdidNumber != 0)
            {
                fromClause = "ibTrips T1, ibLegs T2, ibUdids T3 ";
                whereClauseRawData =
                    "T1.reckey = T2.reckey and T1.reckey = T3.reckey and airline not in ('ZZ','$$') and valcarr not in ('ZZ','$$') and " +
                    WhereClauseOriginal;
            }
            else
            {
                fromClause = "ibTrips T1, ibLegs T2 ";
                whereClauseRawData = "T1.reckey = T2.reckey and airline not in ('ZZ','$$') and valcarr not in ('ZZ','$$') and " + WhereClauseOriginal;
            }

            var fieldList = "T1.reckey, acct, passlast, passfrst, mtggrpnbr, ticket, T1.recloc, bookdate, depdate, airchg";

            var orderClause = "order by T1.reckey, seqno";

            var fullSql = SqlProcessor.ProcessSql(fieldList, true, fromClause, whereClauseRawData, orderClause, Globals);

            return ClientDataRetrieval.GetOpenQueryData<RawData>(fullSql, Globals, BuildWhere.Parameters).ToList();

        }

        public List<RawData> GetCancelledRoutingData()
        {
            string fromClause, whereClauseRawData;

            if (GoodUdid && UdidNumber != 0)
            {
                fromClause = "ibCancTrips T1, ibCancLegs T2, ibCancUdids T3 ";
                whereClauseRawData = "T1.reckey = T2.reckey and T1.reckey = T3.reckey and airline not in ('ZZ','$$') and valcarr not in ('ZZ','$$') and " + WhereClauseOriginal + CancelStampWhere;
            }
            else
            {
                fromClause = "ibCancTrips T1, ibCancLegs T2";
                whereClauseRawData = "T1.reckey = T2.reckey and airline not in ('ZZ','$$') and valcarr not in ('ZZ','$$') and " + WhereClauseOriginal + CancelStampWhere;
            }

            var fieldList =
                "T1.reckey, acct, passlast, passfrst, mtggrpnbr, ticket, T1.recloc, bookdate, depdate, airchg, mode, origin, destinat, connect, convert(int,seqno) as seqno, airline, rdepdate, fltno, deptime, arrtime, convert(int,segnum) as segnum";

            var orderClause = "order by T1.reckey, seqno";

            var fullSql = SqlProcessor.ProcessSql(fieldList, false, fromClause, whereClauseRawData, orderClause, Globals);

            return ClientDataRetrieval.GetOpenQueryData<RawData>(fullSql, Globals, BuildWhere.Parameters).ToList();
        }
    }
}
