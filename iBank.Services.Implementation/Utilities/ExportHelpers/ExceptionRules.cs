using iBank.Server.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace iBank.Services.Implementation.Utilities.ExportHelpers
{
    public class ExceptionRules
    {
        public bool ApplyExceptionRules<T>(List<PropertyInfo> tProperties, string fieldName, T rowItem)
        {
            try
            {
                return CCReconReportDateTimeExceptionRule<T>(tProperties, fieldName, rowItem);
            }
            catch (Exception) {}
            return false;
        }

        // departure and arrival dates should be empty for service fees
        private bool CCReconReportDateTimeExceptionRule<T>(List<PropertyInfo> tProperties, string fieldName, T rowItem)
        {
            List<string> columns = new List<string>() { "depdate", "arrdate" };
            if (!columns.Contains(fieldName.Trim().ToLower())) return false;

            var propertyInfo = tProperties.FirstOrDefault(s => s.Name.EqualsIgnoreCase("airlndesc"));
            if (propertyInfo == null) return false;

            var cellValue = propertyInfo.GetValue(rowItem, null);
            if (cellValue is string)
            {
                var val = (string)cellValue;
                return val.EqualsIgnoreCase("Service Fee") ? true : false;
            }
            else
            {
                return false;
            }
        }
    }
}
