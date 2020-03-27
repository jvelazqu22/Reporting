using System.Collections.Generic;
using System.Linq;
using Domain.Models.ReportPrograms.TopBottomUdids;

namespace iBank.Services.Implementation.ReportPrograms.TopBottomUdids
{
    public static class TopBottomUdidsHelper
    {
        public static List<FinalData> SortAndFilter(List<RawData> rawDataList, string sortBy, bool sortDescending,int howMany)
        {
            IEnumerable<FinalData> finalData;
            if (sortBy.Equals("1"))
            {
                finalData = sortDescending
                    ? rawDataList.OrderByDescending(s => s.UdidCount).Select(s => new FinalData { UdidText = s.UdidText, UdidCount = s.UdidCount })
                    : rawDataList.OrderBy(s => s.UdidCount).Select(s => new FinalData { UdidText = s.UdidText, UdidCount = s.UdidCount });
            }
            else
            {
                 finalData =  rawDataList.OrderBy(s => s.UdidText).Select(s => new FinalData {UdidText = s.UdidText, UdidCount = s.UdidCount});
            }

            return howMany > 0
                ? finalData.Take(howMany).ToList()
                : finalData.ToList();
        }
    }
}
