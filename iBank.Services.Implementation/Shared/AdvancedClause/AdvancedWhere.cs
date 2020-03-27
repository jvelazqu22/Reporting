using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Domain.Helper;

using iBank.Server.Utilities;
using iBank.Server.Utilities.Helpers;

namespace iBank.Services.Implementation.Shared.AdvancedClause
{
    public static class AdvancedWhere<T>
    {
        public static bool ApplyAdvancedWhere<TU>(List<TU> data, AdvancedParameters advParameters)
        {
            if (data.Count == 0) return false;
            var tType = data[0].GetType();
            var tProperties = new List<PropertyInfo>(tType.GetProperties());

            switch (advParameters.AndOr)
            {
                case AndOr.And:
                    ApplyAsAnd(data, advParameters, tProperties);
                    break;
                case AndOr.Or:
                    ApplyAsOr(data, advParameters, tProperties);
                    break;
            }

            return true;
        }

        private static void ApplyAsOr<TU>(List<TU> data, AdvancedParameters advParameters, List<PropertyInfo> tProperties)
        {
            var rowsToRemove = new List<TU>();

            foreach (var row in data)
            {
                var keepRow = false;
                foreach (var parm in advParameters.Parameters)
                {
                    parm.Value1 = parm.Value1.Replace("'", string.Empty).Trim();
                    parm.Value2 = parm.Value2.Replace("'", string.Empty).Trim();
                    if (string.IsNullOrEmpty(parm.Value1)) continue; //if no value to compare with, skip. 
                    var prop = GetProperty(parm.AdvancedFieldName, tProperties);
                    if (prop == null)
                    {
                        continue; //If the field is not part of this class, ignore. 
                    }
                    var val = prop.GetValue(row);



                    switch (parm.Operator)
                    {
                        case Operator.Equal:
                            keepRow = keepRow || ColumnComparison.EqualByColumnType(val, parm.Value1, parm.Type);
                            break;
                        case Operator.GreaterThan:
                            keepRow = keepRow || ColumnComparison.GreaterThanByColumnType(val, parm.Value1, parm.Type);
                            break;
                        case Operator.GreaterOrEqual:
                            keepRow = keepRow || ColumnComparison.GreaterThanByColumnType(val, parm.Value1, parm.Type, true);
                            break;
                        case Operator.Between:
                            keepRow = keepRow || ColumnComparison.BetweenByColumnType(val, parm.Value1, parm.Value2, parm.Type);
                            break;
                        case Operator.Empty:
                            keepRow = keepRow || ColumnComparison.EmptyByColumnType(val, parm.Type);
                            break;
                        case Operator.InList:
                            keepRow = keepRow || ColumnComparison.InListByColumnType(val, parm.Value1, parm.Type);
                            break;
                        case Operator.NotBetween:
                            keepRow = keepRow || !ColumnComparison.BetweenByColumnType(val, parm.Value1, parm.Value2, parm.Type, true);
                            break;
                        case Operator.NotInList:
                            keepRow = keepRow || !ColumnComparison.InListByColumnType(val, parm.Value1, parm.Type);
                            break;
                        case Operator.NotEmpty:
                            keepRow = keepRow || !ColumnComparison.EmptyByColumnType(val, parm.Type);
                            break;
                        case Operator.Lessthan:
                            keepRow = keepRow || ColumnComparison.LessThanByColumnType(val, parm.Value1, parm.Type);
                            break;
                        case Operator.LessThanOrEqual:
                            keepRow = keepRow || ColumnComparison.LessThanByColumnType(val, parm.Value1, parm.Type, true);
                            break;
                        case Operator.NotEqual:
                            keepRow = keepRow || !ColumnComparison.EqualByColumnType(val, parm.Value1, parm.Type);
                            break;
                        case Operator.Like:
                            //keepRow = stringVal.Contains(parm.Value1); //TODO: Probably not sufficient. Can't find a crit that uses this. 
                            break;
                        case Operator.NotLike:
                            //keepRow = !stringVal.Contains(parm.Value1); //TODO: Probably not sufficient. Can't find a crit that uses this. 
                            break;
                    }
                }
                if (!keepRow)
                {
                    rowsToRemove.Add(row);
                }
            }

            foreach (var row in rowsToRemove)
            {
                data.Remove(row);
            }
        }

