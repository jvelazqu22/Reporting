using System;
using System.Collections.Generic;

namespace iBank.Services.Implementation.Shared
{
    public class NumericComparer : IComparer<string>
    {
        public int Compare(string s1, string s2)
        {
            if (s1 == null) s1 = "";
            if (s2 == null) s2 = "";

            if (IsNumeric(s1) && IsNumeric(s2))
            {
                if (Convert.ToInt32(s1) > Convert.ToInt32(s2)) return 1;
                if (Convert.ToInt32(s1) < Convert.ToInt32(s2)) return -1;
                if (Convert.ToInt32(s1) == Convert.ToInt32(s2)) return 0;
            }

            if (IsNumeric(s1) && !IsNumeric(s2)) return 1;

            if (!IsNumeric(s1) && IsNumeric(s2)) return -1;
            
            return string.Compare(s1, s2, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsNumeric(string value)
        {
            int test;
            return int.TryParse(value, out test);
        }
    }
}
