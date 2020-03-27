using CODE.Framework.Core.Utilities.Extensions;
using Domain.Helper;
using Domain.Models;
using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.ReportPrograms.AgentAirActivity
{
    public static class SqlBuilder
    {
        public static SqlScript GetSql(ReportGlobals globals, bool isPreview, string whereClause)
        {
            var script = new SqlScript();

            var udidNumber = globals.GetParmValue(WhereCriteria.UDIDNBR).TryIntParse(0);
            if (udidNumber != 0)
            {
                script.FromClause = isPreview
                    ? "ibtrips T1, iblegs T2, ibudids T3"
                    : "hibtrips T1, hiblegs T2, hibudids T3";
                script.KeyWhereClause = "T1.reckey = T2.reckey and T1.reckey = T3.reckey and ";
                script.WhereClause = script.KeyWhereClause + "airline != 'ZZ' and valcarr not in ('ZZ','$$') and " + whereClause;
            }
            else
            {
                script.FromClause = isPreview
                    ? "ibtrips T1, iblegs T2"
                    : "hibtrips T1, hiblegs T2";
                script.KeyWhereClause = "T1.reckey = T2.reckey and ";
                script.WhereClause = script.KeyWhereClause + "airline != 'ZZ' and valcarr not in ('ZZ','$$') and " + whereClause;
            }
            script.FieldList =
                "T1.recloc, T1.reckey, invoice, ticket, agentid, passlast, passfrst, airchg, offrdchg, depdate, cardnum";

            script.FieldList += isPreview
                ? ", 0.00 as svcfee, convert(int,1) as plusmin, 0.00 as acommisn"
                : ", svcfee, convert(int,plusmin) as plusmin, acommisn";

            script.OrderBy = "order by T1.reckey, seqno ";
            return script;
        }

        public static SqlScript GetSqlSvcFees(ReportGlobals globals, string whereClause, string whereServices)
        {
            var script = new SqlScript();

            var udidNumber = globals.GetParmValue(WhereCriteria.UDIDNBR).TryIntParse(0);
            if (udidNumber != 0)
            {
                script.FromClause = "hibtrips T1, hibServices T6A, hibudids T3";
                script.KeyWhereClause = "T1.reckey = T6A.reckey and and T1.agency = T6A.agency and T1.reckey = T3.reckey and ";
                script.WhereClause = script.KeyWhereClause + "T6A.svcCode = 'TSF' and " +
                    whereClause + whereServices;
            }
            else
            {
                script.FromClause = "hibtrips T1, hibServices T6A";
                script.KeyWhereClause = "T1.reckey = T6A.reckey and T1.agency = T6A.agency and ";
                script.WhereClause = script.KeyWhereClause + "T6A.svcCode = 'TSF' and " +
                    whereClause + whereServices;
            }
            script.FieldList = "T1.reckey, T6A.svcamt as svcFee";

            return script;
        }
    }
}
