using System.Collections.Generic;
using System.Linq;

using Domain.Helper;
using Domain.Interfaces;

using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.Shared.WhereRoute.RouteFilters
{
    public class FlightTypeFilter<T> where T : class, IRouteWhere
    {
        public IList<T> GetDataFilteredOnDomesticInternationalCode(IList<T> rawData, ReportGlobals globals)
        {
            var domIntl = globals.GetParmValue(WhereCriteria.DOMINTL);
            var txtBuilder = new WhereRouteTextBuilder();
            globals.WhereText += txtBuilder.BuildDomesticInternationalWhereText(domIntl, globals.LegDIT, globals.UserLanguage);

            switch (domIntl)
            {
                case "2":
                    rawData = rawData.Where(s => s.DitCode.EqualsIgnoreCase("D")).ToList();
                    break;
                case "3":
                    rawData = rawData.Where(s => s.DitCode.EqualsIgnoreCase("I")).ToList();
                    break;
                case "4":
                    rawData = rawData.Where(s => s.DitCode.EqualsIgnoreCase("T")).ToList();
                    break;
                case "5":
                    rawData = rawData.Where(s => !s.DitCode.EqualsIgnoreCase("D")).ToList();
                    break;
                case "6":
                    rawData = rawData.Where(s => !s.DitCode.EqualsIgnoreCase("I")).ToList();
                    break;
                case "7":
                    rawData = rawData.Where(s => !s.DitCode.EqualsIgnoreCase("T")).ToList();
                    break;
            }

            return rawData;
        }
    }
}
