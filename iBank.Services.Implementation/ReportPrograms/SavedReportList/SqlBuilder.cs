using Domain.Models;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared.AdvancedClause;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;

namespace iBank.Services.Implementation.ReportPrograms.SavedReportList
{
    public static class SqlBuilder
    {
        public static SqlScript CreateScript(ReportGlobals globals, string whereClause, BuildWhere buildWhere, bool includeAllLegs)
        {
            var script = new SqlScript
            {
                FromClause = "savedrpt1 t1, ibuser t3 ",
                FieldList =
                    "convert(int, t1.processkey) as processkey, t1.userrptnam, t3.lastname, t3.firstname, lastused ",
                OrderBy = "",
                WhereClause = " t1.usernumber = t3.usernumber and  t1.agency = '" + globals.Agency +
                              "' and t1.UserNumber = " + globals.UserNumber
            };

            script.WhereClause = new WhereClauseWithAdvanceParamsHandler().GetWhereClauseWithAdvancedParametersAndUpdateSqlScriptIfNeeded(script, buildWhere, includeAllLegs);
            return script;
        }

        public static string GetSql(ReportGlobals globals)
        {
            var sql = "SELECT convert(int, t1.processkey) as processkey, t1.userrptnam, t3.lastname, t3.firstname, " +
                      "lastused FROM savedrpt1 t1, ibuser t3 where t1.usernumber = t3.usernumber and  t1.agency = '" + 
                      globals.Agency + "' and t1.UserNumber = " + globals.UserNumber;

            return sql;
        }

    }
}
