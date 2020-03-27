using System.Collections.Generic;
using System.Linq;

using Domain.Interfaces;
using Domain.Models.ReportPrograms.UserDefinedReport;

using iBank.Server.Utilities;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined
{
    public static class TripParameterFilter
    {
        public static List<RawData> GetDataFromRecloc(Dictionary<int, List<RawData>> dict, string recloc)
        {
            var data = new List<RawData>();

            foreach (var trip in dict.Values)
            {
                data.AddRange(trip.Where(x => x.Recloc.EqualsIgnoreCase(recloc)));
            }

            return data;
        }

        public static List<HotelRawData> GetDataFromRecloc(Dictionary<int, List<HotelRawData>> dict, string recloc)
        {
            var data = new List<HotelRawData>();

            foreach (var trip in dict.Values)
            {
                data.AddRange(trip.Where(x => x.Recloc.EqualsIgnoreCase(recloc)));
            }

            return data;
        }

        public static int GetCountOfMatchingRecKeys<T>(IEnumerable<int> reckeys, Dictionary<int, List<T>> dict) where T : IRecKey
        {
            var count = 0;
            foreach (var key in reckeys)
            {
                if (dict.ContainsKey(key)) count += dict[key].Count;
            }

            return count;
        }

        public static List<T> GetDataFromReckeys<T>(IEnumerable<int> reckeys, Dictionary<int, List<T>> dict) where T : IRecKey
        {
            var data = new List<T>();

            foreach (var key in reckeys)
            {
                if(dict.ContainsKey(key)) data.AddRange(dict[key]);
            }

            return data;
        }


    }
}
