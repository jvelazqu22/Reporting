using Domain.Models;
using iBank.Services.Implementation.Shared.AdvancedClause;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;

namespace iBank.Services.Implementation.ReportPrograms.TopBottomTravelerAir
{
    public static class SqlBuilder
    {
        private static readonly WhereClauseWithAdvanceParamsHandler _whereClauseWithAdvanceParamsHandler = new WhereClauseWithAdvanceParamsHandler();
        // Creates the basic sql to get the raw data
        public static SqlScript GetSql(bool hasUdid, bool isPreview, BuildWhere buildWhere)
        {
            var script = new SqlScript();
            if (hasUdid)
            {
                script.FromClause = isPreview
                    ? "ibtrips T1, iblegs T2, ibudids T3"
                    : "hibtrips T1, hiblegs T2, hibudids T3";
                script.KeyWhereClause = "T1.reckey = T2.reckey and T1.reckey = T3.reckey and passlast is not null and valcarr not in ('ZZ','$$') and ";
                script.WhereClause = script.KeyWhereClause + buildWhere.WhereClauseFull;
            }
            else
            {
                script.FromClause = isPreview
                   ? "ibtrips T1, iblegs T2"
                   : "hibtrips T1, hiblegs T2";
                script.KeyWhereClause = "T1.reckey = T2.reckey and passlast is not null and valcarr not in ('ZZ','$$') and ";
                script.WhereClause = script.KeyWhereClause + buildWhere.WhereClauseFull;
            }
            //seqno, connect and DitCode need to collaspe and route
            script.FieldList = isPreview
                ? "T2.reckey, passlast, passfrst, airchg, offrdchg, invdate, bookdate, SourceAbbr, depdate, convert (int, 1) as plusmin, mode, origin, destinat, airline, convert(int, T2.SeqNo) as Seqno, connect, DitCode"
                : "T2.reckey, passlast, passfrst, airchg, offrdchg, invdate, bookdate, SourceAbbr, depdate, convert (int, plusmin) as plusmin, mode, origin, destinat, airline, convert(int, T2.SeqNo) as Seqno, connect,DitCode";

            //order so collapse should work correctly
            script.OrderBy = "order by T2.reckey, Seqno";
            script.WhereClause = _whereClauseWithAdvanceParamsHandler.GetWhereClauseWithAdvancedParametersAndUpdateSqlScriptIfNeeded(script, buildWhere, true);
            return script;

        }
    }
}
