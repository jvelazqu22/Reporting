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
    public static class MasterAgencySourcesLookup
    {
        private static readonly CacheKeys _key = CacheKeys.MasterAgencySourcesLookup;
        public static List<MasterSourceInformation> GetMasterAgencySources(ICacheService cache, IMastersQueryable db)
        {
            List<MasterSourceInformation> masterAgencySources;
            if (!cache.TryGetValue(_key, out masterAgencySources))
            {
                var query = new GetMasterAgencySourcesQuery(db);
                masterAgencySources = query.ExecuteQuery().ToList();
                cache.Set(_key, masterAgencySources, DateTime.Now.AddDays(1));
            }

            return masterAgencySources;
        }
    }
}
