using System;
using System.Collections.Generic;
using System.Linq;

using Domain.Helper;
using Domain.Orm.iBankMastersQueries;
using Domain.Orm.iBankMastersQueries.Other;
using Domain.Services;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;

namespace iBank.Services.Implementation.Shared.LookupFunctionHelpers
{
    public static class AirlineLookup
    {
        private static readonly CacheKeys _key = CacheKeys.AirlineLookup;
        public static List<airlines> GetAirlines(ICacheService cache, IMastersQueryable db)
        {
            List<airlines> airlines;
            if (!cache.TryGetValue(_key, out airlines))
            {
                var query = new GetAllAirlinesQuery(db);
                airlines = query.ExecuteQuery().ToList();
                cache.Set(_key, airlines, DateTime.Now.AddDays(1));
            }

            return airlines;
        }
    }
}
