using System.Collections.Generic;
using System.Linq;

using CODE.Framework.Core.Utilities;

using Domain.Helper;
using Domain.Interfaces;

using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.Shared.WhereRoute.RouteFilters
{
    public class DateRangeRouting<T> where T : class, IRouteWhere
    {
        public IList<T> GetDataFilteredOnDateRange(IList<T> rawData, ReportGlobals globals, bool ignoreTravel)
        {
            var beginDate = globals.BeginDate.ToDateTimeSafe();
            var endDate = globals.EndDate.ToDateTimeSafe().Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            var dateTypeParameter = globals.GetParmValue(WhereCriteria.DATERANGE);

            int dateType;
            if (!int.TryParse(dateTypeParameter, out dateType))
            {
                //if we don't have a valid datetype, default to Departure Date. 
                dateType = 1;
            }

            switch ((DateType)dateType)
            {
                case DateType.RoutingDepartureDate:
                    rawData = rawData.Where(s => s.RDepDate >= beginDate && s.RDepDate <= endDate).ToList();
                    break;
                case DateType.OnTheRoadDatesSpecial:
                    if (!ignoreTravel)
                        rawData = rawData.Where(s => s.RArrDate >= beginDate && s.RDepDate <= endDate).ToList();
                    break;
                case DateType.RoutingArrivalDate:
                    var beginDate2 = beginDate.AddDays(-30);
                    var endDate2 = endDate.AddDays(30);
                    rawData = rawData.Where(s => s.RDepDate >= beginDate2 && s.RDepDate <= endDate2).ToList();
                    break;
            }

            return rawData;
        }
    }
}
