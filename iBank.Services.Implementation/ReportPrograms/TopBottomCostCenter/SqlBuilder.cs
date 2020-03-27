using Domain.Models;
using iBank.Services.Implementation.Shared.AdvancedClause;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;

namespace iBank.Services.Implementation.ReportPrograms.TopBottomCostCenter
{
    public static class SqlBuilder
    {
        private static readonly WhereClauseWithAdvanceParamsHandler _whereClauseWithAdvanceParamsHandler = new WhereClauseWithAdvanceParamsHandler();

        public static SqlScript GetSqlNoRouting(bool hasUdid, bool isPreview, BuildWhere buildWhere, string groupColumn)
        {
            //Don't need legs when get Hotel data fix Defect 00174565 - Top/Bottom Cost Center – ZZ:H 1/12/2018
            var script = new SqlScript();
            if (hasUdid)
            {
                if (isPreview)
                {
                    script.FromClause = "ibtrips T1, ibudids T3";
                    script.KeyWhereClause = "T1.reckey = T3.reckey and valcarr not in ('ZZ','$$') and ";
                }
                else
                {
                    script.FromClause = "hibtrips T1, hibudids T3";
                    script.KeyWhereClause = "T1.reckey = T3.reckey and valcarr not in ('ZZ','$$') and ";
                }
            }
            else
            {
                if (isPreview)
                {
                    script.FromClause = "ibtrips T1";
                    script.KeyWhereClause = "valcarr not in ('ZZ','$$') and ";
                }
                else
                {
                    script.FromClause = "hibtrips T1";
                    script.KeyWhereClause = "valcarr not in ('ZZ','$$') and ";
                }
            }

            script.WhereClause = script.KeyWhereClause + buildWhere.WhereClauseFull;
            script.FieldList = "T1.reckey, T1." + groupColumn + " as GrpCol, airchg, offrdchg, " + 
                (isPreview ? "convert(int,1) as plusmin" : "convert(int,plusmin) as plusmin");

            script.WhereClause = _whereClauseWithAdvanceParamsHandler.GetWhereClauseWithAdvancedParametersAndUpdateSqlScriptIfNeeded(script, buildWhere, false);
            return script;
        }

        public static SqlScript GetSqlWithRouting(bool hasUdid, bool isPreview, BuildWhere buildWhere, string groupColumn)
        {
            var script = new SqlScript();
            if (hasUdid)
            {
                script.FromClause = isPreview
                    ? "ibtrips T1, iblegs T2, ibudids T3"
                    : "hibtrips T1, hiblegs T2, hibudids T3";
                script.WhereClause = "T1.reckey = T2.reckey and T1.reckey = T3.reckey and valcarr not in ('ZZ','$$') and " + buildWhere.WhereClauseFull;
                script.KeyWhereClause = "T1.reckey = T2.reckey and T1.reckey = T3.reckey and valcarr not in ('ZZ','$$') and ";
            }
            else
            {
                script.FromClause = isPreview
                   ? "ibtrips T1, iblegs T2"
                   : "hibtrips T1, hiblegs T2";
                script.WhereClause = "T1.reckey = T2.reckey and valcarr not in ('ZZ','$$') and " + buildWhere.WhereClauseFull;
                script.KeyWhereClause = "T1.reckey = T2.reckey and valcarr not in ('ZZ','$$') and ";
            }

            script.FieldList = "T1.reckey, T1." + groupColumn + " as GrpCol, airchg, offrdchg,  " +
                (isPreview ? "convert(int,1) as plusmin" : "convert(int,plusmin) as plusmin");

            script.WhereClause = _whereClauseWithAdvanceParamsHandler.GetWhereClauseWithAdvancedParametersAndUpdateSqlScriptIfNeeded(script, buildWhere, true);
            return script;
        }

