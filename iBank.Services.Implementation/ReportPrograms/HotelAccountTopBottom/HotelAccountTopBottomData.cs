using System.Collections.Generic;
using Domain.Models.ReportPrograms.HotelAccountTopBottom;
using System.Linq;
using CODE.Framework.Core.Utilities.Extensions;
using Domain.Constants;

namespace iBank.Services.Implementation.ReportPrograms.HotelAccountTopBottom
{
    public class HotelAccountTopBottomData
    {
        public List<FinalData> SortList(List<FinalData> finalList, string sortedBy, string sortOrder, string howManyString)
        {
            var results = new List<FinalData>();
            var volumeBookedDecimals = finalList.OrderByDescending(O => O.Hotelcost).Select(s => s.Hotelcost).ToList();
            switch (sortedBy)
            {
                case ReportFilters.HOTEL_TOP_BOTTOM_ACCOUNTS_RPT_SORT_BY_COST:
                    finalList = !sortOrder.Equals(ReportFilters.SORT_ORDER_ASCENDING)
                        ? finalList.OrderByDescending(o => o.Hotelcost).ThenBy(o => o.Account).ToList()
                        : finalList.OrderBy(o => o.Hotelcost).ThenBy(o => o.Account).ToList();
                    break;
                case ReportFilters.HOTEL_TOP_BOTTOM_ACCOUNTS_RPT_SORT_BY_STAYS:
                    finalList = !sortOrder.Equals(ReportFilters.SORT_ORDER_ASCENDING)
                        ? finalList.OrderByDescending(o => o.Stays).ThenBy(o => o.Account).ToList()
                        : finalList.OrderBy(o => o.Stays).ThenBy(o => o.Account).ToList();
                    break;
                case ReportFilters.HOTEL_TOP_BOTTOM_ACCOUNTS_RPT_SORT_BY_NIGHTS:
                    finalList = !sortOrder.Equals(ReportFilters.SORT_ORDER_ASCENDING)
                        ? finalList.OrderByDescending(o => o.Nights).ThenBy(o => o.Account).ToList()
                        : finalList.OrderBy(o => o.Nights).ThenBy(o => o.Account).ToList();
                    break;
                case ReportFilters.HOTEL_TOP_BOTTOM_ACCOUNTS_RPT_SORT_BY_BOOK_RATE:
                    finalList = !sortOrder.Equals(ReportFilters.SORT_ORDER_ASCENDING)
                        ? finalList.OrderByDescending(o => o.AveBookRate).ThenBy(o => o.Account).ToList()
                        : finalList.OrderBy(o => o.AveBookRate).ThenBy(o => o.Account).ToList();
                    break;
                default:
                    finalList = !sortOrder.Equals(ReportFilters.SORT_ORDER_DECENDING) && !sortedBy.Equals("5")
                        ? finalList.OrderByDescending(o => o.SourceAbbr).ThenBy(o => o.Account).ToList()
                        : finalList.OrderBy(o => o.SourceAbbr).ThenBy(o => o.Account).ToList();
                    break;
            }

            var howMany = howManyString.TryIntParse(-1);

            results = howMany > 0 ? finalList.Take(howMany).ToList() : finalList;

            return results;
        }

        public List<string> GetExportFields()
        {
            var fieldList = new List<string>();

            fieldList.Add("account");
            fieldList.Add("acctname");
            fieldList.Add("stays");
            fieldList.Add("nights");
            fieldList.Add("hotelcost");
            fieldList.Add("bookrate");
            fieldList.Add("bookcnt");
            fieldList.Add("avgbook");

            return fieldList;
        }

        public string GetColumnName(string groupBy)
        {
            switch(groupBy)
            {
                case ReportFilters.HOTEL_TOP_BOTTOM_ACCOUNTS_RPT_GROUP_BY_PARENT_ACCOUNT_PARAM_VALUE:
                    return ReportNames.HOTEL_TOP_BOTTOM_ACCOUNTS_PARENT_ACCOUNT_COLUMN_NAME;
                case ReportFilters.HOTEL_TOP_BOTTOM_ACCOUNTS_RPT_GROUP_BY_DATA_SOURCE_PARAM_VALUE:
                    return ReportNames.HOTEL_TOP_BOTTOM_ACCOUNTS_DATA_SOURCE_COLUMN_NAME;
                default:
                    return ReportNames.HOTEL_TOP_BOTTOM_ACCOUNTS_ACCOUNT_COLUMN_NAME;
            }
        }

    }
}
