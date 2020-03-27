using System.Linq;
using Domain.Helper;
using Domain.Models;
using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.ReportPrograms.TopBottomUdids
{
    public static class SqlBuilder
    {
        public static string GetUdidSql(int udidNumber, ReportGlobals globals)
        {
            var cWhereUdid = " and udidno = " + udidNumber;
            var udidText = globals.GetParmValue(WhereCriteria.UDIDTEXT);
            if (!string.IsNullOrEmpty(udidText))
            {
                var udidTexts = udidText.Split('|').ToList().Select(s => "'" + s.Trim() + "'");
                cWhereUdid += " and udidtext in (" + string.Join(",", udidTexts) + ")";
                globals.WhereText += "Udid Text = " + udidText + ";";

            }

            return cWhereUdid;
        }

        public static SqlScript GetSql(bool isPreview, string whereClause)
        {
            var script = new SqlScript();
            script.FromClause = isPreview
                ? "ibtrips T1, ibudids T3"
                : "hibtrips T1, hibudids T3";

            script.WhereClause = "T1.reckey = T3.reckey and " + whereClause + " and udidtext is not null ";
            script.FieldList = "upper(udidtext) as udidtext";

            return script;

        }
    }
}
