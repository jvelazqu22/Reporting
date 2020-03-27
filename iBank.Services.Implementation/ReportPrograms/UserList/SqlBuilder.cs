using System.Collections.Generic;
using Domain.Helper;
using Domain.Models;
using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.ReportPrograms.UserList
{
    public static class SqlBuilder
    {

        public static SqlScript GetSql(ReportGlobals globals)
        {
            var script = new SqlScript
            {
                WhereClause = BuildWhere(globals) + " and T1.sGroupNbr = T3.sGroupNbr",
                FromClause = "ibuser T1, Organizations T2, styleGroup T3",
                FieldList = "UserNumber, userid, password, firstname, lastname, " +
                            "emailaddr, convert(int,allaccts) as allaccts, T1.lastlogin, T2.orgname, T2.purgeinact, " +
                            "convert(int,T2.inactdays) as inactdays, T2.purgetemps, convert(int, T2.tempsdays) as tempsdays, T1.Reports, convert(int, T1.AdminLvl) as AdminLvl, " +
                            "T1.pwencrypt, T1.sGroupNbr, T3.sGroupName ",
                OrderBy = " order by T2.OrgName, lastname, firstname"
            };
            
            return script;
        }

        public static SqlScript GetAcctSql(ReportGlobals globals)
        {
            var script = new SqlScript
            {
                FieldList = "T1.UserNumber, acct ",
                FromClause = "ibuser T1, Organizations T2, useraccts T3",
                WhereClause = "T1.UserNumber = T3.UserNumber and " + BuildWhere(globals)
            };
            
            return script;
        }

        public static SqlScript GetAltsSql(ReportGlobals globals)
        {
            var script = new SqlScript
            {
                FieldList = "T1.UserNumber, fieldData ",
                FromClause = "ibuser T1, Organizations T2, ibUserExtras T3 ",
                WhereClause = "T1.UserNumber = T3.userNumber and FieldFunction IN " +
                              "('TA_ALTAUTHUSER1', 'TA_ALTAUTHUSER2') and " + BuildWhere(globals)
            };
            
            return script;
        }

        public static SqlScript GetNewCriteria(ReportGlobals globals)
        {
            var script = new SqlScript
            {
                FieldList = "T1.UserNumber, fieldFunction, fieldData ",
                FromClause = "ibuser T1, Organizations T2, ibUserExtras T3 ",
                WhereClause =
                    "T1.UserNumber = T3.userNumber and FieldFunction IN ('ANALYTICS', 'ALLOW_DASHBOARD', 'TAEMAILFILTER') and " +
                    BuildWhere(globals)
            };
            
            return script;
        }

        private static string BuildWhere(ReportGlobals globals)
        {
            var whereClause = "";
            if (globals.ClientType != ClientType.Sharer)
            {
                whereClause += "T1.agency = '" + globals.Agency + "' and ";
            }
            whereClause += "T1.OrgKey = T2.OrgKey";
            var orgKey = globals.GetParmValue(WhereCriteria.COMPNAME).Replace("'", "\"");
            if (!string.IsNullOrEmpty(orgKey))
            {
                whereClause += " and T2.OrgName = '" + orgKey + "' ";
            }
            var adminLvl = globals.GetParmValue(WhereCriteria.DDADMINLVL);
            if (new List<string> { "1", "2", "3", "4" }.Contains(adminLvl))
            {
                whereClause += " and T1.AdminLvl = " + adminLvl;
            }

            var rptAccess = globals.GetParmValue(WhereCriteria.DDRPTACCESS);
            switch (rptAccess)
            {
                case "1":
                    whereClause += " and T1.reports = 1";
                    break;
                case "2":
                    whereClause += " and T1.reports = 0";
                    break;
            }

            return whereClause;
        }
    }
}
