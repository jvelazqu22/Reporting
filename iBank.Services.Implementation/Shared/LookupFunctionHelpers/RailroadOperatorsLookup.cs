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
    public static class RailroadOperatorsLookup
    {
        private static readonly CacheKeys _key = CacheKeys.RailroadOperatorsLookup;

        public static List<RailroadOperatorsInformation> GetRailroadOperators(ICacheService cache, IMastersQueryable db)
        {
            List<RailroadOperatorsInformation> railroadOperators;
            if (!cache.TryGetValue(_key, out railroadOperators))
            {
                var query = new GetAllRailroadOperatorsQuery(db);
                railroadOperators = query.ExecuteQuery().ToList();
                cache.Set(_key, railroadOperators, DateTime.Now.AddDays(1));
            }

            return railroadOperators;
        }
    }
}
