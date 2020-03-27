using Domain.Helper;
using Domain.Models.ReportPrograms.TripChangesMeetAndGreetReport;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using iBank.Services.Implementation.Utilities.ClientData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.TripChanges.TripChangesMeetAndGreet
{
    public class MeetGreetRawDataProcessor
    {
        MeetGreet _report;
        public MeetGreetRawDataProcessor(MeetGreet report)
        {
            _report = report;
        }

        public void UpdateGlobalWhereVariables(BuildWhere BuildWhere, ref string SpecWhere, DateTime? changeStampFrom, DateTime? changeStampTo, 
            ref string ChangeStampWhere, DateTime? cancelTimeFrom, DateTime? cancelTimeTo, ref string CancelStampWhere )
        {
            var RawDataList = new List<RawData>();

            SpecWhere = string.IsNullOrWhiteSpace(BuildWhere.WhereClauseAdvanced)
                ? BuildWhere.WhereClauseFull
                : BuildWhere.WhereClauseFull + " AND " + BuildWhere.WhereClauseAdvanced;

            /** 07/14/2014 - ABILITY TO SPECIFY A RANGE OF CHANGE STAMPS OR CANCEL STAMPS. **
             ** "CANCEL STAMP" IS JUST THE changStamp COLUMN IN THE ibCancTrips TABLE.     **/

            if (changeStampFrom.HasValue && changeStampTo.HasValue)
                ChangeStampWhere = " and TCL.changstamp <= '" + changeStampTo.Value.ToShortDateString() + "' and TCL.changstamp >= '" + changeStampFrom.Value.ToShortDateString() + "' ";
            else if (changeStampFrom.HasValue)
                ChangeStampWhere = " and TCL.changstamp >= '" + changeStampFrom.Value.ToShortDateString() + "' ";
            else if (changeStampTo.HasValue)
                ChangeStampWhere = " and TCL.changstamp <= '" + changeStampTo.Value.ToShortDateString() + "' ";

            if (cancelTimeFrom.HasValue && cancelTimeTo.HasValue)
                ChangeStampWhere = " and T1.changstamp <= '" + cancelTimeTo.Value.ToShortDateString() + "' and T1.changstamp >= '" + cancelTimeFrom.Value.ToShortDateString() + "' ";
            else if (cancelTimeFrom.HasValue)
                ChangeStampWhere = " and T1.changstamp >= '" + cancelTimeFrom.Value.ToShortDateString() + "' ";
            else if (cancelTimeTo.HasValue)
                ChangeStampWhere = " and T1.changstamp <= '" + cancelTimeTo.Value.ToShortDateString() + "' ";

            if (string.IsNullOrEmpty(CancelStampWhere) && !string.IsNullOrEmpty(ChangeStampWhere))
                CancelStampWhere = ChangeStampWhere.Replace("TCL.", "T1.");
        }

        public List<RawData> GetRawData(ReportGlobals Globals, string whereClauseOriginal, ref string whereClauseRawData)
        {
            var RawDataList = new List<RawData>();

            int udid;
            var goodUdid = int.TryParse(Globals.GetParmValue(WhereCriteria.UDIDNBR), out udid);
            var sql = new MeetGreetSqlCreator().CreateScriptForRawData(Globals, udid, goodUdid, whereClauseOriginal);
            whereClauseRawData = sql.WhereClause;

            //var fullSql = SqlProcessor.ProcessSql(sql.FieldList, true, sql.FromClause, sql.WhereClause, sql.OrderBy, Globals);
            //RawDataList = ClientDataRetrieval.GetUdidFilteredOpenQueryData<RawData>(fullSql, Globals, BuildWhere.Parameters, true).ToList();

            RawDataList = _report.RetrieveRawData<RawData>(sql, true, true).ToList();

            return RawDataList;
        }

        public List<UdidData> GetUdidsNumber(ref string UdidLabel1, int UdidNumber1, string whereClauseOriginal, ReportGlobals Globals, BuildWhere BuildWhere)
        {
            if (string.IsNullOrEmpty(UdidLabel1))
                UdidLabel1 = "Udid # " + UdidNumber1 + " text:";

            if (!UdidLabel1.EndsWith(":"))
                UdidLabel1 = UdidLabel1 + ":";

            var sql = new MeetGreetSqlCreator().CreateScriptForUdidNumber(UdidNumber1, whereClauseOriginal);
            List<UdidData> results = new List<UdidData>();

            //var fullSql = SqlProcessor.ProcessSql(sql.FieldList, false, sql.FromClause, sql.WhereClause, string.Empty, Globals);
            //fullSql += sql.GroupBy;
            //results = ClientDataRetrieval.GetUdidFilteredOpenQueryData<UdidData>(fullSql, Globals, BuildWhere.Parameters, true).ToList();

            results = _report.RetrieveRawData<UdidData>(sql, true, false).ToList();
            return results;
        }

        public List<UdidData> GetCancelledUdidsNumber(int UdidNumber, string whereClauseOriginal, string Where2, string CancelStampWhere, 
            ReportGlobals Globals, BuildWhere BuildWhere)
        {
            List<UdidData> results = new List<UdidData>();

            var sql = new MeetGreetSqlCreator().CreateScriptForCancelUdidNumber(UdidNumber, whereClauseOriginal, Where2, CancelStampWhere);
            //var fullSql = SqlProcessor.ProcessSql(sql.FieldList, false, sql.FromClause, sql.WhereClause, string.Empty, Globals);
            //fullSql += sql.GroupBy;
            //results = ClientDataRetrieval.GetUdidFilteredOpenQueryData<UdidData>(fullSql, Globals, BuildWhere.Parameters, true).ToList();

            results = _report.RetrieveRawData<UdidData>(sql, true, false).ToList();
            return results;
        }

        public void UpdateGlobalAndUdids(ReportGlobals Globals, DateTime? BeginDate2, DateTime? EndDate2, string Where2, bool IncludeCancelledTrips, ref int RowCount,
            BuildWhere BuildWhere, string whereClauseRawData, string CancelStampWhere, ref List<RawData> CancelledRawDataList, bool LegLevel,
            int UdidNumber1, int UdidNumber2, ref List<UdidData> UdidsNumber1, ref List<UdidData> UdidsNumber2, string whereClauseOriginal)
        {
            var beginHour = Globals.GetParmValue(WhereCriteria.BEGHOUR).PadLeft(2, '0');
            int beginHr;
            int.TryParse(beginHour, out beginHr);

            if (!(beginHr >= 1 && beginHr <= 12))
                beginHour = "12";

            var beginMinute = Globals.GetParmValue(WhereCriteria.BEGMINUTE).PadLeft(2, '0');
            int beginMin;
            int.TryParse(beginMinute, out beginMin);

            if (!(beginMin >= 0 && beginMin <= 59))
                beginMinute = "00";

            var beginAmpm = Globals.ParmValueEquals(WhereCriteria.BEGAMPM, "2") ? "PM" : "AM";

            if (BeginDate2.HasValue && EndDate2.HasValue)
            {
                Where2 = " and T1.changstamp <= '" + EndDate2.Value.ToShortDateString() + " 11:59:59 PM' and T1.changstamp >= '" +
                               BeginDate2.Value.ToShortDateString() + " " + beginHour + ":" + beginMinute + ":00 " +
                               beginAmpm + "' ";

                /**THE ibBldAdvWh FUNCTION STRIPS AWAY THE TRAILING ";". * *
                **WE NEED TO PUT IT BACK.*/
                if (string.IsNullOrEmpty(Globals.WhereText))
                    Globals.WhereText = Globals.WhereText + ";";

                if (beginHour != "12" || beginMinute != "00" || beginAmpm != "AM")
                {
                    Globals.WhereText += "Changes from " + BeginDate2.Value.ToShortDateString() + " at " + beginHour +
                                         ":" + beginMinute + " " + beginAmpm + " to " +
                                         EndDate2.Value.ToShortDateString() + " 11:59:59 PM; ";
                }
                else
                {
                    Globals.WhereText += "Changes from " + BeginDate2.Value.ToShortDateString() + " to " +
                                         EndDate2.Value.ToShortDateString();
                }
            }

            if (IncludeCancelledTrips)
            {
                int udid;
                var goodUdid = int.TryParse(Globals.GetParmValue(WhereCriteria.UDIDNBR), out udid);
                var sql = new MeetGreetSqlCreator().CreateScriptForIncludeCancelTrips(udid, goodUdid, whereClauseRawData, Where2, CancelStampWhere);
                var fullSql = SqlProcessor.ProcessSql(sql.FieldList, true, sql.FromClause, sql.WhereClause, sql.OrderBy, Globals);

                CancelledRawDataList = ClientDataRetrieval.GetOpenQueryData<RawData>(fullSql, Globals, BuildWhere.Parameters).ToList();

                if (!LegLevel)
                {
                    //Collapse cancelled data
                    CancelledRawDataList = Collapser<RawData>.Collapse(CancelledRawDataList, Collapser<RawData>.CollapseType.Last);
                }

                RowCount += CancelledRawDataList.Count;

                if (UdidNumber1 > 0)
                {
                    var cancelledUdidsNumber1 = GetCancelledUdidsNumber(UdidNumber1, whereClauseOriginal, Where2, CancelStampWhere, Globals, BuildWhere);
                    UdidsNumber1.AddRange(cancelledUdidsNumber1);
                }

                if (UdidNumber2 > 0)
                {
                    var cancelledUdidsNumber2 = GetCancelledUdidsNumber(UdidNumber2, whereClauseOriginal, Where2, CancelStampWhere, Globals, BuildWhere);
                    UdidsNumber2.AddRange(cancelledUdidsNumber2);
                }
            }

        }
    }
}
