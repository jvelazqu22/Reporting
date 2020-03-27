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
    public static class HomeCountriesLookup
    {
        private static readonly CacheKeys _key = CacheKeys.HomeCountriesLookup;

        public static List<KeyValue> GetHomeCountries(ICacheService cache, IMastersQueryable db, ClientType clientType, string agency)
        {
            Dictionary<string, List<KeyValue>> homeCountriesDict;
            List<KeyValue> homeCountries;
            agency = agency.Trim();

            if (!cache.TryGetValue(_key, out homeCountriesDict))
            {
                //dictionary not in cache
                homeCountries = GetHomeCountryValues(db, clientType, agency);
                homeCountriesDict = new Dictionary<string, List<KeyValue>> { [agency] = homeCountries };
                cache.Set(_key, homeCountriesDict, DateTime.Now.AddDays(1));
                return homeCountries;
            }

            if (!homeCountriesDict.TryGetValue(agency, out homeCountries))
            {
                //dictionary is in cache, but no entry for agency
                homeCountries = GetHomeCountryValues(db, clientType, agency);
                homeCountriesDict[agency] = homeCountries;
                cache.Set(_key, homeCountriesDict, DateTime.Now.AddDays(1));
            }

            return homeCountries;
        }

        private static List<KeyValue> GetHomeCountryValues(IMastersQueryable db, ClientType clientType, string agency)
        {
            if (clientType == ClientType.Sharer)
            {
                var query = new GetHomeCountriesByAgencyForSharerClientQuery(db, agency);
                return query.ExecuteQuery().ToList();
            }
            else
            {
                var query = new GetHomeCountriesByAgencyForNonSharerClientQuery(db, agency);
                return query.ExecuteQuery().ToList();
            }
        }
    }
}
