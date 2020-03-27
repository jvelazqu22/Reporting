using System;
using System.Collections.Generic;
using Domain.Helper;
using Domain.Orm.iBankMastersQueries.Other;
using Domain.Services;
using iBank.Repository.SQL.Interfaces;

namespace iBank.Server.Utilities
{
    public static class WhereCriteriaLookup
    {
        private static readonly CacheKeys _key = CacheKeys.WhereCriteriaLookup;

        public static Dictionary<int, string> GetWhereCriteriaLookup(ICacheService cache, IMasterDataStore store)
        {
            if (!cache.TryGetValue(_key, out Dictionary<int, string> whereCriteriaLookup))
            {
                var query = new GetAllWhereCriteriaQuery(store.MastersQueryDb);
                whereCriteriaLookup = query.ExecuteQuery();
                cache.Set(_key, whereCriteriaLookup, DateTime.Now.AddHours(1));
            }

            return whereCriteriaLookup;
        }
    }
}
