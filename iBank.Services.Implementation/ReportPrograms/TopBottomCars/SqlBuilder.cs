using Domain.Models;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;

namespace iBank.Services.Implementation.ReportPrograms.TopBottomCars
{
    public static class SqlBuilder
    {
        public static SqlScript GetSql(bool hasUdid, bool isPreview, BuildWhere buildWhere)
        {
            var script = new SqlScript();
            if (hasUdid)
            {
                script.FromClause = isPreview
                    ? "ibtrips T1, ibcar T4, ibudids T3"
                    : "hibtrips T1, hibcars T4, hibudids T3";
                script.WhereClause = "T1.reckey = T4.reckey and T1.reckey = T3.reckey and " + buildWhere.WhereClauseFull;
                script.KeyWhereClause = "T1.reckey = T4.reckey and T1.reckey = T3.reckey and ";
            }
            else
            {
                script.FromClause = isPreview
                   ? "ibtrips T1, ibCar T4"
                   : "hibtrips T1, hibCars T4";
                script.WhereClause = "T1.reckey = T4.reckey and " + buildWhere.WhereClauseFull;
                script.KeyWhereClause = "T1.reckey = T4.reckey and ";
            }

            if (!string.IsNullOrEmpty(buildWhere.WhereClauseCar)) script.KeyWhereClause += buildWhere.WhereClauseCar + " and ";

           script.FieldList = isPreview
                ? "autocity,autostat,company,convert(int,1) as cplusmin,abookrat, days"
                : "autocity,autostat,company,convert(int,cplusmin) as cplusmin,abookrat, days ";
            return script;

        }
    }
}
