using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using System.Collections.Generic;
using System.Linq;

using Domain.Models.ReportPrograms.TravelDetail;

namespace iBank.Services.Implementation.ReportPrograms.TravelDetail
{
    public class TopTravAllRawDataCalculator
    {
        // Applies leg or segment filter to RawData list
        public static List<TravDetRawData> GetRawData(TravDetShared travDetShared, bool applyToSegment, BuildWhere buildWhere, TopTravAll topTravAll)
        {
            var rawDataList = travDetShared.RawDataList.Where(r => travDetShared.Legs.Select(s => s.RecKey).Distinct().ToList().Contains(r.RecKey)).ToList();
            if (applyToSegment)
            {
                var segData = travDetShared.Segments;
                segData = buildWhere.ApplyWhereRoute(segData, false);

                return topTravAll.GetLegDataFromFilteredSegData(rawDataList, segData);
            }
            var legDataAfterFilters = applyToSegment ? buildWhere.ApplyWhereRoute(travDetShared.Legs, false) : buildWhere.ApplyWhereRoute(travDetShared.Legs, true);
            rawDataList = rawDataList.Where(w => legDataAfterFilters.Select(s => s.RecKey).ToList().Contains(w.RecKey)).ToList();

            return rawDataList;
        }
    }
}
