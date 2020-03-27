using Domain.Models.ReportPrograms.TripChangesTypeSummaryReport;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using System;
using System.Collections.Generic;

namespace iBank.Services.Implementation.ReportPrograms.TripChanges.TripChangesTypeSummary
{
    public class TripChngSumRawDataProcessor
    {
        public void UpdateGlobalWhereVariables(BuildWhere BuildWhere, ref string WhereClauseOriginal, DateTime? changeStampFrom, DateTime? changeStampTo,
            ref string ChangeStampWhere, DateTime? cancelTimeFrom, DateTime? cancelTimeTo, ref string CancelStampWhere, DateTime? BeginDate2, 
            DateTime? EndDate2, ReportGlobals Globals, string Where2, bool GoodUdid, int UdidNumber)
        {
            var RawDataList = new List<RawData>();

            WhereClauseOriginal = string.IsNullOrWhiteSpace(BuildWhere.WhereClauseAdvanced)
                ? BuildWhere.WhereClauseFull
                : BuildWhere.WhereClauseFull + " AND " + BuildWhere.WhereClauseAdvanced;

            if (BeginDate2.HasValue && EndDate2.HasValue)
            {
                Where2 = " and TCL.changstamp <= '" + EndDate2.Value.ToShortDateString() +
                         " 11:59:59 PM' and TCL.changstamp >= '" + BeginDate2.Value.ToShortDateString();

                if (!string.IsNullOrEmpty(Globals.WhereText))
                    Globals.WhereText = Globals.WhereText + ";";

                Globals.WhereText += "Changes from " + BeginDate2.Value.ToShortDateString() + " to " +
                                     EndDate2.Value.ToShortDateString();
            }

            /*07 / 14 / 2014 - ABILITY TO SPECIFY A RANGE OF CHANGE STAMPS OR CANCEL STAMPS. **
            **"CANCEL STAMP" IS JUST THE changStamp COLUMN IN THE ibCancTrips TABLE.     * */

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

            //var whereChanges = BuildWhere.WhereClauseChanges;
            //if (!string.IsNullOrEmpty(whereChanges))
            //    whereChanges = " and " + whereChanges;

            //if (GoodUdid && UdidNumber != 0)
            //{
            //    fromClause = "ibtrips T1, ibudids T3, changelog TCL";
            //    whereClauseRawData = "T1.reckey = T3.reckey and T1.reckey = TCL.reckey and " + WhereClauseOriginal +
            //                         Where2 + whereChanges + ChangeStampWhere;
            //}
            //else
            //{
            //    fromClause = "ibtrips T1, changelog TCL";
            //    whereClauseRawData = "T1.reckey = TCL.reckey and " + WhereClauseOriginal + Where2 + whereChanges +
            //                         ChangeStampWhere;
            //}

            //var fieldList =
            //    "T1.reckey, TCL.ChangeCode ";

            //var fullSql = string.Format("select {0} from {1} where {2}", fieldList, fromClause, whereClauseRawData);

            //RawDataList = ClientDataRetrieval.GetOpenQueryData<RawData>(fullSql, Globals, BuildWhere.Parameters)
            //        .GroupBy(s => new { s.RecKey, s.ChangeCode },
            //            (k, g) => new RawData { RecKey = k.RecKey, ChangeCode = k.ChangeCode, NumChngs = g.Count() })
            //        .ToList();

            //return RawDataList;
        }

    }
}
