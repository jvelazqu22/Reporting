using CODE.Framework.Core.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iBank.Services.Implementation.Shared.XMLReport
{
    public static class ValueConverter
    {
        public static string ConvertValue(object value, bool mask, bool includeTime = false)
        {
            var result = string.Empty;
            if (value is decimal)
            {
                result = ((decimal)value).ToString("0.00");
            }
            else if (value is DateTime)
            {
                result = includeTime ? DateTimeFormater.FormatDateTime((DateTime)value, "-", ":") : DateTimeFormater.FormatDate((DateTime)value, "-");
            }
            else if (value is int)
            {
                result = ((int)value).ToString();
            }
            else if (value is string)
            {
                result = (string)value;
            }
            else if (value is bool)
            {
                result = ((bool)value) ? "Y" : "N";
            }
            result = result.Trim();
            if (mask)
            {
                return new string('X', result.Length);
            }
            //* FIX ILLEGAL XML CHARACTERS --   <  >  &  ' "
            //result = result.Replace("&", "&amp;")
            //    .Replace("<", "&lt;")
            //    .Replace(">", "&gt;")
            //    .Replace("'", "&apos;")
            //    .Replace("\"", "&quot;");

            result = result.Replace("\"", " ");//TODO: Double quotes are not converting properly. 

            //* REMOVE HIGH-ORDER ASCII VALUES, AND VERY LOW ORDER VALUES
            return result.Where(character => character.Asc() <= 128 && character.Asc() >= 32)
                .Aggregate(string.Empty, (current, character) => current + character);
        }
    }
}
