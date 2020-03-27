using System;
using System.Linq;

using Domain.Exceptions;

namespace iBank.Server.Utilities.Helpers
{
    public static class ColumnComparison
    {
        public static bool InListByColumnType(object val, string crit, string colType)
        {
            if (colType != "TEXT" && colType != "NUMERIC") return true; //INLIST only applies to strings and integers

            switch (colType)
            {
                case "TEXT":
                    CheckIfValueIsString(val);
                    var stringVal = ((string)val).Trim();

                    var critArray = crit.Split(',').ToList();
                    foreach(var item in critArray)
                    {
                        if(stringVal.Equals(item.Trim(), StringComparison.OrdinalIgnoreCase)) return true;
                    }
                    return false;
                    //return critArray.Contains(stringVal);

                case "NUMERIC":
                    CheckIfValueIsInt(val);
                    var intVal = (int)val;
                    try
                    {
                        var intList = crit.Split(',').Select(s => Convert.ToInt32(s)).ToList();
                        return intList.Contains(intVal);
                    }
                    catch (FormatException)
                    {
                        return false;
                    }
            }

            return true;
        }

        public static bool EmptyByColumnType(object val, string colType)
        {
            if (!colType.Equals("TEXT")) return true; //This operator can only work on strings. Ignore otherwise. 

            CheckIfValueIsString(val);
            return string.IsNullOrEmpty(((string)val).Trim());
        }

        public static bool EqualByColumnType(object val, string crit, string colType)
        {
            switch (colType.Trim().ToUpper()) 
            {
                case "TEXT":
                    CheckIfValueIsString(val);
                    return ((string)val).Trim().EqualsIgnoreCase(crit);

                case "CURRENCY":
                    CheckIfValueIsDecimal(val);
                    decimal decimalCrit;
                    if (!decimal.TryParse(crit, out decimalCrit)) return false;
                    return (decimal)val == decimalCrit;

                case "NUMERIC":
                    CheckIfValueIsInt(val);
                    int intCrit;
                    if (!int.TryParse(crit, out intCrit)) return false;
                    return (int)val == intCrit;

                case "DATE":
                    CheckIfValueIsDateTime(val);
                    DateTime dateCrit;
                    if (!DateTime.TryParse(crit, out dateCrit)) return false;
                    return ((DateTime)val).Date == dateCrit.Date;

                case "DATETIME":
                    CheckIfValueIsDateTime(val);
                    DateTime dateTimeCrit;
                    if (!DateTime.TryParse(crit, out dateTimeCrit)) return false;
                    return ((DateTime)val).Equals(dateTimeCrit);
            }

            return true;
        }
        
        public static bool GreaterThanByColumnType(object val, string crit, string colType, bool orEqual = false)
        {
            switch (colType.Trim().ToUpper())
            {
                case "TEXT":
                    CheckIfValueIsString(val);
                    var stringVal = ((string)val).Trim();
                    return orEqual ? string.CompareOrdinal(stringVal, crit) >= 0 : string.CompareOrdinal(stringVal, crit) > 0;

                case "CURRENCY":
                    CheckIfValueIsDecimal(val);
                    decimal decCrit;
                    if (!decimal.TryParse(crit, out decCrit)) return false;
                    return orEqual ? (decimal)val >= decCrit : (decimal)val > decCrit;

                case "NUMERIC":
                    CheckIfValueIsInt(val);
                    int intCrit;
                    if (!int.TryParse(crit, out intCrit)) return false;
                    return orEqual ? (int)val >= intCrit : (int)val > intCrit;

                case "DATE":
                    CheckIfValueIsDateTime(val);
                    DateTime dateCrit;
                    if (!DateTime.TryParse(crit, out dateCrit)) return false;
                    return orEqual ? ((DateTime)val).Date >= dateCrit.Date : ((DateTime)val).Date > dateCrit.Date;

                case "DATETIME":
                    CheckIfValueIsDateTime(val);
                    if (!DateTime.TryParse(crit, out dateCrit)) return false;
                    return orEqual ? (DateTime)val >= dateCrit : (DateTime)val > dateCrit;
            }
            return true;
        }

