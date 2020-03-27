using System;
using System.Collections.Generic;
using System.Linq;
using iBank.Server.Utilities;

namespace iBank.Services.Implementation.Utilities
{
    public static class SpeedLookup
    {
        /// <summary>
        /// Used when a single lookup would normally be performed many times. 
        /// e.g., if all the home countries are USA, and there are 1000 records, we look it up once instead of a 1000 times. 
        /// </summary>
        /// <param name="key">they key</param>
        /// <param name="lookups">a list of key/value pairs</param>
        /// <returns> the value</returns>
        public static string Lookup(string key, List<Tuple<string, string>> lookups)
        {
            var lookup = lookups.FirstOrDefault(s => s.Item1.EqualsIgnoreCase(key));

            return lookup == null ? "NOT FOUND" : lookup.Item2;
        }
    }
}
