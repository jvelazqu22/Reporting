using System.Collections.Generic;

using Domain.Helper;

using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.ReportPrograms.Departures
{
    public class DepartCalculations
    {
        public bool IsDepartureDateRangeSearch(ReportGlobals globals)
        {
            return globals.ParmValueEquals(WhereCriteria.DATERANGE, "5");
        }

        public bool SortByName(ReportGlobals globals)
        {
            var sortBy = globals.GetParmValue(WhereCriteria.SORTBY);

            return sortBy == "2";
        }

        public string GetCrystalReportName(bool includeAllLegs)
        {
            return includeAllLegs ? "ibDepart3" : "ibDepart";
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
                fieldList.AddRange( new List<string>
                           {
                                "reckey",
                                "frstorigin",
                                "frstdesc",
                                "recloc",
                                "rdepdate",
                                "passlast",
                                "passfrst",
                                "sortdeptim",
                                "airline",
                                "alinedesc",
                                "fltno",
                                "deptime",
                                "pseudocity",
                                "origin",
                                "orgdesc",
                                "destinat",
                                "destdesc",
                                "arrtime",
                                "txtdepdate"
                           });
            }
            else
            {
                fieldList.AddRange(new List<string>
                           {
                                "reckey",
                                "origin",
                                "orgdesc",
                                "invoice",
                                "pseudocity",
                                "recloc",
                                "plusmin",
                                "rdepdate",
                                "passlast",
                                "passfrst",
                                "airline",
                                "alinedesc",
                                "fltno",
                                "destinat",
                                "destdesc",
                                "deptime",
                                "fltsort",
                                "sortdeptim",
                                "seg_cntr",
                                "txtdepdate"
                           });
            }
            return fieldList;
        }
    }
}
