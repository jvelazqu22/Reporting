using Domain.Helper;
using System;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers
{
    public static class TwoValuesCompareManager
    {
        public static bool Compare(string value, string crit1, Operator opt, string type, string crit2="")
        {
            switch (opt)
            {
                case Operator.Equal:
                    if (value != crit1) return false;
                    break;
                case Operator.GreaterThan:
                    if (!GreaterThanByColumnType(value, crit1, type)) return false;
                    break;
                case Operator.GreaterOrEqual:
                    if (!GreaterThanByColumnType(value, crit1, type, true)) return false;
                    break;
                case Operator.Between:
                    if (!BetweenByColumnType(value, crit1, crit2, type, false)) return false;
                    break;
                case Operator.Empty:
                    if (type == "NUMERIC") {
                        if (Convert.ToInt32(value) != 0) return false;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(value.Trim())) return false;
                    }
                    break;
                case Operator.InList:
                    if (!crit1.Split(',').Contains(value)) return false;
                    break;
                case Operator.NotBetween:
                    if (BetweenByColumnType(value, crit1, crit2, type, true)) return false;
                    break;
                case Operator.NotInList:
                    if (crit1.Split(',').Contains(value)) return false;
                    break;
                case Operator.NotEmpty:
                    if (type == "NUMERIC")
                    {
                        if (Convert.ToInt32(value) == 0) return false;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(value.Trim())) return false;
                    }
                    break;
                case Operator.Lessthan:
                    if (!LessThanByColumnType(value, crit1, type)) return false;
                    break;
                case Operator.LessThanOrEqual:
                    if (!LessThanByColumnType(value, crit1, type, true)) return false;
                    break;
                case Operator.NotEqual:
                    if (value == crit1) return false;
                    break;
                case Operator.Like:
                    if (!crit1.Contains(value)) return false; 
                    break;
                case Operator.NotLike:
                    if (crit1.Contains(value)) return false; 
                    break;
            }
            return true;
        }



        public static bool GreaterThanByColumnType(string val, string crit, string colType, bool orEqual = false)
        {
            switch (colType.Trim().ToUpper())
            {
                case "TEXT":
                    return orEqual ? string.CompareOrdinal(val, crit) >= 0 : string.CompareOrdinal(val, crit) > 0;
                case "CURRENCY":
                    decimal decVal;
                    var success = decimal.TryParse(val, out decVal);
                    if (!success) return false;
                    decimal decCrit;
                    success = decimal.TryParse(crit, out decCrit);
                    if (!success) return false;
                    return orEqual ? decVal >= decCrit : decVal > decCrit;
                case "NUMERIC":
                    int intVal;
                    success = int.TryParse(val, out intVal);
                    if (!success) return false;
                    int intCrit;
                    success = int.TryParse(crit, out intCrit);
                    if (!success) return false;
                    return orEqual ? intVal >= intCrit : intVal > intCrit;
                case "DATE":
                    DateTime dateVal;
                    success = DateTime.TryParse(val, out dateVal);
                    if (!success) return false;
                    DateTime dateCrit;
                    success = DateTime.TryParse(crit, out dateCrit);
                    if (!success) return false;
                    return orEqual ? dateVal.Date >= dateCrit.Date : dateVal.Date > dateCrit.Date;
                case "DATETIME":
                    success = DateTime.TryParse(val, out dateVal);
                    if (!success) return false;
                    success = DateTime.TryParse(crit, out dateCrit);
                    if (!success) return false;
                    return orEqual ? dateVal >= dateCrit : dateVal > dateCrit;
            }
            return true;
        }

        public static bool LessThanByColumnType(string val, string crit, string colType, bool orEqual = false)
        {
            switch (colType.Trim().ToUpper())
            {
                case "TEXT":
                    return orEqual ? string.CompareOrdinal(val, crit) <= 0 : string.CompareOrdinal(val, crit) < 0;
                case "CURRENCY":
                    decimal decVal;
                    var success = decimal.TryParse(val, out decVal);
                    if (!success) return false;
                    decimal decCrit;
                    success = decimal.TryParse(crit, out decCrit);
                    if (!success) return false;
                    return orEqual ? decVal <= decCrit : decVal < decCrit;
                case "NUMERIC":
                    int intVal;
                    success = int.TryParse(val, out intVal);
                    if (!success) return false;
                    int intCrit;
                    success = int.TryParse(crit, out intCrit);
                    if (!success) return false;
                    return orEqual ? intVal <= intCrit : intVal < intCrit;
                case "DATE":
                    DateTime dateVal;
                    success = DateTime.TryParse(val, out dateVal);
                    if (!success) return false;
                    DateTime dateCrit;
                    success = DateTime.TryParse(crit, out dateCrit);
                    if (!success) return false;
                    return orEqual ? dateVal.Date <= dateCrit.Date : dateVal.Date < dateCrit.Date;
                case "DATETIME":
                    success = DateTime.TryParse(val, out dateVal);
                    if (!success) return false;
                    success = DateTime.TryParse(crit, out dateCrit);
                    if (!success) return false;
                    return orEqual ? dateVal <= dateCrit : dateVal < dateCrit;
            }
            return true;
        }

        public static bool BetweenByColumnType(string val, string crit1, string crit2, string colType, bool notBetween = false)
        {
            switch (colType.Trim().ToUpper())
            {
                case "CURRENCY":
                    decimal decVal;
                    var success = decimal.TryParse(val, out decVal);
                    if (!success) return false;
                    decimal decCrit1;
                    success = decimal.TryParse(crit1, out decCrit1);
                    if (!success) return false;
                    decimal decCrit2;
                    success = decimal.TryParse(crit2, out decCrit2);
                    if (!success) return false;
                    return notBetween ? decVal >= decCrit1 && decVal <= decCrit2 : decVal > decCrit1 && decVal < decCrit2;
                case "NUMERIC":
                    int intVal;
                    success = int.TryParse(val, out intVal);
                    if (!success) return false;
                    int intCrit1;
                    success = int.TryParse(crit1, out intCrit1);
                    if (!success) return false;
                    int intCrit2;
                    success = int.TryParse(crit2, out intCrit2);
                    if (!success) return false;
                    return notBetween ? intVal >= intCrit1 && intVal <= intCrit2 : intVal > intCrit1 && intVal < intCrit2;
                case "DATE":
                    DateTime dateVal;
                    success = DateTime.TryParse(val, out dateVal);
                    if (!success) return false;
                    DateTime dateCrit1;
                    success = DateTime.TryParse(crit1, out dateCrit1);
                    if (!success) return false;
                    DateTime dateCrit2;
                    success = DateTime.TryParse(crit2, out dateCrit2);
                    if (!success) return false;
                    return notBetween ? dateVal.Date >= dateCrit1.Date && dateVal.Date <= dateCrit2.Date : dateVal.Date > dateCrit1.Date && dateVal.Date < dateCrit2.Date;
                case "DATETIME":
                    success = DateTime.TryParse(val, out dateVal);
                    if (!success) return false;
                    success = DateTime.TryParse(crit1, out dateCrit1);
                    if (!success) return false;
                    success = DateTime.TryParse(crit2, out dateCrit2);
                    if (!success) return false;
                    return notBetween ? dateVal >= dateCrit1 && dateVal <= dateCrit2 : dateVal > dateCrit1 && dateVal < dateCrit2;
            }
            return true;
        }

    }
}
