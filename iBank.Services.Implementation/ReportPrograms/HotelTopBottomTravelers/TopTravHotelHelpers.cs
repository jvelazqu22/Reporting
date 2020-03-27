using System.Collections.Generic;
using System.Linq;
using CODE.Framework.Core.Utilities.Extensions;
using Domain.Helper;
using Domain.Models.ReportPrograms.TopBottomTravelersHotelReport;

using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.ReportPrograms.HotelTopBottomTravelers
{
    public class TopTravHotelHelpers
    {
        // Sort data
        public static List<FinalData> SortData(List<FinalData> finalDataList, ReportGlobals globals)
        {
            // Get report paramaters
            var sortBy = globals.GetParmValue(WhereCriteria.SORTBY);
            var howMany = globals.GetParmValue(WhereCriteria.HOWMANY).TryIntParse(0);
            var descending = !globals.ParmValueEquals(WhereCriteria.RBSORTDESCASC, "2");

            switch (sortBy)
            {
                // Volume Booked
                case "1":
                    finalDataList = descending
                        ? finalDataList.OrderByDescending(s => s.HotelCost).ToList()
                        : finalDataList.OrderBy(s => s.HotelCost).ToList();
                    break;
                // # of Stays
                case "2":
                    finalDataList = descending
                        ? finalDataList.OrderByDescending(s => s.Stays).ToList()
                        : finalDataList.OrderBy(s => s.Stays).ToList();
                    break;
                // # of Nights
                case "3":
                    finalDataList = descending
                        ? finalDataList.OrderByDescending(s => s.Nights).ToList()
                        : finalDataList.OrderBy(s => s.Nights).ToList();
                    break;
                // Average cost per trip
                case "4":
                    finalDataList = descending
                        ? finalDataList.OrderByDescending(s => s.AvgBook).ToList()
                        : finalDataList.OrderBy(s => s.AvgBook).ToList();
                    break;
                // Name
                default:
                    finalDataList = descending
                        ? finalDataList.OrderByDescending(s => s.PassLast).ThenByDescending(s => s.PassFrst).ToList()
                        : finalDataList.OrderBy(s => s.PassLast).ThenBy(s => s.PassFrst).ToList();
                    howMany = 0;
                    break;

            }

            // Return top howMany results
            return howMany > 0
                ? finalDataList.Take(howMany).ToList()
                : finalDataList;

        }
    }
}
