using CODE.Framework.Core.Utilities.Extensions;
using Domain.Helper;
using Domain.Models;
using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.ReportPrograms.AgentSummary
{
    public static class SqlBuilder
    {
        public static SqlScript GetSql(ReportGlobals globals,string whereClause)
        {
            var script = new SqlScript();

            var udidNumber = globals.GetParmValue(WhereCriteria.UDIDNBR).TryIntParse(-1);
            if (udidNumber > 0)
            {
                script.FromClause = "hibtrips T1, hibudids T3";
                script.KeyWhereClause = "T1.reckey = T3.reckey and valcarr != 'ZZ' and ";
                script.WhereClause = script.KeyWhereClause + whereClause;
            }
            else
            {
                script.FromClause = "hibtrips T1";
                script.KeyWhereClause = "valcarr != 'ZZ' and ";
                script.WhereClause = script.KeyWhereClause + whereClause;
            }
            script.FieldList = "T1.reckey,agentid, convert(int,plusmin) as plusmin, acommisn as commission, airchg";
            return script;
        }
    }
}
