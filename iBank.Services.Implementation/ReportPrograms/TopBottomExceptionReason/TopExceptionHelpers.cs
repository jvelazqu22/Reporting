using System;
using System.Collections.Generic;
using System.Linq;
using CODE.Framework.Core.Utilities.Extensions;
using Domain.Helper;
using Domain.Models.ReportPrograms.TopBottomExceptionReasonReport;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.ReportPrograms.TopBottomExceptionReason
{
    public static class TopExceptionHelpers
    {
        public static List<FinalData> SortData(List<FinalData> finalDataList, ReportGlobals globals)
        {

            var sortBy = globals.GetParmValue(WhereCriteria.SORTBY);
            var howMany = globals.GetParmValue(WhereCriteria.HOWMANY).TryIntParse(0);
            var descending = !globals.ParmValueEquals(WhereCriteria.RBSORTDESCASC, "2") && !sortBy.Equals("4");

            switch (sortBy)
            {
                case "1":
                    finalDataList = descending
                        ? finalDataList.OrderByDescending(s => s.LostAmt).ToList()
                        : finalDataList.OrderBy(s => s.LostAmt).ToList();
                    break;
                case "2":
                    finalDataList = descending
                        ? finalDataList.OrderByDescending(s => s.NumOccurs).ToList()
                        : finalDataList.OrderBy(s => s.NumOccurs).ToList();
                    break;
                case "3":
                    finalDataList = descending
                        ? finalDataList.OrderByDescending(s => s.Category).ToList()
                        : finalDataList.OrderBy(s => s.Category).ToList();
                    break;
                default:
                    //data is already sorted
                    howMany = 0;
                    break;

            }

            return howMany > 0
                ? finalDataList.Take(howMany).ToList()
                : finalDataList;

        }

        public static string SpeedLookup(string reascode, string acct, List<Tuple<string, string, string>> lookups)
        {
            var lookup = lookups.FirstOrDefault(s => s.Item1.EqualsIgnoreCase(reascode) && s.Item2.EqualsIgnoreCase(acct));

            return lookup == null ? "REASON NOT FOUND" : lookup.Item3;
        }
    }
}
