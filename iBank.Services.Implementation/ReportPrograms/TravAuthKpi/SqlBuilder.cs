using Domain.Models;
using Domain.Models.ReportPrograms.TravAuthKpi;
using iBank.Services.Implementation.Shared.AdvancedClause;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;

namespace iBank.Services.Implementation.ReportPrograms.TravAuthKpi
{
    public static class SqlBuilder
    {
        private static readonly WhereClauseWithAdvanceParamsHandler _whereClauseWithAdvanceParamsHandler = new WhereClauseWithAdvanceParamsHandler();
        public static SqlScript GetSqlAuthStatusTotals(BuildWhere buildWhere, string whereClause, WhereClauses clauses)
        {
            var script = new SqlScript
            {
                FromClause = "ibtrips T1, ibTravAuth T7 ",
                WhereClause =
                    "T1.reckey = T7.reckey and T1.agency = T7.agency and " + clauses.WhereCyMth + whereClause +
                    " and authstatus in ('A','P','D','E','N')",
                KeyWhereClause = "T1.reckey = T7.reckey and T1.agency = T7.agency and ",
                FieldList = "T7.authStatus, T1.reckey "
            };

            script.WhereClause = _whereClauseWithAdvanceParamsHandler.GetWhereClauseWithAdvancedParametersAndUpdateSqlScriptIfNeeded(script, buildWhere, true);

            return script;
            
        }

        public static SqlScript GetSql14DayDomestic(BuildWhere buildWhere, string whereClause, string dateClause)
        {
            var script = new SqlScript
            {
                FromClause = "ibtrips T1, ibTravAuth T7 ",
                WhereClause = @"T1.reckey = T7.reckey and T1.agency = T7.agency and " +
                    dateClause + whereClause + " and domintl = 'D' and " + 
                    "T1.bookdate < (T1.depdate - 14) and T1.valcarr != 'ZZ' and " + 
                    "(valcarMode != 'R' or valcarMode is null)",
                KeyWhereClause = "T1.reckey = T7.reckey and T1.agency = T7.agency and ",
                FieldList = "count(distinct T1.reckey) as value "
            };

            script.WhereClause = _whereClauseWithAdvanceParamsHandler.GetWhereClauseWithAdvancedParametersAndUpdateSqlScriptIfNeeded(script, buildWhere, true);
            return script;

        }

        public static SqlScript GetSql21DayInternational(BuildWhere buildWhere, string whereClause, string dateClause)
        {
            var script = new SqlScript
            {
                FromClause = "ibtrips T1, ibTravAuth T7 ",
                WhereClause = @"T1.reckey = T7.reckey and T1.agency = T7.agency and " +
                    dateClause + whereClause + " and domintl != 'D' and " +
                    "T1.bookdate < (T1.depdate - 21) and T1.valcarr != 'ZZ' and " +
                    "(valcarMode != 'R' or valcarMode is null)",
                KeyWhereClause = "T1.reckey = T7.reckey and T1.agency = T7.agency and ",
                FieldList = "count(distinct T1.reckey) as value "
            };

            script.WhereClause = _whereClauseWithAdvanceParamsHandler.GetWhereClauseWithAdvancedParametersAndUpdateSqlScriptIfNeeded(script, buildWhere, true);
            return script;

        }

        public static SqlScript GetSqlAcceptedLowestFare(BuildWhere buildWhere, string whereClause, string dateClause)
        {
            var script = new SqlScript
            {
                FromClause = "ibtrips T1, ibTravAuth T7 ",
                WhereClause = @"T1.reckey = T7.reckey and T1.agency = T7.agency and " +
                    dateClause + whereClause + " and domintl != 'D' " +
                    " and airchg <= offrdchg  and T1.valcarr != 'ZZ' and " +
                    "(valcarMode != 'R' or valcarMode is null)",
                KeyWhereClause = "T1.reckey = T7.reckey and T1.agency = T7.agency and ",
                FieldList = "count(distinct T1.reckey) as value "
            };

            script.WhereClause = _whereClauseWithAdvanceParamsHandler.GetWhereClauseWithAdvancedParametersAndUpdateSqlScriptIfNeeded(script, buildWhere, true);
            return script;

        }

        public static SqlScript GetSqlHotelIncluded(BuildWhere buildWhere, string whereClause, string dateClause)
        {
            var script = new SqlScript
            {
                FromClause = "ibtrips T1, ibTravAuth T7 ",
                WhereClause = @"T1.reckey = T7.reckey and " + 
                    "T1.agency = T7.agency and " + dateClause + whereClause + 
                    " and T1.valcarr != 'ZZ' and (valcarMode != 'R' or valcarMode is null) " + 
                    "and T1.reckey in " + 
                    "(select distinct T1.reckey from ibtrips T1, ibHotel T5, ibTravAuth T7 " + 
                    "where T1.reckey = T7.reckey and T1.agency = T7.agency and " + 
                    "T1.reckey = T5.reckey and T1.agency = T5.agency and " +
                    dateClause + whereClause + ")",
                KeyWhereClause = "T1.reckey = T7.reckey and T1.agency = T7.agency and ",
                FieldList = "count(distinct T1.reckey) as value "
            };

            script.WhereClause = _whereClauseWithAdvanceParamsHandler.GetWhereClauseWithAdvancedParametersAndUpdateSqlScriptIfNeeded(script, buildWhere, true);
            return script;
        }

