using CODE.Framework.Core.Utilities.Extensions;
using Domain.Exceptions;
using Domain.Helper;
using iBank.Entities.CISMasterEntities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CODE.Framework.Core.Utilities;
using iBank.Server.Utilities.Classes;

namespace iBank.Server.Utilities
{
    public static class ExtensionMethods
    {
        // This method serializes a row into a string and compares them to other rows to remove any duplictes.
        public static IEnumerable<TSource> RemoveDuplicates<TSource>(this IEnumerable<TSource> source)
        {
            var results = new List<TSource>();
            var stringList = new List<string>();

            foreach (var item in source.ToList())
            {
                var itemStr = item.SerializeToXmlString();
                if (stringList.Contains(itemStr)) continue;
                stringList.Add(itemStr);
                results.Add(item);
            }
            return results;
        }

        public static bool IsStringSqlList(this string s)
        {
            if (!s.FirstCharacterEquals('(') || !s.LastCharacterEquals(')')) return false;
            var str = s.RemoveFirstChar();
            str = str.RemoveLastChar();
            var list = str.SplitAndRemoveEmptyStrings(',');
            if (!list.Any()) return false;

            return list.First().Contains(@"'");
        }

        public static bool IsNumericSqlList(this string s)
        {
            if (!s.FirstCharacterEquals('(') || !s.LastCharacterEquals(')')) return false;

            var str = s.RemoveFirstChar();
            str = str.RemoveLastChar();
            var list = str.SplitAndRemoveEmptyStrings(',');
            if (!list.Any()) return false;

            long temp = 0;
            return long.TryParse(list.First(), out temp);
        }

        public static bool IsNumeric(this string s)
        {
            long temp = 0;
            return long.TryParse(s, out temp);
        }

        public static IEnumerable<string> SplitAndRemoveEmptyStrings(this string s, char delimiter)
        {
            return s.Split(delimiter)
                    .Select(x => x.Trim())
                    .Where(x => !string.IsNullOrEmpty(x));
        }

        public static decimal GetRate(this IList<CarbonCalculationRate> rates, string key)
        {
            var rate = rates.FirstOrDefault(x => x.RateName.Equals(key, StringComparison.OrdinalIgnoreCase));

            if (rate == null) throw new CarbonRateNotFoundException($"No carbon rate found for key [{key}]");

            return rate.Rate;
        }

        public static CarbonCalculationHaul GetHaul(this IList<CarbonCalculationHaul> hauls, string haulName)
        {
            var haul = hauls.FirstOrDefault(x => x.HaulType.Equals(haulName, StringComparison.OrdinalIgnoreCase));

            if (haul == null) throw new HaulNotFoundException($"No haul found for key [{haulName}]");

            return haul;
        }

        /// <summary>
        /// When an object has a value of zero, it gets cast as decimal. We know we want it to always be an int. 
        /// </summary>
        /// <param name="val">An object that should contain an integer.</param>
        /// <returns>The value as an int; if it isn't an int, we return zero.</returns>
        public static int GetIntSafe(this object val)
        {
            return val as int? ?? 0;
        }

        /// <summary>
        /// Returns the value of a nullable int, or a zero if there is no value. 
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static int ZeroIfNull(this int? val)
        {
            return val ?? 0;
        }

        //this function is necessary since FoxPro has the concept of a Date vs a DateTime, and legacy code made decisions based on the type
        /// <summary>
        /// Returns if the DateTime value is representing a Date.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool IsDateType(this DateTime val)
        {
            // nextRun 6/23 00:00:00
            return val.TimeOfDay.TotalSeconds == 0;
        }

        /// <summary>
        /// Returns the value of a nullable byte as an int, or returns zero if there is no value. 
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static int ZeroIfNull(this byte? val)
        {
            return val ?? 0;
        }

        /// <summary>
        /// Returns true if the nullable boolean has a value, and that value is 1 (true). Otherwise, returns false. 
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool HasTrueValue(this bool? val)
        {
            return val.HasValue && val.Value;
        }
        
        /// <summary>
        /// returns true if the value is between the start and end values, inclusive.
        /// </summary>
        public static bool IsBetween(this string val, int start, int end)
        {
            int valInt;

            if (!Int32.TryParse(val, out valInt)) return false;

            return (valInt >= start && valInt <= end);

        }
        /// <summary>
        /// returns true if the value is between the start and end values, inclusive.
        /// </summary>
        public static bool IsBetween(this IComparable value, IComparable min, IComparable max)
        {
            return value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0;

        }

