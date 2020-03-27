using CODE.Framework.Core.Utilities.Extensions;
using Domain.Constants;
using System.Collections.Generic;
using System.Linq;
using Domain.Models.ReportPrograms.TravelDetail;
using Domain.Helper;

namespace iBank.Services.Implementation.ReportPrograms.TravelDetail.TravelDetailByUdid
{
    public class TopTravAllData
    {
        // Sort list and return number of results wanted
        public static List<TopTravAllByUdidFinalData> SortList(List<TopTravAllByUdidFinalData> finalList, string sortedBy, string sortOrder, string howManyString)
        {
            switch (sortedBy)
            {
                case ReportFilters.TOP_TRAVELER_COMBINED_RPT_SORT_BY_TRIP_COUNT:
                    finalList = !sortOrder.Equals("2")
                        ? finalList.OrderByDescending(o => o.tripcount).ToList()
                        : finalList.OrderBy(o => o.tripcount).ToList();
                    break;
                case ReportFilters.TOP_TRAVELER_COMBINED_RPT_SORT_BY_DAYS_ON_THE_ROAD:
                    finalList = !sortOrder.Equals("2")
                        ? finalList.OrderByDescending(o => o.daysonroad).ToList()
                        : finalList.OrderBy(o => o.daysonroad).ToList();
                    break;
                case ReportFilters.TOP_TRAVELER_COMBINED_RPT_SORT_BY_RAIL_COUNT:
                    finalList = !sortOrder.Equals("2")
                        ? finalList.OrderByDescending(o => o.railcount).ToList()
                        : finalList.OrderBy(o => o.railcount).ToList();
                    break;
                default:
                    finalList = !sortOrder.Equals("2")
                        ? finalList.OrderByDescending(o => o.tripcost).ToList()
                        : finalList.OrderBy(o => o.tripcost).ToList();
                    break;
            }
            // Trim list
            var howMany = howManyString.TryIntParse(-1);
            return howMany > 0 ? finalList.Take(howMany).ToList() : finalList;
        }

        // Get required export fields for XLS/CSV
        public static List<string> GetExportFields(UserBreaks userBreaks, bool accountBreak, UserInformation user, bool displayCo2)
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

            fieldList.Add("passlast");
            fieldList.Add("passfrst");
            fieldList.Add("tripcount");
            fieldList.Add("airchg");
            if (displayCo2) fieldList.Add("airco2");
            if (!displayCo2)
            {
                fieldList.Add("railcount");
                fieldList.Add("railchg");
            }
            fieldList.Add("cardays");
            fieldList.Add("carcost");
            if (displayCo2) fieldList.Add("carco2");
            fieldList.Add("hotnights");
            fieldList.Add("hotelcost");
            if (displayCo2) fieldList.Add("hotelco2");
            fieldList.Add("tripcost");
            if (displayCo2) fieldList.Add("totco2");
            fieldList.Add("daysonroad");

            return fieldList;
        }
    }
}