        public static SqlScript GetSqlTopBottomTraveler(BuildWhere buildWhere, string whereClause, string dateClause)
        {
            var script = new SqlScript
            {
                FromClause = "ibtrips T1, ibTravAuth T7 ",
                WhereClause = @"T1.reckey = T7.reckey and T1.agency = T7.agency and " +
                    dateClause + whereClause + " and T7.authstatus in ('A','P','D','E') ",
                KeyWhereClause = "T1.reckey = T7.reckey and T1.agency = T7.agency and ",
                FieldList = "T1.passlast, T1.passfrst, T7.authStatus,T1.Reckey "
            };

            script.WhereClause = _whereClauseWithAdvanceParamsHandler.GetWhereClauseWithAdvancedParametersAndUpdateSqlScriptIfNeeded(script, buildWhere, true);
            return script;
        }

        public static SqlScript GetSqlTopBottomApprovers(BuildWhere buildWhere, string whereClause, string dateClause)
        {
            var script = new SqlScript
            {
                FromClause = "ibtrips T1, ibTravAuth T7, ibTravAuthorizers T8",
                WhereClause = @"T1.reckey = T7.reckey and T1.agency = T7.agency and T7.TravAuthNo = T8.TravAuthNo and " + 
                    dateClause + whereClause + " and T7.authstatus in ('A','P','D','E') ",
                KeyWhereClause = "T1.reckey = T7.reckey and T1.agency = T7.agency and T7.TravAuthNo = T8.TravAuthNo and ",
                FieldList = "T8.authrzrNbr, T8.auth1Email, T7.authStatus,T1.Reckey "
            };

            script.WhereClause = _whereClauseWithAdvanceParamsHandler.GetWhereClauseWithAdvancedParametersAndUpdateSqlScriptIfNeeded(script, buildWhere, true);
            return script;
        }

        public static SqlScript GetSqlReasonCodesAir(BuildWhere buildWhere, string whereClause, string dateClause)
        {
            var script = new SqlScript
            {
                FromClause = "ibtrips T1, ibTravAuth T7 ",
                WhereClause = @"T1.reckey = T7.reckey and T1.agency = T7.agency and " +
                    dateClause + whereClause + " and outpolcods != ''",
                KeyWhereClause = "T1.reckey = T7.reckey and T1.agency = T7.agency and ",
                FieldList = "T1.acct, outpolcods, airchg as cost "
            };

            script.WhereClause = _whereClauseWithAdvanceParamsHandler.GetWhereClauseWithAdvancedParametersAndUpdateSqlScriptIfNeeded(script, buildWhere, true);
            return script;
        }

        public static SqlScript GetSqlReasonCodesCar(BuildWhere buildWhere, string whereClause, string dateClause)
        {
            var script = new SqlScript
            {
                FromClause = "ibtrips T1, ibCar T4, ibTravAuth T7 ",
                WhereClause = @"T1.reckey = T4.reckey and T1.agency = T4.agency and " + 
                    "T1.reckey = T7.reckey and T1.agency = T7.agency and " +
                    dateClause + whereClause + " and outpolcods != ''",
                KeyWhereClause = "T1.reckey = T4.reckey and T1.agency = T4.agency and " +
                    "T1.reckey = T7.reckey and T1.agency = T7.agency and ",
                FieldList = "T1.acct, outpolcods, abookrat*days as cost "
            };

            script.WhereClause = _whereClauseWithAdvanceParamsHandler.GetWhereClauseWithAdvancedParametersAndUpdateSqlScriptIfNeeded(script, buildWhere, true);
            return script;
        }

        public static SqlScript GetSqlReasonCodesHotel(BuildWhere buildWhere, string whereClause, string dateClause)
        {
            var script = new SqlScript
            {
                FromClause = "ibtrips T1, ibHotel T5, ibTravAuth T7 ",
                WhereClause = @"T1.reckey = T5.reckey and T1.agency = T5.agency and " + 
                    "T1.reckey = T7.reckey and T1.agency = T7.agency and " +
                    dateClause + whereClause + " and outpolcods != ''",
                KeyWhereClause = "T1.reckey = T5.reckey and T1.agency = T5.agency and " + 
                    "T1.reckey = T7.reckey and T1.agency = T7.agency and ",
                FieldList = "T1.acct, outpolcods, bookrate*nights*rooms as cost "
            };

            script.WhereClause = _whereClauseWithAdvanceParamsHandler.GetWhereClauseWithAdvancedParametersAndUpdateSqlScriptIfNeeded(script, buildWhere, true);
            return script;
        }
    }
}
