using System.Collections.Generic;
using System.Linq;
using Domain.Helper;
using Domain.Interfaces;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared.WhereRoute.Helpers;

namespace iBank.Services.Implementation.Shared.WhereRoute.RouteFilters
{
    public class AirlineRouting<T> where T : class, IRouteWhere
    {
        public WhereRouteData<T> GetAirlineFilteredData(IList<T> rawData, ReportGlobals globals)
        {
            var routedData = new WhereRouteData<T> { FiltersApplied = false, Data = rawData };
            var mode = globals.GetParmValue(WhereCriteria.MODE);
            var airline = globals.GetParmValue(WhereCriteria.AIRLINE);
            var airlineList = globals.GetParmValue(WhereCriteria.INAIRLINE);
            var notInAirlineList = globals.IsParmValueOn(WhereCriteria.NOTINAIRLINES);

            var txtBuilder = new WhereRouteTextBuilder();
            globals.WhereText += txtBuilder.BuildWhereTextFromAirlines(mode, globals.LanguageVariables);
            
            if (string.IsNullOrEmpty(airlineList)) airlineList = airline;

            var pl = new PickListParms(globals);
            pl.ProcessList(airlineList, string.Empty, "AIRLINES");

            if (pl.PickList.Any())
            {
                routedData.FiltersApplied = true;

                routedData.Data = notInAirlineList
                                  ? rawData.Where(s => !pl.PickList.Contains(s.Airline.Trim())).ToList()
                                  : rawData.Where(s => pl.PickList.Contains(s.Airline.Trim())).ToList();
            }

            switch (mode)
            {
                case "1":
                    routedData.Data = GetAirModeData(routedData.Data).ToList();
                    routedData.FiltersApplied = true;
                    break;
                case "2":
                    routedData.Data = GetRailModeData(routedData.Data).ToList();
                    routedData.FiltersApplied = true;
                    break;
            }

            return routedData;
        }

        private IEnumerable<T> GetAirModeData(IEnumerable<T> data)
        {
            return data.Where(x => x.Mode.Equals("A"));
        }

        private IEnumerable<T> GetRailModeData(IEnumerable<T> data)
        {
            return data.Where(x => x.Mode.Equals("R"));
        }
    }
}
