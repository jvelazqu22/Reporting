using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using com.ciswired.libraries.CISLogger;
using Domain;
using Domain.Constants;
using iBank.Server.Utilities;

namespace iBank.Services.Implementation.Shared.AdvancedClause
{
    public class FieldPrefixer
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public string PerformWherePrefix(string clauseToPrefix, bool isReservationReport, Dictionary<string, string> tablePrefixPairs, 
            char stringSeparator, string stringToReJoinOn)
        {
            return PerformPrefix(clauseToPrefix, IsNoNeedToPrefix, isReservationReport, tablePrefixPairs, stringSeparator, stringToReJoinOn, true);
        }

        public string PerformSelectPrefix(string clauseToPrefix, bool isReservationReport, Dictionary<string, string> tablePrefixPairs, 
            char stringSeparator, string stringToReJoinOn)
        {
            return PerformPrefix(clauseToPrefix, PrefixExistsOnField, isReservationReport, tablePrefixPairs, stringSeparator, stringToReJoinOn, false);
        }

        private bool IsNoNeedToPrefix(string item)
        {
            //added more no need prefix items
            if(Features.FieldPreFix.IsEnabled())
            {
                return item.EqualsIgnoreCase("AND")
                       || item.EqualsIgnoreCase("OR")
                       || item.Equals("=")
                       || item.Equals("<>")
                       || item.Equals(">")
                       || item.Equals("<")
                       || item.Equals("<=")
                       || item.Equals(">=")
                       || item.EqualsIgnoreCase("is")
                       || item.EqualsIgnoreCase("NOT")
                       || item.EqualsIgnoreCase("LIKE")
                       || item.EqualsIgnoreCase("IN")
                       || item.EqualsIgnoreCase("'%VOID%'")
                       || PrefixExistsOnField(item)
                       || item.Contains(@"'") //takes care of words -- ex: field = 'foo'
                       || item.IsStringSqlList() //takes care of words -- ex: field = ('test1','test2','test3')
                       || item.Contains(@"@") //takes care sql parameters -- ex: field = @t1TranType1
                       || item.IsNumeric() //takes care of numbers -- ex: field = 1
                       || item.IsNumericSqlList() //takes care of numbers -- ex: field in (12345)
                       || item.Equals("!=") //field != 1
                       || item.Equals("null") //ex: field is or is not null
                       || item.EqualsIgnoreCase("(select") // and  t1.reckey in (select reckey from hibServices where trandate >= @t1BeginDate and trandate <= @t1EndDate  AND  (T1.acct = @T1acct0) AND T1.agency = 'DEMO')
                       || item.EqualsIgnoreCase("from") // and  t1.reckey in (select reckey from hibServices where trandate >= @t1BeginDate and trandate <= @t1EndDate  AND  (T1.acct = @T1acct0) AND T1.agency = 'DEMO')
                       || item.EqualsIgnoreCase("where"); // and  t1.reckey in (select reckey from hibServices where trandate >= @t1BeginDate and trandate <= @t1EndDate  AND  (T1.acct = @T1acct0) AND T1.agency = 'DEMO')
            }
            else
            {
                return item.EqualsIgnoreCase("AND")
                       || item.EqualsIgnoreCase("OR")
                       || item.Equals("=")
                       || item.Equals("<>")
                       || item.Equals(">")
                       || item.Equals("<")
                       || item.Equals("<=")
                       || item.Equals(">=")
                       || item.EqualsIgnoreCase("is")
                       || item.EqualsIgnoreCase("NOT")
                       || item.EqualsIgnoreCase("LIKE")
                       || item.EqualsIgnoreCase("IN")
                       || item.EqualsIgnoreCase("'%VOID%'")
                       || PrefixExistsOnField(item)
                       || item.Contains(@"'") //takes care of words -- ex: field = 'foo'
                       || item.IsStringSqlList() //takes care of words -- ex: field = ('test1','test2','test3')
                       || item.Contains(@"@") //takes care sql parameters -- ex: field = @t1TranType1
                       || item.IsNumeric() //takes care of numbers -- ex: field = 1
                       || item.IsNumericSqlList() //takes care of numbers -- ex: field in (12345)
                       || item.Equals("!=") //field != 1
                       || item.Equals("null"); //ex: field is or is not null
            }
        }

        private bool PrefixExistsOnField(string val)
        {
            //a period in the clause would indicate the existence of a prefix - ie, T1.foo
            if (val.Contains(".")) return true;
            if (val.Contains("'I' as ")) return true;
            if (val.Contains("\'A\' as ")) return true;

            return false;
        }

