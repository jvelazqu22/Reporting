using Domain.Extensions;
using Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodaTime;
using NodaTime.Extensions;
using iBank.Repository.SQL.Interfaces;
using Domain.Orm.iBankMastersQueries.BroadcastQueries.LongRunning;
using iBank.Entities.MasterEntities;
using Domain.Orm.iBankMastersCommands.LongRunning;
using com.ciswired.libraries.CISLogger;
using System.Reflection;

namespace Domain.Helper
{
    public class LongRunningThresholdsHelper
    {
        private readonly ICacheService _cache;
        private readonly CacheKeys _key = CacheKeys.BroadcastLongRunningThreshold;
        private readonly int _cacheExpirationInSec = 60;
        private readonly IMasterDataStore _store;

        private readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public LongRunningThresholdsHelper(IMasterDataStore store, ICacheService cache)
        {
            _cache = cache;
            _store = store;
        }
        public virtual DateTime GetNow() => DateTime.Now;

        public int GetAgencyDateRangeLimit(string agency)
        {
            var thresholds = GetThresholds();
            if (!thresholds.TryGetValue(agency, out var rangeLimit))
            {
                rangeLimit = GetDefaultThreshold(thresholds);
            }

            return rangeLimit;
        }

        public Dictionary<string, int> GetThresholds()
        {
            if (!_cache.TryGetValue(_key, out Dictionary<string, int> thresholds))
            {
                //this call can be contained in an outer scope so we need a new scope for this call
                using (var scope = TransactionScopeBuilder.BuildNoLockScope())
                {
                    try
                    {
                        var query = new GetLongRunningThresholdsQuery(_store.MastersQueryDb);
                        thresholds = query.ExecuteQuery();
                    }
                    finally
                    {
                        scope.Complete();
                    }
                }

                _cache.Set(_key, thresholds, GetNow().AddSeconds(_cacheExpirationInSec));
            }

            return thresholds;
        }

        public int GetDefaultThreshold(Dictionary<string, int> thresholds)
        {
            var defaultAgencyName = Constants.DefaultFontPlaceholder;
            if (!thresholds.TryGetValue(defaultAgencyName, out var rangeLimit))
            {
                var defaultThreshold = 12;
                rangeLimit = defaultThreshold;
                AddDefaultThresholdToDb(defaultAgencyName, defaultThreshold);
                thresholds.Add(defaultAgencyName, defaultThreshold);
                _cache.Set(_key, thresholds, GetNow().AddSeconds(_cacheExpirationInSec));
            }

            return rangeLimit;
        }

        private void AddDefaultThresholdToDb(string defaultAgencyName, int defaultThresholdValue)
        {
            var rec = new BroadcastLongRunningThreshold
            {
                Agency = defaultAgencyName,
                MonthsInRangeThreshold = defaultThresholdValue
            };

            try
            {
                var cmd = new AddLongRunningThresholdCommand(_store.MastersCommandDb, rec);
                cmd.ExecuteCommand();
            }
            catch (Exception ex)
            {
                //log and swallow
                LOG.Warn("Exception encountered attempting to add default long run threshold", ex);
            }
        }

    }
}
