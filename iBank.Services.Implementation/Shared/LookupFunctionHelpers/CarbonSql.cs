using Domain.Models;
using iBank.Services.Implementation.Shared.AdvancedClause;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;

namespace iBank.Services.Implementation.Shared.LookupFunctionHelpers
{
    public static class CarbonSql
    {
        private static readonly WhereClauseWithAdvanceParamsHandler whereClauseWithAdvanceParamsHandler = new WhereClauseWithAdvanceParamsHandler();

        public static string CreateAirCarbonSql(bool preview, bool hasUdid, BuildWhere buildWhere)
        {
            var sql = new SqlScript();

            sql.FieldList = "T1.reckey, connect, origin, destinat, airline, class, classcat, mode, convert(int, miles) as miles, ditcode, " +
                (preview ? " convert(int, 1) as plusmin " : " convert(int,T2.rplusmin) as plusmin ");

            if (hasUdid)
            {
                sql.FromClause = preview
                    ? "ibtrips T1, iblegs T2, ibudids T3"
                    : "hibtrips T1, hiblegs T2, hibudids T3";

                sql.KeyWhereClause = "T1.reckey = t2.reckey and T1.reckey = T3.reckey and ";
                sql.WhereClause = sql.KeyWhereClause + buildWhere.WhereClauseFull;
            }
            else
            {
                sql.FromClause = preview
                    ? "ibtrips T1, iblegs T2"
                    : "hibtrips T1, hiblegs T2";

                sql.KeyWhereClause = "T1.reckey = t2.reckey and ";
                sql.WhereClause = sql.KeyWhereClause + buildWhere.WhereClauseFull;
            }
            sql.OrderBy = "order by T1.reckey, seqno";
  
            sql.WhereClause = whereClauseWithAdvanceParamsHandler.GetWhereClauseWithAdvancedParametersAndUpdateSqlScriptIfNeeded(sql, buildWhere, true);

            return SqlProcessor.ProcessSql(sql.FieldList, false, sql.FromClause, sql.WhereClause, sql.OrderBy, buildWhere.ReportGlobals);

        }
               
        //ibCar hibCars use liason T4
        public static string CreateCarCarbonSql(bool isReservation, bool hasUdid, BuildWhere buildWhere)
        {        
            var sql = new SqlScript();

            sql.FieldList = "T1.reckey, T4.days,T4.cartype, " +
                           (isReservation ? " convert(int, 1) as cplusmin " : " convert(int,T4.cplusmin) as cplusmin ");

            if (hasUdid)
            {
                sql.FromClause = isReservation
                    ? "ibtrips T1, ibcar T4, ibudids T3"
                    : "hibtrips T1, hibcars T4, hibudids T3";

                sql.KeyWhereClause = "T1.reckey = T4.reckey and T1.reckey = T3.reckey and ";
                sql.WhereClause = sql.KeyWhereClause + buildWhere.WhereClauseFull;
            }
            else
            {
                sql.FromClause = isReservation
                    ? "ibtrips T1, ibcar T4"
                    : "hibtrips T1, hibcars T4";

                sql.KeyWhereClause = "T1.reckey =T4.reckey and ";
                sql.WhereClause = sql.KeyWhereClause + buildWhere.WhereClauseFull;
            }
            sql.OrderBy = "order by T1.reckey, seqno";
            
            sql.WhereClause = whereClauseWithAdvanceParamsHandler.GetWhereClauseWithAdvancedParametersAndUpdateSqlScriptIfNeeded(sql, buildWhere, true);

            return SqlProcessor.ProcessSql(sql.FieldList, false, sql.FromClause, sql.WhereClause, sql.OrderBy, buildWhere.ReportGlobals);
        }

    }
}