        public string PerformPrefix(string clauseToPrefix, Func<string, bool> noNeedToAddSelectOrWherePrefix, bool isReservationReport,
            Dictionary<string, string> tablePrefixPairs, char stringSeparator, string stringToReJoinOn, bool performWherePrefix)
        {
            var prefixedItems = new List<string>();
            var fieldsLookup = new EntityFieldsLookup();
            var commaSeparatedSelectFields = clauseToPrefix.SplitAndRemoveEmptyStrings(stringSeparator);
            var lastItem = string.Empty;
            var appendLastItem = false;

            foreach (var item in commaSeparatedSelectFields)
            {
                if (appendLastItem)
                {
                    // This code handles cases like this one: convert(int,plusmin) as plusmin
                    prefixedItems.Add($"{lastItem},{item}");
                    appendLastItem = false;
                    continue;
                }
                if (noNeedToAddSelectOrWherePrefix(item))
                {
                    prefixedItems.Add(item);
                    continue;
                }
                if (IsFieldInSpecialCaseList(item))
                {
                    // This code handles cases like this one: convert(int,plusmin) as plusmin or cast(ccnumber2 as nvarchar(max)) ccnumber2
                    lastItem = item;
                    appendLastItem = true;
                    continue;
                }
                if (performWherePrefix)
                {
                    // At some point we may need to handle a case where there is a "select from where" withint the where statement like this one:
                    // "T1.reckey = t2.reckey and T1.reckey=TD.reckey and valcarr != 'ZZ' and valcarr != '$$' and airline != 'ZZ' and airline != '$$'  
                    // and  t1.reckey in (select reckey from hibServices where trandate >= @t1BeginDate and trandate <= @t1EndDate  AND  (T1.acct = @T1acct0) AND T1.agency = 'DEMO')"
                    // For reference look up defect 9629: Defect 442562 - iBank - .NET Service Fee User Defined Reporting - Report will error when using the Service Fee Level “Credit Card #” filter
                }

                var prefixedValue = GetPrefixedValue(item, isReservationReport, tablePrefixPairs, fieldsLookup);
                prefixedItems.Add(prefixedValue);
            }

            return string.Join(stringToReJoinOn, prefixedItems);
        }

        public static bool IsFieldInSpecialCaseList(string selectFieldName)
        {
            foreach (var item in FieldList.SqlSelectSpecialCasesList)
            {
                if (selectFieldName.ContainsIgnoreCase(item)) return true;
            }

            return false;
        }

        private string GetPrefixedValue(string field, bool isReservationReport, Dictionary<string, string> tablePrefixPairs, EntityFieldsLookup lookup)
        {
            var isParentheticalField = field.StartsWith("(");

            //if it starts with a parentheses (ex: (field = ...) we need to strip out the parentheses to make it a real field name)
            if (isParentheticalField) field = field.Substring(1);

            var table = "";
            var prefix = "";
            var isLiasonField = field.IndexOf(" as ") > 0;
            if (isLiasonField)
            {
                table = lookup.GetTableThatFieldBelongsTo(field.Substring(0, field.IndexOf(" as ")).Trim(), isReservationReport, tablePrefixPairs.Keys.ToList());
                prefix = GetTablePrefix(table, tablePrefixPairs);
            }
            else
            {
                table = lookup.GetTableThatFieldBelongsTo(field, isReservationReport, tablePrefixPairs.Keys.ToList());
                prefix = GetTablePrefix(table, tablePrefixPairs);
            }

            var prefixedValue = string.Empty;
            if (Features.FieldPreFix.IsEnabled())
            {
                prefixedValue = prefix.Equals(string.Empty) ? field : $"{prefix}.{field}";
            }
            else
            {
                prefixedValue = $"{prefix}.{field}";
            }

            return isParentheticalField
                ? $"({prefixedValue}"
                : prefixedValue;
        }

        private string GetTablePrefix(string table, Dictionary<string, string> tablePrefixPairs)
        {
            if (!tablePrefixPairs.TryGetValue(table, out var prefix))
            {
                if (Features.FieldPreFix.IsEnabled())
                {
                    LOG.Error($"Failed to retrieve the prefix for table [{table}], which did not exist in the mapping created via the from clause.");
                    return string.Empty;
                }
                else
                {
                    throw new KeyNotFoundException($"Failed to retrieve the prefix for table [{table}], which did not exist in the mapping created via the from clause.");
                }
            }

            return prefix;
        }
    }
}
