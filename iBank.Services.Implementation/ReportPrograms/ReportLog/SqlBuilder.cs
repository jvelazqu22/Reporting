using System.Linq;
using Domain.Helper;
using Domain.Models;
using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.ReportPrograms.ReportLog
{
    public static class SqlBuilder
    {
        public static  SqlScript CreateScript(ReportGlobals globals)
        {
            var sql = new SqlScript {};

            sql.FromClause = "ibrptlog";
            sql.KeyWhereClause = string.Empty;
            sql.WhereClause = GetWhereClause(globals);

            sql.FieldList = @"rptLogNo, UserNumber, username, rptdate, processkey, rptProgram ";

            sql.OrderBy = string.Empty;

            return sql;
        }

        public static string GetReportLogSql(ReportGlobals globals)
        {
            return "select rptLogNo, UserNumber, username, rptdate, processkey, rptProgram from ibrptlog where " + GetWhereClause(globals);
        }

        public static string GetReportLogCriteriaSql(ReportGlobals globals)
        {
            return "select T1.rptLogNo, varName, varValue from ibrptlog T1, ibRptlogCrit T2 where " + GetWhereClause(globals) + 
                " and T1.rptLogNo = T2.rptLogNo and varName in ('BEGDATE','ENDDATE','INACCT','NOTINACCT')";
        }

        public static string GetOrganizationSql(ReportGlobals globals)
        {
            var whereClause = string.Empty;
            if (globals.User.AdminLevel != 1)
            {
                whereClause = " and Orgkey = " + globals.User.OrganizationKey;
            }
            else
            {
                var orgs = globals.GetParmValue(WhereCriteria.ORGANIZATION) + globals.GetParmValue(WhereCriteria.INORGANIZATION);
                if (!string.IsNullOrEmpty(orgs))
                {
                    whereClause = globals.IsParmValueOn(WhereCriteria.NOTINORGANIZTN)
                        ? " and Orgkey not in (" + AddSingleQuotes(orgs) + ")"
                        : " and Orgkey in (" + AddSingleQuotes(orgs) + ")";
                }
            }

            return "select UserNumber from ibUser where agency = '" + globals.Agency + "' " + whereClause;
        }

        private static string GetWhereClause(ReportGlobals globals)
        {
            var whereClause = "rptdate between '" + globals.BeginDate.Value.ToShortDateString() + "' and '" +
                globals.EndDate.Value.ToShortDateString() + " 11:59:59 PM' and agency = '" + globals.Agency + "'";

            var userIds = globals.GetParmValue(WhereCriteria.USERID) + globals.GetParmValue(WhereCriteria.INUSERID);

            //TODO: Test 
            if (!string.IsNullOrEmpty(userIds))
            {
                var notText = globals.IsParmValueOn(WhereCriteria.NOTINUSERID) ? " not " : string.Empty;
                whereClause += " and UserID " + notText + "in (" + AddSingleQuotes(userIds) + ")";
            }

            return whereClause;
        }

        private static string AddSingleQuotes(string list)
        {
            var userIdList = list.Split().ToList().Select(s => "'" + s.Trim() + "'");

            return string.Join(",", userIdList);
        }
    }
    
}
