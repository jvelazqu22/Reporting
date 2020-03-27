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
    public static class MetrosLookup
    {
        private static readonly CacheKeys _key = CacheKeys.MetrosLookup;

        public static List<MetroInformation> GetMetros(ICacheService cache, IMastersQueryable db)
        {
            List<MetroInformation> metros;
            if (!cache.TryGetValue(_key, out metros))
            {
                var query = new GetAllMetrosQuery(db);
                metros = query.ExecuteQuery().ToList();
                cache.Set(_key, metros, DateTime.Now.AddDays(1));
            }

            return metros;
        }
    }
}
