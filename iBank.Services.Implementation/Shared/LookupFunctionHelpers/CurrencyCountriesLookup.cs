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
    public static class CurrencyCountriesLookup
    {
        private static readonly CacheKeys _key = CacheKeys.CurrencyCountriesLookup;

        public static List<CurrencyCountry> GetCurrencyCountries(ICacheService cache, IMastersQueryable db)
        {
            List<CurrencyCountry> currencyCountries;
            if (!cache.TryGetValue(_key, out currencyCountries))
            {
                var query = new GetAllCurrencyCountriesQuery(db);
                currencyCountries = query.ExecuteQuery().ToList();
                cache.Set(_key, currencyCountries, DateTime.Now.AddDays(1));
            }

            return currencyCountries;
        }
    }
}
