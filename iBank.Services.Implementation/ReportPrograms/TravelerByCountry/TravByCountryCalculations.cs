using System.Collections.Generic;

namespace iBank.Services.Implementation.ReportPrograms.TravelerByCountry
{
    public class TravByCountryCalculations
    {
        public string GetCrystalReportName()
        {
            return "ibTravByCountry";
        }

        public IList<string> GetExportFields()
        {
            return new List<string>
            {
                "country",
                "passlast",
                "passfrst",
                "tickets",
                "dispticks",
                "totdays",
                "longstay",
                "ctryticks",
                "ctrydays",
                "ctrylong"
            };
        }
    }
}