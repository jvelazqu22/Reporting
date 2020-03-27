using System.Collections.Generic;
using System.Linq;

using Domain.Interfaces;

namespace iBank.Services.Implementation.Shared.WhereRoute.Helpers
{
    public class Combiner<T> where T : class, IRouteWhere
    {
        /// <summary>
        /// Returns the entire trip if the filtered data contains at least one leg of that trip
        /// </summary>
        /// <param name="rawData"></param>
        /// <param name="possibleFilteredData"></param>
        /// <returns></returns>
        public List<T> GetAllLegsInTrip(IList<T> rawData, IEnumerable<WhereRouteData<T>> possibleFilteredData)
        {
            var filteredData = GetDataWithFiltersApplied(possibleFilteredData).ToList();
            var filteredReckeys = new List<int>();
            foreach (var data in filteredData)
            {
                filteredReckeys.AddRange(data.Data.Select(x => x.RecKey));
            }
            
            return rawData.Where(x => filteredReckeys.Contains(x.RecKey)).ToList();
        }

        /// <summary>
        /// Joins all the filtered data to only return legs that match across all filters
        /// </summary>
        /// <param name="rawData"></param>
        /// <param name="possibleFilteredData"></param>
        /// <returns></returns>
        public IEnumerable<T> JoinMatchingData(IEnumerable<T> rawData, IEnumerable<WhereRouteData<T>> possibleFilteredData)
        {
            var filteredData = GetDataWithFiltersApplied(possibleFilteredData).ToList();
            
            if (filteredData.Count == 0) return rawData;
            if (filteredData.Count == 1) return filteredData[0].Data;
            if (filteredData.Count == 2) return JoinMatchingData(filteredData[0].Data, filteredData[1].Data);
            
            var aggregatedData = JoinMatchingData(filteredData[0].Data, filteredData[1].Data);
            for (var i = 2; i < filteredData.Count; i++)
            {
                aggregatedData = JoinMatchingData(aggregatedData, filteredData[i].Data);
            }

            return aggregatedData;
        }

        private IEnumerable<T> JoinMatchingData(IEnumerable<T> list1, IEnumerable<T> list2)
        {
            return from l1 in list1
                           join l2 in list2 on
                           new { l1.RecKey, l1.SeqNo, l1.Origin, l1.Destinat } equals
                           new { l2.RecKey, l2.SeqNo, l2.Origin, l2.Destinat }
                           select l1;
        }

        private IEnumerable<WhereRouteData<T>> GetDataWithFiltersApplied(IEnumerable<WhereRouteData<T>> possibleFilteredData)
        {
            return possibleFilteredData.Where(x => x.FiltersApplied).ToList();
        }
    }
}
