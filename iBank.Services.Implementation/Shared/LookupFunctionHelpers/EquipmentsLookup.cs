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
    public static class EquipmentsLookup
    {
        private static readonly CacheKeys _key = CacheKeys.EquipmentLookup;

        public static List<KeyValue> GetEquipment(ICacheService cache, IMastersQueryable db)
        {
            List<KeyValue> equipment;
            if (!cache.TryGetValue(_key, out equipment))
            {
                var query = new GetEquipmentQuery(db);
                equipment = query.ExecuteQuery().ToList();
                cache.Set(_key, equipment, DateTime.Now.AddDays(1));
            }

            return equipment;
        }
    }
}
