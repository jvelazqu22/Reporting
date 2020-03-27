using System;
using System.Collections.Generic;

namespace iBank.Services.Implementation.Shared
{
    public class DecimalComparer : IComparer<string>
    {
        public int Compare(string s1, string s2)
        {
            if (s1 == null) s1 = "";
            if (s2 == null) s2 = "";

            if (IsDecimal(s1) && IsDecimal(s2))
            {
                if (Convert.ToDecimal(s1) > Convert.ToDecimal(s2)) return 1;
                if (Convert.ToDecimal(s1) < Convert.ToDecimal(s2)) return -1;
                if (Convert.ToDecimal(s1) == Convert.ToDecimal(s2)) return 0;
            }
            
            if (IsDecimal(s1) && !IsDecimal(s2))  return 1;

            if (!IsDecimal(s1) && IsDecimal(s2)) return -1;

            return string.Compare(s1, s2, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsDecimal(string value)
        {
            decimal test;
            return decimal.TryParse(value,out test);
        }
    }
}