        private static void ApplyAsAnd<TU>(List<TU> data, AdvancedParameters advParameters, List<PropertyInfo> tProperties)
        {
            var rowsToRemove = new List<TU>();
            foreach (var advancedParam in advParameters.Parameters)
            {
                advancedParam.Value1 = advancedParam.Value1.Replace("'", string.Empty).Trim();
                advancedParam.Value2 = advancedParam.Value2.Replace("'", string.Empty).Trim();
                if (string.IsNullOrEmpty(advancedParam.Value1)) continue; //if no value to compare with, skip. 

                var prop = GetProperty(advancedParam.AdvancedFieldName, tProperties);
                if (prop == null) continue; //If the field is not part of this class, ignore. 

                foreach (var row in data)
                {
                    var val = prop.GetValue(row);

                    var keepRow = true;

                    switch (advancedParam.Operator)
                    {
                        case Operator.Equal:
                            keepRow = ColumnComparison.EqualByColumnType(val, advancedParam.Value1, advancedParam.Type);
                            break;
                        case Operator.GreaterThan:
                            keepRow = ColumnComparison.GreaterThanByColumnType(val, advancedParam.Value1, advancedParam.Type);
                            break;
                        case Operator.GreaterOrEqual:
                            keepRow = ColumnComparison.GreaterThanByColumnType(val, advancedParam.Value1, advancedParam.Type, true);
                            break;
                        case Operator.Between:
                            keepRow = ColumnComparison.BetweenByColumnType(val, advancedParam.Value1, advancedParam.Value2, advancedParam.Type);
                            break;
                        case Operator.Empty:
                            keepRow = ColumnComparison.EmptyByColumnType(val, advancedParam.Type);
                            break;
                        case Operator.InList:
                            keepRow = ColumnComparison.InListByColumnType(val, advancedParam.Value1, advancedParam.Type);
                            break;
                        case Operator.NotBetween:
                            keepRow = !ColumnComparison.BetweenByColumnType(val, advancedParam.Value1, advancedParam.Value2, advancedParam.Type, true);
                            break;
                        case Operator.NotInList:
                            keepRow = !ColumnComparison.InListByColumnType(val, advancedParam.Value1, advancedParam.Type);
                            break;
                        case Operator.NotEmpty:
                            keepRow = !ColumnComparison.EmptyByColumnType(val, advancedParam.Type);
                            break;
                        case Operator.Lessthan:
                            keepRow = ColumnComparison.LessThanByColumnType(val, advancedParam.Value1, advancedParam.Type);
                            break;
                        case Operator.LessThanOrEqual:
                            keepRow = ColumnComparison.LessThanByColumnType(val, advancedParam.Value1, advancedParam.Type, true);
                            break;
                        case Operator.NotEqual:
                            keepRow = !ColumnComparison.EqualByColumnType(val, advancedParam.Value1, advancedParam.Type);
                            break;
                        case Operator.Like:
                            //keepRow = stringVal.Contains(advancedParam.Value1); //TODO: Probably not sufficient. Can't find a crit that uses this. 
                            break;
                        case Operator.NotLike:
                            //keepRow = !stringVal.Contains(advancedParam.Value1); //TODO: Probably not sufficient. Can't find a crit that uses this. 
                            break;
                    }

                    if (!keepRow)
                    {
                        rowsToRemove.Add(row);
                    }
                }

                foreach (var row in rowsToRemove)
                {
                    data.Remove(row);
                }
            }
        }

        /// <summary>
        /// There may be some special handling for some columns (e.g., DEPDATE may be RDEPDATE). 
        /// This should be handled here. 
        /// </summary>
        /// <param name="fieldName">The name of the advanced criteria field</param>
        /// <param name="tProperties">a list of the properties of the given type</param>
        /// <returns></returns>
        private static PropertyInfo GetProperty(string fieldName, List<PropertyInfo> tProperties)
        {
            //Fix anything that just needs the name changed
            //TODO: We don't really need to rename class to classcode in the report programs. Refactor.
            fieldName = fieldName.EqualsIgnoreCase("CLASS") ? "CLASSCODE" : fieldName;


            var fieldNamesToTry = new List<string> { fieldName };

            if (fieldName.EqualsIgnoreCase("DEPDATE"))
            {
                fieldNamesToTry.Add("RDEPDATE");
                fieldNamesToTry.Add("MSDEPDATE");
            }


            foreach (var field in fieldNamesToTry)
            {
                var prop = tProperties.FirstOrDefault(s => s.Name.EqualsIgnoreCase(field));
                if (prop != null) return prop;
            }

            //can return null if not found.
            return null;
        }
    }
}
