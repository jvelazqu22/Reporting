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
    public static class CarLookup
    {
        private static readonly CacheKeys _key = CacheKeys.CarTypeLookup;
        public static List<CarTypeInfo> GetCarTypes(ICacheService cache, IMastersQueryable db)
        {
            List<CarTypeInfo> cars;
            if (!cache.TryGetValue(_key, out cars))
            {
                var query = new GetAllCarTypesQuery(db);
                cars = query.ExecuteQuery().ToList();

                cache.Set(_key, cars, DateTime.Now.AddDays(1));
            }

            return cars;
        }
    }
}
