using System;
using System.Collections.Generic;

namespace iBank.Services.Implementation.Shared
{
    public class DateComparer : IComparer<string>
    {
        public int Compare(string s1, string s2)
        {
            if (s1 == null) s1 = "";
            if (s2 == null) s2 = "";

            if (IsDate(s1) && IsDate(s2))
            {
                if (Convert.ToDateTime(s1) > Convert.ToDateTime(s2)) return 1;
                if (Convert.ToDateTime(s1) < Convert.ToDateTime(s2)) return -1;
                if (Convert.ToDateTime(s1) == Convert.ToDateTime(s2)) return 0;
            }

            if (IsDate(s1) && !IsDate(s2)) return 1;

            if (!IsDate(s1) && IsDate(s2)) return -1;

            return string.Compare(s1, s2, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsDate(string value)
        {
            DateTime test;
            return DateTime.TryParse(value, out test);
        }
    }
}