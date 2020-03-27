using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using com.ciswired.libraries.CISLogger;
using Domain.Exceptions;
using iBank.Server.Utilities;

namespace iBank.Services.Implementation.Shared.AdvancedClause
{
    public class MissingFieldParamsHandler
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        // example 1: "(ditcode = 'd')"
        // example 2: "(seat = '4d' and fltno = '1113')"
        public string ReplaceMissingFieldsAndGetUpdatedAdvanceWhereClause(string advancedWhereClause, List<string> missingFields)
        {
            
            try
            {
                if (!missingFields.Any()) return advancedWhereClause;

                var startsWithParenthesis = advancedWhereClause.StartsWith("(");
                var endsWithParenthesis = advancedWhereClause.EndsWith(")");

                if (!startsWithParenthesis || !endsWithParenthesis) throw new MissingFieldParamsHandlerException($"advancedWhereClause missing opening or closing parentheses");

                var whereClause = advancedWhereClause.TrimStart('(');
                whereClause = whereClause.TrimEnd(')');
                var whereClauseComponentList = whereClause.SplitAndRemoveEmptyStrings(' ').ToList();

                var newAdvanceWhereClause = string.Empty;
                // each param has a field name, an operator, and a value; 
                if (whereClauseComponentList.Count() >= 3) 
                {
                    newAdvanceWhereClause = GetNewAdvanceParamsWhereClause(whereClauseComponentList, missingFields);
                }
                else
                {
                    // soemthing is not right
                    LOG.Error($"advancedWhereClause: {advancedWhereClause} - whereClause {whereClause} - newAdvanceWhereClause {newAdvanceWhereClause}");
                    throw new MissingFieldParamsHandlerException($"whereClauseComponentList.Count() is less than 3 - whereClauseComponentList.Count() : {whereClauseComponentList.Count()}");
                }

                newAdvanceWhereClause = $"({newAdvanceWhereClause})";

                return newAdvanceWhereClause;
            }
            catch (Exception ex)
            {
                LOG.Error($"advancedWhereClause: {advancedWhereClause} ", ex);
                throw new MissingFieldParamsHandlerException($"advancedWhereClause: {advancedWhereClause} ex.Message: {ex.Message}");
            }
        }

        private static string GetNewAdvanceParamsWhereClause(IReadOnlyList<string> whereClauseComponentList, IReadOnlyCollection<string> missingFields)
        {
            

            var newAdvanceWhereClause = string.Empty;
            var arrayIndex = 0;

            // each param has a field name, an operator, and a value; thus, divided by 3
            var numberOfParams = whereClauseComponentList.Count() / 3;
            for (var currentParam = 1; currentParam <= numberOfParams; currentParam++)
            {
                var paramFieldName = whereClauseComponentList[arrayIndex];
                if (missingFields.Contains(paramFieldName, StringComparer.CurrentCultureIgnoreCase))
                {
                    // replace missing fields with an operation that returns false; since the param does not exists it makes sense to return false or nor found.
                    newAdvanceWhereClause += $"1 = 0";
                    arrayIndex += 3;
                    // if this is not the last param include the operator that connects the two parameters ie: (seat = '4d' and fltno = '1113')
                    if (currentParam < numberOfParams) newAdvanceWhereClause += $" {whereClauseComponentList[arrayIndex++]}";
                }
                else
                {
                    newAdvanceWhereClause += $" {whereClauseComponentList[arrayIndex]} {whereClauseComponentList[++arrayIndex]} {whereClauseComponentList[++arrayIndex]}";
                    // if this is not the last param include the operator that connects the two parameters ie: (seat = '4d' and fltno = '1113')
                    if (currentParam < numberOfParams)
                    {
                        newAdvanceWhereClause += $" {whereClauseComponentList[++arrayIndex]}";
                        arrayIndex++;
                    }
                }

                LOG.Info($"newAdvanceWhereClause: {newAdvanceWhereClause} ");
            }
            return newAdvanceWhereClause;
        }
    }
}
