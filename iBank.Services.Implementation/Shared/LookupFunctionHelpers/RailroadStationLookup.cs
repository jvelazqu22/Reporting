using System;
using System.Collections.Generic;
using System.Linq;

using Domain.Helper;
using Domain.Orm.Classes;
using Domain.Orm.iBankMastersQueries;
using Domain.Orm.iBankMastersQueries.Other;
using Domain.Services;
using iBank.Repository.SQL.Interfaces;

namespace iBank.Services.Implementation.Shared.LookupFunctionHelpers
{
    public static class RailroadStationLookup
    {
        private static readonly CacheKeys _key = CacheKeys.RailroadStationLookup;

        public static List<RRStationInformation> GetRailroadStations(ICacheService cache, IMastersQueryable db)
        {
            List<RRStationInformation> railroads;
            if (!cache.TryGetValue(_key, out railroads))
            {
                var query = new GetAllRailroadStationsQuery(db);
                railroads = query.ExecuteQuery().ToList();
                cache.Set(_key, railroads, DateTime.Now.AddDays(1));
            }

            return railroads;
        }
    }
}
