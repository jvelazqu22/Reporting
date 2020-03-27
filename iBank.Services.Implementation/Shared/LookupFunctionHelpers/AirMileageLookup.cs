using System;
using System.Collections.Generic;

using Domain.Helper;
using Domain.Orm.CISMastersQueries;
using Domain.Services;
using iBank.Repository.SQL.Interfaces;

namespace iBank.Services.Implementation.Shared.LookupFunctionHelpers
{
    public class AirMileageLookup
    {
        private static readonly CacheKeys _key = CacheKeys.AirMileageLookup;
        
        public static Dictionary<Tuple<string, string>, int> GetAirMileages(ICacheService cache, ICisMastersQueryable db)
        {
            Dictionary<Tuple<string, string>, int> airMileage;
            if (!cache.TryGetValue(_key, out airMileage))
            {
                var query = new GetAllAirMileagesQuery(db);
                airMileage = query.ExecuteQuery();
                cache.Set(_key, airMileage, DateTime.Now.AddDays(1));
            }

            return airMileage;
        }
    }
}
