using CODE.Framework.Core.Utilities.Extensions;
using iBank.Server.Utilities;
using System;

namespace iBank.Services.Implementation.Shared.XMLReport
{
    public static class DateTimeFormater
    {
        public static string FormatDateTime(DateTime date, string dateDelim = "", string timeDelim = "")
        {
            var yr = date.Year == 1 ? "1900" : date.Year.ToString().PadLeft(4, '0');
            var mo = date.Month.ToString().PadLeft(2, '0');
            var da = date.Day.ToString().PadLeft(2, '0');
            var hr = date.Hour.ToString().PadLeft(2, '0');
            var mi = date.Minute.ToString().PadLeft(2, '0');
            var se = date.Second.ToString().PadLeft(2, '0');
            return yr + dateDelim + mo + dateDelim + da + "T" + hr + timeDelim + mi + timeDelim + se;
        }

        public static string FormatDate(DateTime? date, string dateDelim = "")
        {
            var yr = date.GetValueOrDefault().Year == 1 ? "1900" : date.GetValueOrDefault().Year.ToString().PadLeft(4, '0');
            var mo = date.GetValueOrDefault().Month.ToString().PadLeft(2, '0');
            var da = date.GetValueOrDefault().Day.ToString().PadLeft(2, '0');

            return yr + dateDelim + mo + dateDelim + da;
        }

        /// <summary>
        /// THE EXPECTED TIME FORMAT IS  HH:MM in 24 hr military time.
        /// WE MAY GET A TIME VALUE AS  0HHMM  or just HHMM - still in 24 hr time format.
        /// THIS FUNCTION OUTPUTS THE VALUE IN HH:MM format.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string FormatTime(string time)
        {
            time = time.Trim();
            int hr;
            int min;

            if (!time.Contains(":"))
            {
                time = time.PadLeft(4, '0');
                hr = time.Left(2).TryIntParse(0);
                min = time.Substring(2, 2).TryIntParse(0);
            }
            else
            {
                var aTime = time.Split(':');
                hr = aTime[0].TryIntParse(0);
                min = aTime[1].TryIntParse(0);        
            }

            hr = hr < 0 || hr > 23 ? 0 : hr;
            min = min < 0 || min > 59 ? 0 : min;
            return hr.ToString().Trim().PadLeft(2, '0') + ":" + min.ToString().Trim().PadLeft(2, '0');
        }
    }
}
