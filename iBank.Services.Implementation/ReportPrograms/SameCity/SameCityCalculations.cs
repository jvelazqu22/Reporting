using CODE.Framework.Core.Utilities.Extensions;
using Domain.Helper;
using iBank.Server.Utilities.Classes;
using System.Collections.Generic;

namespace iBank.Services.Implementation.ReportPrograms.SameCity
{
    public class SameCityCalculations
    {
        public string GetCrystalReportName()
        {
            return "ibSameCity";
        }

        public IList<string> GetExportFields()
        {
            return new List<string>
            {
                "rarrdate",
                "sgrpcnt",
                "airline",
                "arrtime",
                "carrier",
                "class",
                "destcity",
                "destinat",
                "fltno",
                "invoice",
                "orgcity",
                "origin",
                "passfrst",
                "passlast",
                "recloc",
                "sorttime",
                "ticket",
                "plusmin"
            };
        }

        public int GetNumberOfTravelers(ReportGlobals globals)
        {
            var numberOfTravelers = (globals.GetParmValue(WhereCriteria.HOWMANY)).TryIntParse(0);
            if (numberOfTravelers < 1)
            {
                numberOfTravelers = 5;
            }

            return numberOfTravelers;
        }
    }
}