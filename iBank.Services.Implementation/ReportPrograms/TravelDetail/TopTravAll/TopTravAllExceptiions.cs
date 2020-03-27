using Domain.Constants;
using System.Collections.Generic;
using System.Linq;

using Domain.Models.ReportPrograms.TravelDetail;

namespace iBank.Services.Implementation.ReportPrograms.TravelDetail
{
    public class TopTravAllExceptions
    {
        // Filter List based on exception option
        public static List<TopTravAllFinalData> ApplyExceptionsToFinalDataList(List<TopTravAllFinalData> finalDataList, string exceptions)
        {
            if (exceptions.Equals(string.Empty)) return finalDataList;

            else if (exceptions.Contains(ReportFilters.DDAIRRAILCARHOTELOPTIONS_AIR_RAIL))
            {
                return finalDataList.Where(w => w.cardays == 0 && w.carcost == 0 && w.hotnights == 0 && w.hotelcost == 0).ToList();
            }
            else if (exceptions.Contains(ReportFilters.DDAIRRAILCARHOTELOPTIONS_NO_AIR))
            {
                return finalDataList.Where(w => w.tripcount == 0 && w.railcount == 0 && w.airchg == 0 && w.railchg == 0).ToList();
            }
            else if (exceptions.Contains(ReportFilters.DDAIRRAILCARHOTELOPTIONS_NO_CAR))
            {
                return finalDataList.Where(w => w.cardays == 0 && w.carcost == 0).ToList();
            }
            else if (exceptions.Contains(ReportFilters.DDAIRRAILCARHOTELOPTIONS_NO_HOTEL))
            {
                return finalDataList.Where(w => w.hotnights == 0 && w.hotelcost == 0).ToList();
            }
            else if (exceptions.Contains(ReportFilters.DDAIRRAILCARHOTELOPTIONS_HOTEL_ONLY))
            {
                return finalDataList.Where(w => w.tripcount == 0 && w.railcount == 0 && w.airchg == 0 && w.railchg == 0 && w.cardays == 0 && w.carcost == 0).ToList();
            }
            else if (exceptions.Contains(ReportFilters.DDAIRRAILCARHOTELOPTIONS_CAR_ONLY))
            {
                return finalDataList.Where(w => w.tripcount == 0 && w.railcount == 0 && w.airchg == 0 && w.railchg == 0 && w.hotnights == 0 && w.hotelcost == 0).ToList();
            }
            if (exceptions.Contains("Air Only"))
            {
                return finalDataList.Where(w => w.railcount == 0 && w.railchg == 0).ToList();
            }
            else if (exceptions.Contains("Rail Only"))
            {
                return finalDataList.Where(w => w.railcount > 0 && w.railchg > 0).ToList();
            }

            return finalDataList;
        }

    }
}
