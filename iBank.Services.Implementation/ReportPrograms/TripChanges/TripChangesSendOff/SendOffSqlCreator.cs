using Domain.Helper;
using Domain.Models;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using System;

namespace iBank.Services.Implementation.ReportPrograms.TripChanges.TripChangesSendOff
{
    public class SendOffSqlCreator
    {
        public string CancelledTripFrom { get { return "ibCancTrips T1, ibCancLegs T2, ibCancUdids T3"; } }

        public string GetChangeStampWhere(ReportGlobals globals)
        {
            var changeStampFrom = globals.GetParmValue(WhereCriteria.CHANGESTAMP).ToDateFromiBankFormattedString();
            var changeStampTo = globals.GetParmValue(WhereCriteria.CHANGESTAMP2).ToDateFromiBankFormattedString();

            var changeStampWhere = "";
            if (changeStampFrom.HasValue && changeStampTo.HasValue)
            {
                changeStampWhere = " and TCL.changstamp <= '" + changeStampTo.Value.ToShortDateString() +
                                   "' and TCL.changstamp >= '" + changeStampFrom.Value.ToShortDateString() + "' ";
            }
            else if (changeStampFrom.HasValue)
            {
                changeStampWhere = " and TCL.changstamp >= '" + changeStampFrom.Value.ToShortDateString() + "' ";
            }
            else if (changeStampTo.HasValue)
            {
                changeStampWhere = " and TCL.changstamp <= '" + changeStampTo.Value.ToShortDateString() + "' ";
            }

            return changeStampWhere;
        }

        public string GetCancelStampWhere(ReportGlobals globals, string changeStampWhere)
        {
            var cancelTimeFrom = globals.GetParmValue(WhereCriteria.CANCELTIME).ToDateFromiBankFormattedString();
            var cancelTimeTo = globals.GetParmValue(WhereCriteria.CANCELTIME2).ToDateFromiBankFormattedString();

            var cancelStampWhere = "";
            if (cancelTimeFrom.HasValue && cancelTimeTo.HasValue)
            {
                cancelStampWhere = " and T1.changstamp <= '" + cancelTimeTo.Value.ToShortDateString() +
                                   "' and T1.changstamp >= '" + cancelTimeFrom.Value.ToShortDateString() + "' ";
            }
            else if (cancelTimeFrom.HasValue)
            {
                cancelStampWhere = " and T1.changstamp >= '" + cancelTimeFrom.Value.ToShortDateString() + "' ";
            }
            else if (cancelTimeTo.HasValue)
            {
                cancelStampWhere = " and T1.changstamp <= '" + cancelTimeTo.Value.ToShortDateString() + "' ";
            }

            if (string.IsNullOrEmpty(cancelStampWhere) && !string.IsNullOrEmpty(changeStampWhere))
            {
                return changeStampWhere.Replace("TCL.", "T1.");
            }

            return cancelStampWhere;
        }

        public string GetDepartureDateWhere(DateTime beginDate, DateTime endDate)
        {
            return string.Format(" and DepDate <= '{0} 11:59:59 PM' and DepDate >= '{1}' ", endDate.AddDays(45).ToShortDateString(),
                                                                                            beginDate.AddDays(-45).ToShortDateString());
        }

        public string GetArrivalDateWhere(DateTime beginDate, DateTime endDate)
        {
            return string.Format(" and rArrDate <= '{0} 11:59:59 PM' and rArrDate >= '{1}' ", endDate.AddDays(5).ToShortDateString(),
                                                                                                beginDate.AddDays(-5).ToShortDateString());
        }

        public SqlScript GetRawDataSql(string existingWhereClause, bool udidExists)
        {
            var sql = new SqlScript();

            if (udidExists)
            {
                sql.FromClause = "ibtrips T1, iblegs T2, ibudids T3";
                sql.KeyWhereClause = "T1.reckey = T2.reckey and T1.reckey = T3.reckey and airline != 'ZZ' and valcarr not in ('ZZ','$$') and ";
            }
            else
            {
                sql.FromClause = "ibtrips T1, iblegs T2";
                sql.KeyWhereClause = "T1.reckey = T2.reckey and airline != 'ZZ' and valcarr not in ('ZZ','$$') and ";
            }

            sql.WhereClause = sql.KeyWhereClause + existingWhereClause;

            sql.FieldList = "T1.reckey, acct, passlast, passfrst, mtggrpnbr, bookdate, emailaddr, destinat as firstdest, 000 as seg_cntr, T1.recloc, ticket ";

            sql.OrderBy = "";

            return sql;
        }

