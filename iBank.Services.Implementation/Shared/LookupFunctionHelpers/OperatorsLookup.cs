using System;
using System.Collections.Generic;
using System.Linq;

using Domain.Helper;
using Domain.Orm.Classes;
using Domain.Orm.iBankMastersQueries;
using Domain.Services;
using iBank.Repository.SQL.Interfaces;

namespace iBank.Services.Implementation.Shared.LookupFunctionHelpers
{
    public static class OperatorsLookup
    {
        private static readonly CacheKeys _key = CacheKeys.OperatorsLookup;

        public static List<KeyValue> GetOperators(ICacheService cache, IMastersQueryable db)
        {
            List<KeyValue> operators;
            if (!cache.TryGetValue(_key, out operators))
            {
                var query = new GetOperatorsQuery(db);
                operators = query.ExecuteQuery().ToList();
                cache.Set(_key, operators, DateTime.Now.AddDays(1));
            }

            return operators;
        }
    }
}
