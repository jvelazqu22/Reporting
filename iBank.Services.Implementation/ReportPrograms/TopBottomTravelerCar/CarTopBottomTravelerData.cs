using CODE.Framework.Core.Utilities.Extensions;
using Domain.Constants;
using Domain.Models.ReportPrograms.TopBottomTravelersCar;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.TopBottomTravelerCar
{
    public class CarTopBottomTravelerData
    {
        public List<FinalData> SortList(List<FinalData> finalList, string sortedBy, string sortOrder, string howManyString)
        {
            var results = new List<FinalData>();
            var volumeBookedDecimals = finalList.OrderByDescending(O => O.Carcost).Select(s => s.Carcost).ToList();
            switch (sortedBy)
            {
                case ReportFilters.CAR_TOP_BOTTOM_TRAVELER_RPT_SORT_BY_CAR_COST:
                    finalList = !sortOrder.Equals(ReportFilters.SORT_ORDER_ASCENDING)
                        ? finalList.OrderByDescending(o => o.Carcost).ToList()
                        : finalList.OrderBy(o => o.Carcost).ToList();
                    break;
                case ReportFilters.CAR_TOP_BOTTOM_TRAVELER_RPT_SORT_BY_RENTALS:
                    finalList = !sortOrder.Equals(ReportFilters.SORT_ORDER_ASCENDING)
                        ? finalList.OrderByDescending(o => o.Rentals).ToList()
                        : finalList.OrderBy(o => o.Rentals).ToList();
                    break;
                case ReportFilters.CAR_TOP_BOTTOM_TRAVELER_RPT_SORT_BY_DAYS:
                    finalList = !sortOrder.Equals(ReportFilters.SORT_ORDER_ASCENDING)
                        ? finalList.OrderByDescending(o => o.Days).ToList()
                        : finalList.OrderBy(o => o.Days).ToList();
                    break;
                case ReportFilters.CAR_TOP_BOTTOM_TRAVELER_RPT_SORT_BY_AVG_BOOK:
                    finalList = !sortOrder.Equals(ReportFilters.SORT_ORDER_ASCENDING)
                        ? finalList.OrderByDescending(o => o.Avgbook).ToList()
                        : finalList.OrderBy(o => o.Avgbook).ToList();
                    break;
                default:
                    finalList = !sortOrder.Equals(ReportFilters.SORT_ORDER_DECENDING) && !sortedBy.Equals("5")
                        ? finalList.OrderByDescending(o => o.Passlast).ThenBy(o => o.Passfrst).ToList()
                        : finalList.OrderBy(o => o.Passlast).ThenBy(o => o.Passfrst).ToList();
                    break;
            }

            var howMany = howManyString.TryIntParse(-1);

            results = howMany > 0 ? finalList.Take(howMany).ToList() : finalList;

            return results;
        }

        public List<string> GetExportFields()
        {
            var fieldList = new List<string>();

            fieldList.Add("passlast");
            fieldList.Add("passfrst");
            fieldList.Add("rentals");
            fieldList.Add("days");
            fieldList.Add("carcost");
            fieldList.Add("bookrate");
            fieldList.Add("bookcnt");
            fieldList.Add("avgbook");

            return fieldList;
        }
    }
}
