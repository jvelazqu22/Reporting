using System.Collections.Generic;

using Domain.Helper;

using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.ReportPrograms.Arrival
{
    public class ArrivalCalculations
    {
        public string GetCrystalReportName(bool includeAllLegs)
        {
            return includeAllLegs ? "ibArrival3" : "ibArrival";
        }

        public bool SortByName(ReportGlobals globals)
        {
            var sortBy = globals.GetParmValue(WhereCriteria.SORTBY);

            return sortBy == "2";
        }

        public bool UseCrystalPageBreak(ReportGlobals globals, bool includePageBreakByDate)
        {
            var outputType = globals.GetParmValue(WhereCriteria.OUTPUTTYPE);
            if (!(outputType.Equals("2") || outputType.Equals("5") || outputType.Equals("99")))
            {
                if (includePageBreakByDate)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsArrivalDateRangeSearch(ReportGlobals globals)
        {
            return globals.ParmValueEquals(WhereCriteria.DATERANGE, "5");
        }

        public IList<string> GetExportFields(UserBreaks userBreaks, bool accountBreak, UserInformation user, bool includeAllLegs)
        {
            var fieldList = new List<string>();

            if (accountBreak)
            {
                fieldList.Add("acct");
                fieldList.Add("acctdesc");
            }
            if (userBreaks.UserBreak1)
            {
                var break1 = string.IsNullOrEmpty(user.Break1Name) ? "break_1" : user.Break1Name;
                fieldList.Add($"break1 as {break1}");
            }
            if (userBreaks.UserBreak2)
            {
                var break2 = string.IsNullOrEmpty(user.Break2Name) ? "break_2" : user.Break2Name;
                fieldList.Add($"break2 as {break2}");
            }
            if (userBreaks.UserBreak3)
            {
                var break3 = string.IsNullOrEmpty(user.Break3Name) ? "break_3" : user.Break3Name;
                fieldList.Add($"break3 as {break3}");
            }

            if (includeAllLegs)
            {
                fieldList.AddRange(new List<string>
                           {
                               "reckey",
                               "finaldest",
                               "finaldesc",
                               "pseudocity",
                               "recloc",
                               "rarrdate",
                               "passlast",
                               "passfrst",
                               "sortarrtim",
                               "airline",
                               "alinedesc",
                               "fltno",
                               "deptime",
                               "origin",
                               "orgdesc",
                               "destinat",
                               "destdesc",
                               "arrtime",
                               "txtarrdate"
                           });
            }
            else
            {
                fieldList.AddRange(new List<string>
                           {
                                "reckey",
                                "destinat",
                                "destdesc",
                                "invoice",
                                "recloc",
                                "plusmin",
                                "rarrdate",
                                "passlast",
                                "passfrst",
                                "airline",
                                "alinedesc",
                                "pseudocity",
                                "fltno",
                                "origin",
                                "orgdesc",
                                "arrtime",
                                "fltsort",
                                "sortarrtim",
                                "seg_cntr",
                                "txtarrdate"
                           });
            }

            return fieldList;
        }


    }
}
