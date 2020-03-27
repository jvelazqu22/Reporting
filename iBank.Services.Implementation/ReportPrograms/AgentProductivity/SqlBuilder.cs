using Domain.Helper;
using Domain.Models;
using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.ReportPrograms.AgentProductivity
{
    public static class SqlBuilder
    {
        public static SqlScript GetSql(ReportGlobals globals, bool isPreview, string whereClause)
        {
            var script = new SqlScript();
            var agentColumn = GetAgentColumn(globals.GetParmValue(WhereCriteria.DDAGENTTYPE));
            if (isPreview)
            {
                script.FromClause = "ibtrips T1";
                script.FieldList = $"reckey, {agentColumn} as AgentID, valcarr, 'A' as valcarMode, airchg";
            }
            else
            {
                script.FromClause = "hibtrips T1";
                script.FieldList = $"reckey, {agentColumn} as AgentID, valcarr, valcarMode, airchg";
            }

            script.WhereClause =
                $"{agentColumn} is not null and {agentColumn}  != '  ' and valcarr is not null and valcarr not in ('ZZ','$$','  ') and {whereClause}";
            
            return script;
        }

        private static string GetAgentColumn(string agentType)
        {
            switch (agentType)
            {
                case "2":
                    return "TkAgent";
                case "3":
                    return "BkAgent";
                default:
                    return "AgentID";
            }
        }

    }
}
