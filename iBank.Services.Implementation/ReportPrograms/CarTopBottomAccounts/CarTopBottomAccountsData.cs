using System.Collections.Generic;
using System.Linq;
using CODE.Framework.Core.Utilities.Extensions;
using Domain.Constants;
using Domain.Models.ReportPrograms.CarTopBottomAccountsReport;

namespace iBank.Services.Implementation.ReportPrograms.CarTopBottomAccounts
{
    public class CarTopBottomAccountsData
    {
        public List<FinalData> SortList(List<FinalData> finalList, string sortedBy, string sortOrder, string howManyString)
        {
            var results = new List<FinalData>();
            var volumeBookedDecimals = finalList.OrderByDescending(O => O.Carcost).Select(s => s.Carcost).ToList();
            switch (sortedBy)
            {
                case ReportFilters.CAR_TOP_BOTTOM_ACCOUNTS_RPT_SORT_BY_CAR_COST:
                    finalList = !sortOrder.Equals(ReportFilters.SORT_ORDER_ASCENDING)
                        ? finalList.OrderByDescending(o => o.Carcost).ThenBy(o => o.Account).ToList()
                        : finalList.OrderBy(o => o.Carcost).ThenBy(o => o.Account).ToList();
                    break;
                case ReportFilters.CAR_TOP_BOTTOM_ACCOUNTS_RPT_SORT_BY_RENTALS:
                    finalList = !sortOrder.Equals(ReportFilters.SORT_ORDER_ASCENDING)
                        ? finalList.OrderByDescending(o => o.Rentals).ThenBy(o => o.Account).ToList()
                        : finalList.OrderBy(o => o.Rentals).ThenBy(o => o.Account).ToList();
                    break;
                case ReportFilters.CAR_TOP_BOTTOM_ACCOUNTS_RPT_SORT_BY_DAYS:
                    finalList = !sortOrder.Equals(ReportFilters.SORT_ORDER_ASCENDING)
                        ? finalList.OrderByDescending(o => o.Days).ThenBy(o => o.Account).ToList()
                        : finalList.OrderBy(o => o.Days).ThenBy(o => o.Account).ToList();
                    break;
                case ReportFilters.CAR_TOP_BOTTOM_ACCOUNTS_RPT_SORT_BY_AVG_BOOK:
                    finalList = !sortOrder.Equals(ReportFilters.SORT_ORDER_ASCENDING)
                        ? finalList.OrderByDescending(o => o.avgbook).ThenBy(o => o.Account).ToList()
                        : finalList.OrderBy(o => o.avgbook).ThenBy(o => o.Account).ToList();
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
            fieldList.Add("rentals");
            fieldList.Add("days");
            fieldList.Add("carcost");
            fieldList.Add("bookrate");
            fieldList.Add("bookcnt");
            fieldList.Add("avgbook");

            return fieldList;
        }

        public string GetColumnName(string groupBy)
        {
            switch(groupBy)
            {
                case ReportFilters.CAR_TOP_BOTTOM_ACCOUNTS_RPT_GROUP_BY_PARENT_ACCOUNT_PARAM_VALUE:
                    return ReportNames.CAR_TOP_BOTTOM_ACCOUNTS_PARENT_ACCOUNT_COLUMN_NAME;
                case ReportFilters.CAR_TOP_BOTTOM_ACCOUNTS_RPT_GROUP_BY_DATA_SOURCE_PARAM_VALUE:
                    return ReportNames.CAR_TOP_BOTTOM_ACCOUNTS_DATA_SOURCE_COLUMN_NAME;
                default:
                    return ReportNames.CAR_TOP_BOTTOM_ACCOUNTS_ACCOUNT_COLUMN_NAME;
            }
        }

    }
}
