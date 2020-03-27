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
    public static class CountriesLookup
    {
        private static readonly CacheKeys _key = CacheKeys.CountriesLookup;

        public static List<CountriesInformation> GetCountries(ICacheService cache, IMastersQueryable db)
        {
            List<CountriesInformation> countries;
            if (!cache.TryGetValue(_key, out countries))
            {
                var query = new GetAllCountriesQuery(db);
                countries = query.ExecuteQuery().ToList();
                cache.Set(_key, countries, DateTime.Now.AddDays(1));
            }

            return countries;
        }
        
    }
}