        public static bool LessThanByColumnType(object val, string crit, string colType, bool orEqual = false)
        {
            switch (colType.Trim().ToUpper())
            {
                case "TEXT":
                    CheckIfValueIsString(val);
                    var stringVal = ((string)val).Trim();
                    return orEqual ? string.CompareOrdinal(stringVal, crit) <= 0 : string.CompareOrdinal(stringVal, crit) < 0;

                case "CURRENCY":
                    CheckIfValueIsDecimal(val);
                    decimal decCrit;
                    if (!decimal.TryParse(crit, out decCrit)) return false;
                    return orEqual ? (decimal)val <= decCrit : (decimal)val < decCrit;

                case "NUMERIC":
                    CheckIfValueIsInt(val);
                    int intCrit;
                    if (!int.TryParse(crit, out intCrit)) return false;
                    return orEqual ? (int)val <= intCrit : (int)val < intCrit;

                case "DATE":
                    CheckIfValueIsDateTime(val);
                    DateTime dateCrit;
                    if (!DateTime.TryParse(crit, out dateCrit)) return false;
                    return orEqual ? ((DateTime)val).Date <= dateCrit.Date : ((DateTime)val).Date < dateCrit.Date;

                case "DATETIME":
                    CheckIfValueIsDateTime(val);
                    if (!DateTime.TryParse(crit, out dateCrit)) return false;
                    return orEqual ? (DateTime)val <= dateCrit : (DateTime)val < dateCrit;
            }
            return true;
        }

        public static bool BetweenByColumnType(object val, string crit1, string crit2, string colType, bool notBetween = false)
        {
            switch (colType.Trim().ToUpper())
            {
                case "TEXT":
                    CheckIfValueIsString(val);
                    var stringVal = ((string)val).Trim();
                    return notBetween 
                        ? string.Compare(stringVal, crit1, StringComparison.Ordinal) < 0 || string.Compare(stringVal, crit2, StringComparison.Ordinal) > 0 
                        : string.Compare(stringVal, crit1, StringComparison.Ordinal) >= 0 && string.Compare(stringVal, crit2, StringComparison.Ordinal) <= 0;

                case "CURRENCY":
                    CheckIfValueIsDecimal(val);
                    var decimalVal = (decimal)val;
                    decimal decCrit1;
                    if (!decimal.TryParse(crit1, out decCrit1)) return false;
                    decimal decCrit2;
                    if (!decimal.TryParse(crit2, out decCrit2)) return false;
                    return notBetween 
                        ? decimalVal < decCrit1 || decimalVal > decCrit2 
                        : decimalVal >= decCrit1 && decimalVal <= decCrit2;

                case "NUMERIC":
                    CheckIfValueIsInt(val);
                    var intVal = (int)val;
                    int intCrit1;
                    if (!int.TryParse(crit1, out intCrit1)) return false;
                    int intCrit2;
                    if (!int.TryParse(crit2, out intCrit2)) return false;
                    return notBetween 
                        ? intVal < intCrit1 || intVal > intCrit2 
                        : intVal >= intCrit1 && intVal <= intCrit2;

                case "DATE":
                    CheckIfValueIsDateTime(val);
                    var dateVal = (DateTime)val;
                    DateTime dateCrit1;
                    if (!DateTime.TryParse(crit1, out dateCrit1)) return false;
                    DateTime dateCrit2;
                    if (!DateTime.TryParse(crit2, out dateCrit2)) return false;
                    return notBetween 
                        ? dateVal.Date < dateCrit1.Date || dateVal.Date > dateCrit2.Date 
                        : dateVal.Date >= dateCrit1.Date && dateVal.Date <= dateCrit2.Date;

                case "DATETIME":
                    CheckIfValueIsDateTime(val);
                    var dateTimeVal = (DateTime)val;
                    DateTime dateTimeCrit1;
                    if (!DateTime.TryParse(crit1, out dateTimeCrit1)) return false;
                    DateTime dateTimeCrit2;
                    if (!DateTime.TryParse(crit2, out dateTimeCrit2)) return false;
                    return notBetween 
                        ? dateTimeVal < dateTimeCrit1 || dateTimeVal > dateTimeCrit2
                        : dateTimeVal >= dateTimeCrit1 && dateTimeVal <= dateTimeCrit2;
            }
            return true;
        }

        private static string FormatExceptionMessage(Type expectedType, Type actualType)
        {
            return $"Type Mismatch in Column Comparison: Expected: [{expectedType.Name}] | Actual: [{actualType.Name}]";
        }

        private static void CheckIfValueIsString(object val)
        {
            if (!(val is string)) throw new ColumnComparisonException(FormatExceptionMessage(typeof(string), val.GetType()));
        }

        private static void CheckIfValueIsDecimal(object val)
        {
            if (!(val is decimal)) throw new ColumnComparisonException(FormatExceptionMessage(typeof(decimal), val.GetType()));
        }

        private static void CheckIfValueIsInt(object val)
        {
            if (!(val is int)) throw new ColumnComparisonException(FormatExceptionMessage(typeof(int), val.GetType()));
        }

        private static void CheckIfValueIsDateTime(object val)
        {
            if (!(val is DateTime)) throw new ColumnComparisonException(FormatExceptionMessage(typeof(DateTime), val.GetType()));
        }
    }
}
