using Domain.Models;
using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.ReportPrograms.TopBottomExceptionReason
{
    public static class SqlBuilder
    {
        public static SqlScript GetSql(bool hasUdid, bool isPreview, string whereClause, ReportGlobals globals)
        {
            var script = new SqlScript();
            if (hasUdid)
            {
                script.FromClause = isPreview
                    ? "ibtrips T1, ibudids T3"
                    : "hibtrips T1, hibudids T3";
                script.KeyWhereClause = "T1.reckey = T3.reckey and reascode is not null and reascode != '  ' and reascode not in (" + globals.AgencyInformation.ReasonExclude + ") and ";
                script.WhereClause = script.KeyWhereClause + whereClause;
            }
            else
            {
                script.FromClause = isPreview
                   ? "ibtrips T1"
                   : "hibtrips T1";
                script.KeyWhereClause = " reascode is not null and reascode != '  ' and reascode not in (" + globals.AgencyInformation.ReasonExclude + ") and ";
                script.WhereClause = script.KeyWhereClause + whereClause;
            }

            script.FieldList = isPreview
                 ? "T1.RecKey,'AIR' as type, reascode, passlast, passfrst, convert (int,1) as  plusmin, offrdchg, airchg, basefare, acct"
                 : "T1.RecKey,'AIR' as type, reascode, passlast, passfrst, convert (int,plusmin) as plusmin, offrdchg, airchg, basefare, acct";

            return script;

        }
    }
}
