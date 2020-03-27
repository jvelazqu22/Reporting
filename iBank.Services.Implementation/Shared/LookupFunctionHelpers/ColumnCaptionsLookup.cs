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
    public static class ColumnCaptionsLookup
    {
        private static readonly CacheKeys _key = CacheKeys.ColumnCaptionsLookup;

        public static List<KeyValue> GetColumnCaptions(ICacheService cache, IMasterDataStore store, string languageCode)
        {
            if (string.IsNullOrEmpty(languageCode)) languageCode = "EN";

            Dictionary<string, List<KeyValue>> columnCaptionDict;
            List<KeyValue> columnCaptions;

            if (!cache.TryGetValue(_key, out columnCaptionDict))
            {
                //dictionary not in cache, get list, add to dictionary, add dictionary to the cache
                columnCaptions = GetValues(store.MastersQueryDb, languageCode);
                columnCaptionDict = new Dictionary<string, List<KeyValue>> { [languageCode] = columnCaptions };
                cache.Set(_key, columnCaptionDict, DateTime.Now.AddDays(1));
                return columnCaptions;
            }

            if (!columnCaptionDict.TryGetValue(languageCode, out columnCaptions))
            {
                //dictionary is in cache, but that language is not, add to dict, overwrite existing cache entry
                columnCaptions = GetValues(store.MastersQueryDb, languageCode);
                columnCaptionDict[languageCode] = columnCaptions;
                cache.Set(_key, columnCaptionDict, DateTime.Now.AddDays(1));
            }

            return columnCaptions;
        }

        private static List<KeyValue> GetValues(IMastersQueryable db, string languageCode)
        {
            var query = new GetTranslationsByLanguageCodeQuery(db, languageCode);
            return query.ExecuteQuery().ToList();
        }
    }
}
