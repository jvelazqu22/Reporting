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
    public static class DomesticInternationalLookup
    {
        private static readonly CacheKeys _key = CacheKeys.DomesticInternationLookup;

        public static List<KeyValue> GetDomesticInternational(ICacheService cache, IMastersQueryable db, string languageCode)
        {
            if (string.IsNullOrEmpty(languageCode)) languageCode = "EN";

            Dictionary<string, List<KeyValue>> domesticInternationDict;
            List<KeyValue> domesticInternational;

            if (!cache.TryGetValue(_key, out domesticInternationDict))
            {
                //dictionary not in cache, get list, add to dictionary, add dictionary to the cache
                domesticInternational = GetValues(db, languageCode);
                domesticInternationDict = new Dictionary<string, List<KeyValue>> { [languageCode] = domesticInternational };
                cache.Set(_key, domesticInternationDict, DateTime.Now.AddDays(1));
                return domesticInternational;
            }
            
            if (!domesticInternationDict.TryGetValue(languageCode, out domesticInternational))
            {
                //dictionary is in cache, but that language is not, add to dict, overwrite existing cache entry
                domesticInternational = GetValues(db, languageCode);
                domesticInternationDict[languageCode] = domesticInternational;
                cache.Set(_key, domesticInternationDict, DateTime.Now.AddDays(1));
            }
            
            return domesticInternational;
        }

        private static List<KeyValue> GetValues(IMastersQueryable db, string languageCode)
        {
            var query = new GetDomesticInternationalQuery(db, languageCode);
            return query.ExecuteQuery().ToList();
        }
    }
}
