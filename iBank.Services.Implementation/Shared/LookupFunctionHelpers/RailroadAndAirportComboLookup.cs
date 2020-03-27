using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Domain.Helper;
using Domain.Services;
using iBank.Repository.SQL.Interfaces;
using iBank.Services.Implementation.Classes;

namespace iBank.Services.Implementation.Shared.LookupFunctionHelpers
{
    public static class RailroadAndAirportComboLookup
    {
        private static readonly CacheKeys _key = CacheKeys.RailroadAndAirportComboLookup;

        public static List<PortInformation> GetAirports(ICacheService cache, IMasterDataStore store)
        {
            List<PortInformation> ports;
            if (!cache.TryGetValue(_key, out ports))
            {
                var airports = GetFromAirports(cache, store.MastersQueryDb);
                var rails = GetFromRailroads(cache, store.MastersQueryDb);

                ports = airports.Concat(rails).ToList();
                cache.Set(_key, ports, DateTime.Now.AddDays(1));
            }

            return ports;
        }

        private static IEnumerable<PortInformation> GetFromAirports(ICacheService cache, IMastersQueryable db)
        {
            var airports = AirportLookup.GetAirports(cache, db);

            return airports.Select(x => new PortInformation
            {
                PortCode = x.Airport.Trim().PadRight(10),
                City = x.City,
                State = x.State,
                Mode = x.Mode,
                Metro = x.Metro,
                CountryCode = x.CountryCode,
                RegionCode = x.RegionCode
            });
        }

        private static IEnumerable<PortInformation> GetFromRailroads(ICacheService cache, IMastersQueryable db)
        {
            var rails = RailroadStationLookup.GetRailroadStations(cache, db);

            return rails.Select(x => new PortInformation
            {
                PortCode = x.StationNumber.ToString(CultureInfo.InvariantCulture).Trim().PadRight(10),
                City = x.City,
                State = x.State,
                Mode = "R",
                Metro = x.Metro,
                CountryCode = x.CountryCode,
                RegionCode = x.RegionCode
            });
        }
    }
}
