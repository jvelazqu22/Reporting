using com.ciswired.libraries.CISLogger;
using System;
using System.Reflection;

namespace iBank.Server.Utilities.Classes
{
    public static class iBankDateFormattedString
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Parses an iBank date string. Acceptable formats are "DT:YYYY,m,d", which will parse as DateTime(year, month, day),
        /// or "DT:YYYY,m,d T:h:m", which will parse as DateTime(year, month, day, hour, minute, 0).
        /// If <paramref name="setToEndOfDay"/> is true then either format will parse as DateTime(year, month, day, 23, 59, 59).
        /// If any errors are encountered in parsing then null will be returned.
        /// </summary>
        /// <param name="val">The value.</param>
        /// <param name="setToEndOfDay">if set to <c>true</c> [set to end of day].</param>
        /// <returns></returns>
        public static DateTime? Parse(string val, bool setToEndOfDay = false)
        {
            try
            {
                if (string.IsNullOrEmpty(val?.Trim())) return null;

                val = val.Trim();
                if (!ContainsDateComponent(val))
                {
                    throw new ArgumentException($"Malformed date formatting. Expected format of DT:YYYY,m,d. Actual format is [{val}].");
                }

                return ContainsTimeComponent(val)
                    ? ParseDateTime(val)
                    : ParseDate(val, setToEndOfDay);
            }
            catch (Exception e)
            {
                //it feels horribly wrong to return null instead of an exception, but that legacy logic is so ingrained in the code it must be done
                LOG.Warn(e.Message, e);
                return null;
            }
        }

        /// <summary>
        /// Converts a data parameter, which looks like "DT:yyyy,mm,dd", into a nullable date. This completely ignores any time portion
        /// </summary>
        /// <param name="val"></param>
        /// <param name="setToEndOfDay"></param>
        /// <returns></returns>
        private static DateTime? ParseDate(string val, bool setToEndOfDay)
        {
            var dateTemp = SplitOutDate(val);
            var date = SplitOutDateComponents(dateTemp);

            return setToEndOfDay 
                ? new DateTime(date.Year, date.Month, date.Day, 23, 59, 59) 
                : new DateTime(date.Year, date.Month, date.Day);
        }

        /// <summary>
        /// Parses out the date and time from the iBank formatted string. Expects a format of DT:YYYY,m,d T:h:m [Example - 4/1/2016 04:00:00 would be "DT:2016,4,1 T:4:0"]
        /// Returns null if date OR time delimiters are not present.
        /// </summary>
        /// <param name="val">The value.</param>
        /// <returns></returns>
        private static DateTime? ParseDateTime(string val)
        {
            var dateTemp = SplitOutDate(val);
            var timeTemp = SplitOutTime(val);
            var date = SplitOutDateComponents(dateTemp);
            var time = SplitOutTimeComponents(timeTemp);

            return new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, 0);
        }

        private static bool ContainsDateComponent(string val)
        {
            return val.Length >= 3 || val.Substring(0, 3) == "DT:";
        }

        private static bool ContainsTimeComponent(string val)
        {
            return val.Contains(" T:");
        }

        private static string SplitOutDate(string val)
        {
            var temp = val.Split(' ');
            return StripOutDateDelimiter(temp[0]);
        }

        private static string SplitOutTime(string val)
        {
            var temp = val.Split(' ');
            return StripOutTimeDelimiter(temp[1]);
        }

        private static string StripOutDateDelimiter(string s)
        {
            return s.Replace("DT:", "");
        }

        private static string StripOutTimeDelimiter(string s)
        {
            return s.Replace("T:", "");
        }

        private static Date SplitOutDateComponents(string s)
        {
            var dateComponents = s.Split(',');
            if (dateComponents.Length != 3)
            {
                throw new ArgumentException($"Malformed date component. Should have format of YYYY:m:d. Actual format is [{s}].");
            }

            if (dateComponents[0].Length == 2) dateComponents[0] = $"20{dateComponents[0]}";

            try
            {
                var year = int.Parse(dateComponents[0]);
                var month = int.Parse(dateComponents[1]);
                var day = int.Parse(dateComponents[2]);
                return new Date(year, month, day);
            }
            catch (Exception e)
            {
                throw new ArgumentException($"Unable to parse given date component of [{s}]", e);
            }
        }

        private static Time SplitOutTimeComponents(string s)
        {
            var timeComponents = s.Split(':');
            if (timeComponents.Length != 2)
            {
                throw new ArgumentException($"Malformed time component. Should have format of H:m. Actual format is [{s}]");
            }

            try
            {
                var hour = int.Parse(timeComponents[0]);
                var minute = int.Parse(timeComponents[1]);
                return new Time(hour, minute);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Unable to parse given time component of [{s}]", ex);
            }
        }

        private struct Date
        {
            public int Year { get; set; }

            public int Month { get; set; }

            public int Day { get; set; }

            public Date(int year, int month, int day)
            {
                Year = year;
                Month = month;
                Day = day;
            }
        }

        private struct Time
        {
            public int Hour { get; set; }

            public int Minute { get; set; }

            public Time(int hour, int minute)
            {
                Hour = hour;
                Minute = minute;
            }
        }
    }

    
}
