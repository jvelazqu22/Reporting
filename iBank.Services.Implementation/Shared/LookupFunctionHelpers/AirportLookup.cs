using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Helper;
using Domain.Orm.Classes;
using Domain.Orm.iBankMastersQueries.Other;
using Domain.Services;
using iBank.Repository.SQL.Interfaces;

namespace iBank.Services.Implementation.Shared.LookupFunctionHelpers
{
    public static class AirportLookup
    {
        private static readonly CacheKeys _key = CacheKeys.AirportLookup;

        public static List<AirportInformation> GetAirports(ICacheService cache, IMastersQueryable db)
        {
            List<AirportInformation> airports;
            if (!cache.TryGetValue(_key, out airports))
            {
                var query = new GetAllAirportsQuery(db);
                airports = query.ExecuteQuery().ToList();
                cache.Set(_key, airports, DateTime.Now.AddDays(1));
            }

            return airports;
        }
    }
}
