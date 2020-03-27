using System;
using System.Linq;
using Domain.Helper;
using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.ReportPrograms.EventLog
{
    public static class SqlBuilder
    {
        public static string GetSql(ReportGlobals globals, DateTime begDateTime, DateTime endDateTime)
        {
            var sql = "select T2.eventType, T2.eventDesc, T1.UserNumber, T1.UserID, T1.UserName, T1.DateStamp, TargetUserID, ";
            sql += "convert(int, T1.eventCode) as eventCode, T2.eventTarget, T1.IPAddress  from eventLog T1, eventCodes T2 ";
            sql += "where " + GetWhereSqlClause(globals, begDateTime, endDateTime);

            return sql;
        }

        private static string GetWhereSqlClause(ReportGlobals globals, DateTime begDateTime, DateTime endDateTime)
        {
            string whereClause;

            if (globals.ClientType == ClientType.Sharer)
            {
                var agencies = globals.CorpAccountDataSources.Select(x => x.DataSource).ToList();
                var sqlFilter = "(";

                for (var counter = 0; counter < agencies.Count; counter++)
                {
                    sqlFilter += counter == agencies.Count
                        ? $"'{agencies[counter]}'"
                        : $"'{agencies[counter]}',";
                }
                sqlFilter += ") ";
                whereClause = "T1.agency in " + sqlFilter + " and dateStamp between '" + begDateTime + "' and '" + endDateTime + "'";
            }
            else
            {
                whereClause = "T1.agency = '" + globals.Agency + "' and dateStamp between '" + begDateTime + "' and '" + endDateTime + "'";
            }


            var eventCategory = globals.GetParmValue(WhereCriteria.DDEVENTCATEGORY);
            if (!string.IsNullOrEmpty(eventCategory))
            {
                whereClause += " and T2.eventType = '" + eventCategory + "'";
            }

            var eventCode = globals.GetParmValue(WhereCriteria.DDEVENTCODE);
            if (!string.IsNullOrEmpty(eventCode))
            {
                whereClause += " and T2.eventCode = '" + eventCode + "'";
            }

            var userIds = globals.GetParmValue(WhereCriteria.USERID) + globals.GetParmValue(WhereCriteria.INUSERID);
            if (!string.IsNullOrEmpty(userIds))
            {
                if (globals.IsParmValueOn(WhereCriteria.NOTINUSERID))
                {
                    whereClause += " and UserID not in (" + AddSingleQuotes(userIds) + ")";
                }
                else
                {
                    whereClause += " and UserID in (" + AddSingleQuotes(userIds) + ")";
                }
            }

            var targetOrgs = globals.GetParmValue(WhereCriteria.TARGETORG) + globals.GetParmValue(WhereCriteria.INTARGETORG);
            if (!string.IsNullOrEmpty(targetOrgs))
            {
                if (globals.IsParmValueOn(WhereCriteria.NOTINTARGETORG))
                {
                    whereClause += " and TargetUserID not in (" + AddSingleQuotes(targetOrgs) + ")";
                }
                else
                {
                    whereClause += " and TargetUserID in (" + AddSingleQuotes(targetOrgs) + ")";
                }
                whereClause += " and eventTarget = 'ORGANIZATION'";
            }

            var targetUsers = globals.GetParmValue(WhereCriteria.TARGETUSERID) + globals.GetParmValue(WhereCriteria.INTARGETUSERID);
            if (!string.IsNullOrEmpty(targetUsers))
            {
                if (globals.IsParmValueOn(WhereCriteria.NOTINTARGUSERID))
                {
                    whereClause += " and TargetUserID not in (" + AddSingleQuotes(targetUsers) + ")";
                }
                else
                {
                    whereClause += " and TargetUserID in (" + AddSingleQuotes(targetUsers) + ")";
                }
                whereClause += " and eventTarget = 'USER'";
            }

            var targetStyleGroups = globals.GetParmValue(WhereCriteria.TARGSTYLEGRP) + globals.GetParmValue(WhereCriteria.INTARGSTYLEGRP);
            if (!string.IsNullOrEmpty(targetStyleGroups))
            {
                if (globals.IsParmValueOn(WhereCriteria.NOTINTARGSTYLGRP))
                {
                    whereClause += " and TargetUserID not in (" + AddSingleQuotes(targetStyleGroups) + ")";
                }
                else
                {
                    whereClause += " and TargetUserID in (" + AddSingleQuotes(targetStyleGroups) + ")";
                }
                whereClause += " and eventTarget = 'STYLEGROUP'";
            }

            if (globals.ParmHasValue(WhereCriteria.ACCT) || globals.ParmHasValue(WhereCriteria.INACCT))
            {
                whereClause += " and eventTarget = 'ACCOUNT'";
            }

            var ipAddress = globals.GetParmValue(WhereCriteria.TXTIPADDRESS);
            if (!string.IsNullOrEmpty(ipAddress))
            {
                whereClause += " and T1.IPAddress = '" + ipAddress + "'";
            }

            if (globals.IsParmValueOn(WhereCriteria.CBEXCLEVENTCODE101))
            {
                whereClause += " and T1.eventCode != '101' ";
            }

            whereClause = "T1.eventCode = T2.eventCode and T1.eventCode not between 800 and 899 and " + whereClause;

            return whereClause;
        }

        private static string AddSingleQuotes(string list)
        {
            var userIdList = list.Split().ToList().Select(s => "'" + s.Trim() + "'");

            return string.Join(",", userIdList);
        }
    }
}
