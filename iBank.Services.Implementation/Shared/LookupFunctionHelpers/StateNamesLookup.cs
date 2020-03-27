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
    public static class StateNamesLookup
    {
        private static readonly CacheKeys _key = CacheKeys.StateNamesLookup;

        public static List<state_names> GetStatesNames(ICacheService cache, IMastersQueryable db)
        {
            List<state_names> stateNames;
            if (!cache.TryGetValue(_key, out stateNames))
            {
                var query = new GetAllStateNamesAndAbbreviationsQuery(db);
                stateNames = query.ExecuteQuery().ToList();
                cache.Set(_key, stateNames, DateTime.Now.AddDays(1));
            }

            return stateNames;
        }
    }
}
