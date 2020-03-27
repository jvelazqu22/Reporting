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
    public static class TripChangeInformationLookup
    {
        private static readonly CacheKeys _key = CacheKeys.TripChangeInformationLookup;

        public static List<TripChangeCodeInformation> GetTripChangeCodeInformation(ICacheService cache, IMastersQueryable db)
        {
            List<TripChangeCodeInformation> tripChangeInfo;
            if (!cache.TryGetValue(_key, out tripChangeInfo))
            {
                var query = new GetAllTripChangeCodesQuery(db);
                tripChangeInfo = query.ExecuteQuery().ToList();
                cache.Set(_key, tripChangeInfo, DateTime.Now.AddDays(1));
            }

            return tripChangeInfo;
        }
    }
}