        /// <summary>
        /// returns true if the string is NullOrWhiteSpace.
        /// </summary>
        public static bool IsNullOrWhiteSpace(this string s)
        {
            return string.IsNullOrWhiteSpace(s);
        }

        /// <summary>
        /// returns true if the value is between the start and end values, inclusive.
        /// </summary>
        public static bool IsBetween(this int val, int start, int end)
        {
            return (val >= start && val <= end);

        }

        public static bool HasWildCards(this string val)
        {
            //No need for the comparisons on empty strings
            if (string.IsNullOrEmpty(val)) return false;

            return val.Contains("*") || val.Contains("%") || val.Contains("?") || val.Contains("_");
        }

        public static string ReplaceWildcards(this string s)
        {
            if (string.IsNullOrEmpty(s)) return s;

            if (!s.HasWildCards()) return s;

            if (s.IndexOfAny("*?".ToCharArray()) != -1) s = s.Replace("*", "%").Replace("?", "_");

            return s;
        }

        #region Standard string functions

        public static bool ContainsIgnoreCase(this string s, string comparisonString)
        {
            return s?.Trim().IndexOf(comparisonString.Trim(), StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static string Left(this string val, int len)
        {
            if (val == null) return "";
            return val.Length <= len 
                ? val 
                : val.Substring(0, len);
        }

        public static string Right(this string val, int len)
        {
            if (val == null) return "";
            return val.Length <= len 
                ? val 
                : val.Substring(val.Length - len, len);
        }

        public static string RemoveLastChar(this string val)
        {
            if (string.IsNullOrEmpty(val?.Trim())) return "";
            val = val.Trim();
            return val.Left(val.Length - 1);
        }

        public static string RemoveFirstChar(this string val)
        {
            if (string.IsNullOrEmpty(val?.Trim())) return "";
            val = val.Trim();
            return val.Right(val.Length - 1);
        }

        /// <summary>
        /// Returns string with any spaces padding commas removed. 
        /// RemoveUnintentialSpaces( "Foo BAR ,Foo , bar" ) returns "Foo BAR,Foo,bar"
        /// Name is from common comment in original VFP code.
        /// </summary>
        public static string RemoveUnintentionalSpaces(this string val)
        {
            return Regex.Replace(val, @"\s*,\s*", ",");
        }
        #endregion

        public static string ToFriendlyString(this Operator val)
        {
            switch (val)
            {
                case Operator.Equal:
                    return "=";
                case Operator.GreaterThan:
                    return ">";
                case Operator.GreaterOrEqual:
                    return ">=";
                case Operator.Between:
                    return "BETWEEN";
                case Operator.Empty:
                    return "EMPTY";
                case Operator.InList:
                    return "IN";
                case Operator.NotBetween:
                    return "NOT BETWEEN";
                case Operator.NotInList:
                    return "NOT IN";
                case Operator.NotEmpty:
                    return "!EMPTY";
                case Operator.Lessthan:
                    return "<";
                case Operator.LessThanOrEqual:
                    return "<=";
                case Operator.NotEqual:
                    return "<>";
                case Operator.NotLike:
                    return "NOT LIKE";
                case Operator.Like:
                    return "LIKE";
                default:
                    throw new ArgumentOutOfRangeException($"Unexpected operator [{val.ToString()}] encountered.");
            }
        }

        public static Operator ToOperator(this string s)
        {
            switch (s.ToUpper())
            {
                case "=":
                    return Operator.Equal;
                case ">":
                    return Operator.GreaterThan;
                case ">=":
                    return Operator.GreaterOrEqual;
                case "BETWEEN":
                    return Operator.Between;
                case "EMPTY":
                    return Operator.Empty;
                case "IN":
                    return Operator.InList;
                case "NOT BETWEEN":
                    return Operator.NotBetween;
                case "NOT IN":
                    return Operator.NotInList;
                case "!EMPTY":
                    return Operator.NotEmpty;
                case "<":
                    return Operator.Lessthan;
                case "<=":
                    return Operator.LessThanOrEqual;
                case "!=":
                case "<>":
                    return Operator.NotEqual;
                case "LIKE":
                    return Operator.Like;
                case "NOT LIKE":
                    return Operator.NotLike;
                default:
                    throw new ArgumentOutOfRangeException($"Unexpected operator [{s}] encountered.");
            }
        }

        public static string GetMonthAbbreviationFromNumber(this int val, IList<string> monthAbbreviations)
        {
            var defaultMonthAbbreviations = "Jan,Feb,Mar,Apr,May,Jun,Jul,Aug,Sep,Oct,Nov,Dec";
            if (monthAbbreviations == null || monthAbbreviations.Count == 0) monthAbbreviations = defaultMonthAbbreviations.Split(',');

            //months are 1 based, but our array is 0 based, so drop month number by 1 to match up
            val = val - 1;

            return monthAbbreviations[val];
        }


        /// <summary>
        /// Given the name of a month ("August", "July") returns the month number 1 - 12 
        /// </summary>
        /// <param name="val">The name of the month</param>
        /// <returns>The number of the month. </returns>
        public static int MonthNumberFromName(this string val)
        {
            val = val.ToUpper();
            switch (val)
            {
                case "JANUARY":
                    return 1;
                case "FEBRUARY":
                    return 2;
                case "MARCH":
                    return 3;
                case "APRIL":
                    return 4;
                case "MAY":
                    return 5;
                case "JUNE":
                    return 6;
                case "JULY":
                    return 7;
                case "AUGUST":
                    return 8;
                case "SEPTEMBER":
                    return 9;
                case "OCTOBER":
                    return 10;
                case "NOVEMBER":
                    return 11;
                case "DECEMBER":
                    return 12;
                default:
                    return 0; //this will probably cause an error in the calling program


            }
        }
        
        public static string MonthNameFromNumber(this int val)
        {
            
            switch (val)
            {
                case 1:
                    return "January";
                case 2:
                    return "February";
                case 3:
                    return "March";
                case 4:
                    return "April";
                case 5:
                    return "May";
                case 6:
                    return "June";
                case 7:
                    return "July";
                case 8:
                    return "August";
                case 9:
                    return "September";
                case 10:
                    return "October";
                case 11:
                    return "November";
                case 12:
                    return "December";
                default:
                    return string.Empty; //this will probably cause an error in the calling program


            }
        }
        
        public static string TrimCommaDelimitedItem(this string val)
        {
            return val.Trim(" ,".ToCharArray());

        }

        /// <summary>
        /// Returns an "Equals" using Invariant Culture and ignoring case and whitespace. 
        /// </summary>
        /// <param name="val"></param>
        /// <param name="compVal"></param>
        /// <returns></returns>
        public static bool EqualsIgnoreCase(this string val, string compVal)
        {
            if (null == val) return false;
            return val.Trim().Equals(compVal.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// returns a number indicating the day of the week, with Sunday being 1 and Saturday being 7
        /// </summary>
        /// <param name="val"></param>
        /// <param name="startsMonday">if true, Monday is 1, and Sunday is 7</param>
        /// <returns></returns>
        public static int DayOfWeekNumber(this DateTime val, bool startsMonday = false)
        {
            var dow = val.DayOfWeek;
            switch (dow)
            {
                case DayOfWeek.Sunday:
                    return startsMonday ? 7 : 1;
                case DayOfWeek.Monday:
                    return startsMonday ? 1 : 2;
                case DayOfWeek.Tuesday:
                    return startsMonday ? 2 : 3;
                case DayOfWeek.Wednesday:
                    return startsMonday ? 3 : 4;
                case DayOfWeek.Thursday:
                    return startsMonday ? 4 : 5;
                case DayOfWeek.Friday:
                    return startsMonday ? 5 : 6;
                case DayOfWeek.Saturday:
                    return startsMonday ? 6 : 7;
                default:
                    throw new Exception("Not a valid day of week!");
                
            }
        }

        public static string ReplaceFirst(this string text, string search, string replace)
        {
            int pos = text.IndexOf(search, StringComparison.Ordinal);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        public static DateTime MakeDateTime(this DateTime date, string timeToAdd)
        {
            if (string.IsNullOrEmpty(timeToAdd)) return date;

            var year = date.Year;
            var month = date.Month;
            var day = date.Day;

            var hours = timeToAdd.Left(2).TryIntParse(0);
            if (hours > 23 || hours < 1)
                hours = 0;

            var minutes = (timeToAdd.Length < 6) ? timeToAdd.Substring(3,2).TryIntParse(0) : timeToAdd.Substring(4, 2).TryIntParse(0);
            if (minutes > 59 || minutes < 1)
                minutes = 0;

            return new DateTime(year, month, day, hours, minutes, 0);

        }

        public static int GetMonthNumber(this DateTime? date, int defaultMonth)
        {
            return date.HasValue ? date.Value.Month : defaultMonth;
        }

        public static int GetYearNumber(this DateTime? date, int defaultYear)
        {
            return date.HasValue ? date.Value.Year : defaultYear;
        }
        
        /// <summary>
        /// Converts a data parameter, which looks like "DT:yyyy,mm,dd", into a nullable date. 
        /// </summary>
        /// <param name="val"></param>
        /// <param name="endOfDay">Set to True if you want the datetime value to be the end of the day. False will return the beginning of the day. </param>
        /// <returns></returns>
        public static DateTime? ToDateFromiBankFormattedString(this string val, bool endOfDay = false)
        {
            return iBankDateFormattedString.Parse(val, endOfDay);
        }

        public static string FormatMessageWithReportLogKey(this string message, int reportLogKey)
        {
            return $"report log #: [{reportLogKey}] | [{message}]";
        }

        public static string FormatMessageWithErrorNumber(this string message, int errorNumber)
        {
            return $"Error Number #: [{errorNumber}] | [{message}]";
        }
        
        /// <summary>
        /// Truncates a DateTime to the desired TimeSpan precision
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public static DateTime Truncate(this DateTime dateTime, TimeSpan timeSpan)
        {
            return timeSpan == TimeSpan.Zero 
                ? dateTime 
                : dateTime.AddTicks(-(dateTime.Ticks % timeSpan.Ticks));
        }

        public static string ToIBankDateFormat(this DateTime date)
        {
            return $"DT:{date.Year},{date.Month},{date.Day}";
        }

        public static bool IsPriorToOrSameDay(this DateTime beginDate, DateTime endDate)
        {
            return beginDate <= endDate;
        }

        public static string GetValidationErrors(this DbEntityValidationException dbEx)
        {
            var errorMessages = new List<string> { dbEx.Message };
            foreach (var validationErrors in dbEx.EntityValidationErrors)
            {
                foreach (var validationError in validationErrors.ValidationErrors)
                {
                    var exMsg = $"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}";

                    errorMessages.Add(exMsg);
                }
            }

            return string.Join(" | ", errorMessages);
        }

        public static string FormatWholeDateWithAmPm(this DateTime val)
        {
            return val.ToString(@"MM/dd/yyyy hh:mm:ss tt");
        }

        public static bool IsOfflineBroadcast(this string s)
        {
            return s.Length >= 7 && s.Substring(0, 7).EqualsIgnoreCase("sysDR:[");
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        {
            return new HashSet<T>(source);
        }

        public static List<List<T>> ToBatch<T>(this IList<T> source, int itemsInBatch)
        {
            return source.Select((x, i) => new { Index = i, Value = x })
                        .GroupBy(x => x.Index / itemsInBatch)
                        .Select(x => x.Select(v => v.Value).ToList())
                        .ToList();
        }

        public static string Truncate(this string s, int totalCharacters)
        {
            if (s.Length < totalCharacters) return s;

            return s.Substring(0, totalCharacters);
        }

        public static string RemoveTrailingChar(this string s, char charToRemove)
        {
            s = s.Trim();

            if (string.IsNullOrEmpty(s)) return s;

            return s[s.Length - 1] == charToRemove
                ? s.Left(s.Length - 1) 
                : s;
        }

        public static string GetDateMonthNumber(this DateTime dateTime)
        {
            return dateTime.Month.ToString(CultureInfo.InvariantCulture);
        }

        public static string GetDateMonthAbbreviation(this DateTime dateTime)
        {
            return dateTime.ToString("MMM");
        }
        
        public static string GetDateMonthName(this DateTime dateTime)
        {
            return dateTime.Month.ToString("MMMM");
        }
        
        public static string GetDateYear(this DateTime dateTime)
        {
            return dateTime.Year.ToString(CultureInfo.InvariantCulture);
        }

        public static string GetDateQuarter(this DateTime dateTime)
        {
            return ((dateTime.Month + 2) / 3).ToString(CultureInfo.InvariantCulture);
        }

        public static bool FirstCharacterEquals(this string s, char charToMatch)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;

            var sTrimmed = s.Trim();
            return sTrimmed[0].ToString().EqualsIgnoreCase(charToMatch.ToString());
        }

        public static bool LastCharacterEquals(this string s, char charToMatch)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;

            var sTrimmed = s.Trim();
            return sTrimmed[sTrimmed.Length - 1].ToString().EqualsIgnoreCase(charToMatch.ToString());
        }

        /// <summary>
        /// Compares two values in an approximation of a SQL LIKE operation. The value that function is called on
        /// should contain a wildcard. The parameter value comparisonValue should not.
        /// </summary>
        /// <param name="valueWithWildcard"></param>
        /// <param name="comparisonValue"></param>
        /// <returns></returns>
        public static bool Like(this string valueWithWildcard, string comparisonValue)
        {
            if (valueWithWildcard == null || comparisonValue == null) return false;

            comparisonValue = comparisonValue.Trim();
            valueWithWildcard = valueWithWildcard.Trim();

            if (!valueWithWildcard.HasWildCards()) return valueWithWildcard.EqualsIgnoreCase(comparisonValue);
            
            //this is done so we don't have to worry about * and ? as wildcards
            valueWithWildcard = valueWithWildcard.ReplaceWildcards().ToUpper();

            // %foo
            if (valueWithWildcard.FirstCharacterEquals('%')) return comparisonValue.EndsWith(valueWithWildcard.RemoveFirstChar(), StringComparison.OrdinalIgnoreCase);

            // _foo
            if (valueWithWildcard.FirstCharacterEquals('_')) return comparisonValue.RemoveFirstChar().EqualsIgnoreCase(valueWithWildcard.RemoveFirstChar());

            // foo%
            if (valueWithWildcard.LastCharacterEquals('%')) return comparisonValue.StartsWith(valueWithWildcard.RemoveLastChar(), StringComparison.OrdinalIgnoreCase);

            // foo_
            if (valueWithWildcard.LastCharacterEquals('_')) return comparisonValue.RemoveLastChar().EqualsIgnoreCase(valueWithWildcard.RemoveLastChar());
            
            return false;
        }

        /// <summary>
        /// Returns a numerical representation of what fiscal quarter a date is in. 
        /// [Oct - Dec = 1 ; Jan - Mar = 2 ; Apr - Jun = 3 ; Jul - Sep = 4]
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string GetDateFiscalQuarter(this DateTime dateTime)
        {
            if (dateTime.Month.IsBetween(10, 12)) return "1";
            if (dateTime.Month.IsBetween(1, 3)) return "2";
            if (dateTime.Month.IsBetween(4, 6)) return "3";
            if (dateTime.Month.IsBetween(7, 9)) return "4";
            return "0";
        }

        public static string GetDateFiscalYear(this DateTime dateTime)
        {
            //10/1/16-9/30/17 = 2017, 10/1/17-9/30/18=2018, etc
            if (dateTime.Month >=10) return (dateTime.AddYears(1).Year).ToString(CultureInfo.InvariantCulture);
            else return (dateTime.Year).ToString(CultureInfo.InvariantCulture);
        }

        public static string GetDateFirstOfMonth(this DateTime dateTime, string format)
        {
            //First of Month - Invoice Date (3/18/17 would be 3/1/17)
            var date = new DateTime(dateTime.Year, dateTime.Month, 1);
            return Convert.ToDateTime(date).ToString(format);
        }

        public static string NormalizeColumnHeader(this string column)
        {
            var sb = new StringBuilder();
            var str = column.Replace("#", "no");

            var regex = new Regex(@"^[-./_`&'\s]");

            foreach (char c in str)
            {
                if (char.IsLetterOrDigit(c))
                {
                    sb.Append(Char.ToLower(c));
                }
                else if (regex.IsMatch(c.ToString()))
                {
                    sb.Append('_');
                }
            }

            return sb.ToString();
        }

        public static string TitleCaseString(this string value)
        {
            //has to be lower case then title case would work properly.
            if (value == null) return string.Empty;
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.Trim().ToLower());
        }

        public static bool IsReservationReport(this ReportGlobals globals)
        {
            return globals.ParmValueEquals(WhereCriteria.PREPOST, "1");
        }
    }
}