        public SqlScript GetUdidSql(string existingWhereClause, int udidNumber)
        {
            var sql = new SqlScript();

            sql.FromClause = "ibtrips T1, iblegs T2, ibudids T3";
            sql.KeyWhereClause = $"T1.reckey = T2.reckey and T1.reckey = T3.reckey and airline != 'ZZ' and valcarr not in ('ZZ','$$') and UdidNo = {udidNumber} and ";
            sql.WhereClause = sql.KeyWhereClause + existingWhereClause;
            sql.FieldList = "T1.reckey, convert(int,UdidNo) as UdidNbr, UdidText";
            sql.OrderBy = "";
            sql.GroupBy = " group by T1.reckey, UdidNo, UdidText";

            return sql;
        }

        public string GetChangeStampOnBeginEndDateWhereClause(DateTime beginDate, DateTime endDate, string beginHour, string beginMinute, string beginAmOrPm)
        {
            return $" and T1.changstamp <= '{endDate.ToShortDateString()} 11:59:59 PM' and T1.changstamp >= '{beginDate.ToShortDateString()} {beginHour}:{beginMinute}:00 {beginAmOrPm}' ";
        }

        public SqlScript GetIncludeCancelledTripsSql(string existingWhereClause, bool udidExists)
        {
            var sql = new SqlScript();

            if (udidExists)
            {
                sql.FromClause = "ibCancTrips T1, ibCancLegs T2, ibCancUdids T3";
            }
            else
            {
                sql.FromClause = "ibCancTrips T1, ibCancLegs T2";
            }

            sql.WhereClause = existingWhereClause;
            sql.FieldList = "T1.reckey, acct, passlast, passfrst, mtggrpnbr, bookdate, emailaddr, destinat as firstdest, 000 as seg_cntr, T1.recloc, ticket, changstamp";
            sql.OrderBy = "";

            return sql;
        }

        public string GetWhereChangesClause(BuildWhere buildWhere)
        {
            var whereChanges = buildWhere.WhereClauseChanges;
            if (!string.IsNullOrEmpty(whereChanges))
            {
                whereChanges = " and " + whereChanges;
            }

            return whereChanges;
        }

        public string GetChangeStampWhereClauseOnBeginEndDateForUseWithChangelog(string existingChangeStampWhereOnBeginEndDate)
        {
            if (!string.IsNullOrEmpty(existingChangeStampWhereOnBeginEndDate))
            {
                existingChangeStampWhereOnBeginEndDate = existingChangeStampWhereOnBeginEndDate.Replace("T1.changstamp", "TCL.changstamp");
            }

            return existingChangeStampWhereOnBeginEndDate;
        }

        public SqlScript GetChangelogSql(bool udidExists, string specWhere, string whereChanges, string changeStampWhereOnBeginEndDate,
            string changeStampWhere, string departureDateWhere, bool excludeCancelledTrips)
        {
            var sql = new SqlScript();

            if (udidExists)
            {
                sql.FromClause = "ibtrips T1, ibudids T3, changelog TCL";
                sql.KeyWhereClause = "T1.reckey = T3.reckey and T1.reckey = TCL.reckey and valcarr not in ('ZZ','$$') and ";
            }
            else
            {
                sql.FromClause = "ibtrips T1, changelog TCL";
                sql.KeyWhereClause = "T1.reckey = TCL.reckey and valcarr not in ('ZZ','$$') and ";
            }

            sql.WhereClause = sql.KeyWhereClause + specWhere + whereChanges + changeStampWhereOnBeginEndDate + changeStampWhere + departureDateWhere;

            sql.WhereClause = sql.WhereClause.Replace("sArrDate", "ArrDate");

            if (excludeCancelledTrips)
            {
                sql.WhereClause += " and changecode != 101";
            }

            sql.FieldList = "T1.reckey, convert(int, segnum) as segnum, TCL.ChangeCode, TCL.ChangStamp, ChangeFrom, ChangeTo, PriorItin, bookdate";
            sql.OrderBy = "";

            return sql;
        }


    }
}
