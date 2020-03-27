using CODE.Framework.Core.Utilities;
using Domain;
using Domain.Models;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using iBank.Services.Implementation.Utilities.ClientData;
using System.Collections.Generic;
using System.Linq;
using iBank.Server.Utilities;
using iBank.Services.Implementation.Shared.AdvancedClause;

namespace iBank.Services.Implementation.Shared
{
    public class AdvancedCriteriaHandler<T>
    {
        public List<T> GetData(SqlScript sql, BuildWhere buildWhere, bool addFieldsFromLegsTable, bool includeAllLegs, bool checkForDuplicatesAndRemoveThem = false, 
            bool handleAdvanceParamsAtReportLevelOnly = false)
        {
            //don't want to mutate the original SqlScript object, as this is a Get method
            var sqlCopy = new SqlScript();
            Mapper.Map(sql, sqlCopy);

            if (!string.IsNullOrEmpty(buildWhere.WhereClauseUdid)) sqlCopy.KeyWhereClause += $"{buildWhere.WhereClauseUdid} and ";

            // if handleAdvanceParamsAtReportLevelOnly = true, it means that the WhereClauseWithAdvanceParamsHandler.GetWhereClauseWithAdvancedParametersAndUpdateSqlScriptIfNeeded
            // was called in the report code, so there is no neeed to call this code again
            if (buildWhere.ReportGlobals.AdvancedParameters.Parameters.Any() && !handleAdvanceParamsAtReportLevelOnly)
            {
                sqlCopy.WhereClause = new WhereClauseWithAdvanceParamsHandler().GetWhereClauseWithAdvancedParametersAndUpdateSqlScript(sqlCopy, buildWhere, includeAllLegs);
            }

            var fullSql = SqlProcessor.ProcessSql(sqlCopy.FieldList, addFieldsFromLegsTable, sqlCopy.FromClause, sqlCopy.WhereClause, sqlCopy.OrderBy, buildWhere.ReportGlobals);
            fullSql = AppendGroupBy(fullSql, sqlCopy.GroupBy);
            
            return checkForDuplicatesAndRemoveThem
                ? ClientDataRetrieval.GetOpenQueryData<T>(fullSql, buildWhere.ReportGlobals, buildWhere.Parameters).RemoveDuplicates().ToList()
                : ClientDataRetrieval.GetOpenQueryData<T>(fullSql, buildWhere.ReportGlobals, buildWhere.Parameters).ToList();
            
        }

        private string AppendGroupBy(string sql, string groupBy)
        {
            return string.IsNullOrEmpty(groupBy) 
                ? sql 
                : $"{sql} {groupBy}";
        }
    }
}
