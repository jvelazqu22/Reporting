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
    public static class ChainsLookup
    {
        private static readonly CacheKeys _key = CacheKeys.ChainsLookup;
        public static List<ChainsInformation> GetChains(ICacheService cache, IMastersQueryable db)
        {
            List<ChainsInformation> rooms;
            if (!cache.TryGetValue(_key, out rooms))
            {
                var query = new GetAllChainsQuery(db);
                rooms = query.ExecuteQuery().ToList();
                cache.Set(_key, rooms, DateTime.Now.AddDays(1));
            }

            return rooms;
        }
    }
}