        public static SqlScript GetSqlCar(bool hasUdid, bool isPreview, BuildWhere buildWhere, string groupColumn)
        {
            //Don't need legs when get Hotel data fix Defect 00174565 - Top/Bottom Cost Center – ZZ:H 1/12/2018
            var script = new SqlScript();
            if (hasUdid)
            {
                if (isPreview)
                {
                    script.FromClause = "ibtrips T1, ibcar T4, ibudids T3";
                    script.KeyWhereClause = "T1.reckey = T4.reckey and T1.reckey = T3.reckey and ";
                    script.WhereClause = script.KeyWhereClause + buildWhere.WhereClauseFull.Replace("T1.trantype", "T4.CarTranTyp");
                }
                else
                {
                    script.FromClause = "hibtrips T1, hibcars T4, hibudids T3";
                    script.KeyWhereClause = "T1.reckey = T4.reckey and T1.reckey = T3.reckey and ";
                    script.WhereClause = script.KeyWhereClause + buildWhere.WhereClauseFull.Replace("T1.trantype", "T4.CarTranTyp");
                }
            }
            else
            {
                if (isPreview)
                {
                    script.FromClause = "ibtrips T1, ibcar T4";
                    script.KeyWhereClause = "T1.reckey = T4.reckey and ";
                    script.WhereClause = script.KeyWhereClause + buildWhere.WhereClauseFull.Replace("T1.trantype", "T4.CarTranTyp");
                }
                else
                {
                    script.FromClause = "hibtrips T1, hibcars T4";
                    script.KeyWhereClause = "T1.reckey = T4.reckey and ";
                    script.WhereClause = script.KeyWhereClause + buildWhere.WhereClauseFull.Replace("T1.trantype", "T4.CarTranTyp");
                }
            }

            script.FieldList = "T1.reckey, T1." + groupColumn + " as GrpCol, abookrat, convert(int,days) as days, " + 
                (isPreview ? "convert(int,1) as cplusmin" : "convert(int,cplusmin) as cplusmin");

            script.WhereClause = _whereClauseWithAdvanceParamsHandler.GetWhereClauseWithAdvancedParametersAndUpdateSqlScriptIfNeeded(script, buildWhere, false);
            return script;
        }

        public static SqlScript GetSqlHotel(bool hasUdid, bool isPreview, BuildWhere buildWhere, string groupColumn)
        {
            //Don't need legs when get Hotel data fix Defect 00174565 - Top/Bottom Cost Center – ZZ:H 1/12/2018
            var script = new SqlScript();
            if (hasUdid)
            {
                if (isPreview)
                {
                    script.FromClause = "ibtrips T1, ibhotel T5, ibudids T3";
                    script.KeyWhereClause = "T1.reckey = T5.reckey and T1.reckey = T3.reckey and ";
                    script.WhereClause = script.KeyWhereClause + buildWhere.WhereClauseFull.Replace("T1.trantype", "T5.HotTranTyp");
                }
                else
                {
                    script.FromClause = "hibtrips T1, hibhotel T5, hibudids T3";
                    script.KeyWhereClause = "T1.reckey = T5.reckey and T1.reckey = T3.reckey and ";
                    script.WhereClause = script.KeyWhereClause + buildWhere.WhereClauseFull.Replace("T1.trantype", "T5.HotTranTyp");

                }
            }
            else
            {
                if (isPreview)
                {
                    script.FromClause = "ibtrips T1, ibhotel T5";
                    script.KeyWhereClause = "T1.reckey = T5.reckey and ";
                    script.WhereClause = script.KeyWhereClause + buildWhere.WhereClauseFull.Replace("T1.trantype", "T5.HotTranTyp");
                }
                else
                {
                    script.FromClause = "hibtrips T1, hibhotel T5";
                    script.KeyWhereClause = "T1.reckey = T5.reckey and ";
                    script.WhereClause = script.KeyWhereClause + buildWhere.WhereClauseFull.Replace("T1.trantype", "T5.HotTranTyp");
                }
            }

            script.FieldList = "T1.reckey, T1." + groupColumn + " as GrpCol, bookrate,convert(int,nights) as  nights,convert(int,rooms) as  rooms, " +
                (isPreview ? "convert(int,1) as hplusmin" : "convert(int,hplusmin) as hplusmin");

            script.WhereClause = _whereClauseWithAdvanceParamsHandler.GetWhereClauseWithAdvancedParametersAndUpdateSqlScriptIfNeeded(script, buildWhere, false);
            return script;
        }

    }
}
