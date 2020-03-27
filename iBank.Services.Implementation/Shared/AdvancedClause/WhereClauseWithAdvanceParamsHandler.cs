using System.Collections.Generic;
using Domain.Models;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using iBank.Services.Implementation.Utilities;
using System.Linq;
using System.Reflection;
using AutoMapper;
using com.ciswired.libraries.CISLogger;
using Domain;

namespace iBank.Services.Implementation.Shared.AdvancedClause
{
    public class WhereClauseWithAdvanceParamsHandler
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public WhereClauseWithAdvanceParamsHandler()
        {
            if (!Features.AutoMapperInitializer.IsEnabled())
            {
                Mapper.Initialize(cfg => cfg.CreateMap<SqlScript, SqlScript>());
            }
        }

        public string GetWhereClauseWithAdvancedParametersAndUpdateSqlScriptIfNeeded(SqlScript sql, BuildWhere buildWhere, bool includeAllLegs, string serverFeeAdvancedWhereClause = null)
        {
            if (!buildWhere.ReportGlobals.AdvancedParameters.Parameters.Any()) return sql.WhereClause;

            if (!string.IsNullOrWhiteSpace(serverFeeAdvancedWhereClause))
            {
                return $"{sql.WhereClause} and T1.reckey in (select DISTINCT reckey from hibservices nolock where {serverFeeAdvancedWhereClause} and reckey = t1.reckey)";
            }

            var sqlCopy = new SqlScript();
            //Somehow this not always Map the value correctly. see Defect 00171593 - Itinerary Detail Combined Error When Included in Broadcast 
            //change to make a copy manually.
            //Mapper.Map(sql, sqlCopy);
            sqlCopy.FieldList = sql.FieldList;
            sqlCopy.FromClause = sql.FromClause;
            sqlCopy.GroupBy = sql.GroupBy;
            sqlCopy.KeyWhereClause = sql.KeyWhereClause;
            sqlCopy.OrderBy = sql.OrderBy;
            sqlCopy.WhereClause = sql.WhereClause;

            var fromClause = sqlCopy.FromClause;
            var keyWhereClause = sqlCopy.KeyWhereClause;

            var appender = new AdvancedParameterAppender(fromClause, keyWhereClause, sqlCopy.FieldList, sqlCopy.WhereClause);
            appender.AppendMissingTableParamPairs(buildWhere);
            sqlCopy.WhereClause = appender.WhereClause;

            //buildWhere.WhereClauseAdvanced = SqlTransformer.TranslateLegsAndMktSegsColumnNames(buildWhere.WhereClauseAdvanced, sqlCopy.FromClause.Contains("mktsegs"));
            var whereClauseAdvanced = new SqlTransformer().TranslateLegsAndMktSegsColumnNames(buildWhere.WhereClauseAdvanced, sqlCopy.FromClause.Contains("mktsegs"));

            var whereClause = GetConcatenatedWhereClause(whereClauseAdvanced, sqlCopy.WhereClause, appender.FieldsNotFound);

            var mutatedWhereClause = string.Empty;
            if (buildWhere.AdvancedParameterQueryTableRefList.Count != 0)
            {
                  //should be no difference that buildWhere.AdvancedParameterQueryTableRefList.Count != 0 after using AdvancedParameterUseTablePrefix
                    mutatedWhereClause = string.IsNullOrEmpty(sqlCopy.KeyWhereClause)
                    ? $"{appender.KeyWhereClause}{whereClause}"
                    : whereClause.Replace(sqlCopy.KeyWhereClause, appender.KeyWhereClause);                
            }
            else
            {
                mutatedWhereClause = string.IsNullOrEmpty(sqlCopy.KeyWhereClause)
                ? $"{appender.KeyWhereClause}{whereClause}"
                : whereClause.Replace(sqlCopy.KeyWhereClause, appender.KeyWhereClause);
            }
 
            if (includeAllLegs)
            {
                //we want to know what reckeys match up with the advanced parameters
                var reckeySql = SqlProcessor.ProcessSql("DISTINCT t1.reckey", false, appender.FromClause, mutatedWhereClause, "", buildWhere.ReportGlobals);

                //but then we want all the records that belong to those reckeys
                return $"{sqlCopy.KeyWhereClause} T1.reckey in ({reckeySql})";
            }

            //we only want the records that match up with the advanced parameters

            sql.FromClause = appender.FromClause;
            sql.KeyWhereClause = appender.KeyWhereClause;
            sql.WhereClause = mutatedWhereClause;
            sql.FieldList = appender.SelectClause;

            return mutatedWhereClause;
        }

        public string GetWhereClauseWithAdvancedParametersAndUpdateSqlScript(SqlScript sql, BuildWhere buildWhere, bool includeAllLegs)
        {
            buildWhere.WhereClauseAdvanced = new SqlTransformer().TranslateLegsAndMktSegsColumnNames(buildWhere.WhereClauseAdvanced, sql.FromClause.Contains("mktsegs"));
            var whereClause = GetConcatenatedWhereClause(buildWhere.WhereClauseAdvanced, sql.WhereClause, new List<string>());

            if (includeAllLegs)
            {
                if(Features.AdvanceParamsReplaceReckeyInWithAnd.IsEnabled())
                {
                    whereClause = $"{sql.KeyWhereClause} {whereClause}";
                }
                else
                {
                    //we want to know what reckeys match up with the advanced parameters
                    var reckeySql = SqlProcessor.ProcessSql("DISTINCT t1.reckey", false, sql.FromClause, whereClause, "", buildWhere.ReportGlobals);
                    //but then we want all the records that belong to those reckeys
                    whereClause = $"{sql.KeyWhereClause} T1.reckey in ({reckeySql})";
                }
            }

            return whereClause;

        }

        private bool IsAdvancedWhereClauseAlreadyAdded(string whereClause, string advancedWhereClause)
        {
            return whereClause.ToUpper().Contains(advancedWhereClause.ToUpper());
        }

        private string GetConcatenatedWhereClause(string advancedWhereClause, string whereClause, List<string> missingFields)
        {
            if (string.IsNullOrEmpty(advancedWhereClause)) return whereClause;

            if (IsAdvancedWhereClauseAlreadyAdded(whereClause, advancedWhereClause)) return whereClause;

            advancedWhereClause = new MissingFieldParamsHandler().ReplaceMissingFieldsAndGetUpdatedAdvanceWhereClause(advancedWhereClause, missingFields);

            return $"{whereClause} and {advancedWhereClause}";
        }

    }
}
