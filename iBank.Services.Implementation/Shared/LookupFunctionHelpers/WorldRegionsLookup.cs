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
    public static class WorldRegionsLookup
    {
        private static readonly CacheKeys _key = CacheKeys.WorldRegionsLookup;

        public static List<KeyValue> GetWorldRegions(ICacheService cache, IMastersQueryable db)
        {
            List<KeyValue> worldRegions;
            if (!cache.TryGetValue(_key, out worldRegions))
            {
                var query = new GetAllWorldRegionsQuery(db);
                worldRegions = query.ExecuteQuery().ToList();
                cache.Set(_key, worldRegions, DateTime.Now.AddDays(1));
            }

            return worldRegions;
        }
    }
}
