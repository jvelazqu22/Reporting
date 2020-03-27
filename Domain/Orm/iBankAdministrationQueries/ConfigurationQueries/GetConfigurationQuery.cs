using System;
using System.Linq;
using Domain.Exceptions;
using Domain.Helper;
using Domain.Services;
using iBank.Entities.AdministrationEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankAdministrationQueries.ConfigurationQueries
{
    public class GetConfigurationQuery : IQuery<ReportingConfiguration>
    {
        private CacheKeys _configNameCacheKey;
        private readonly ICacheService _cache;
        private readonly IAdministrationQueryable _db;
        private static readonly int _cacheExpirationInSec = 60;
        public GetConfigurationQuery(ICacheService cache, IAdministrationQueryable db, CacheKeys configNameCacheKey)
        {
            _cache = cache;
            _db = db;
            _configNameCacheKey = configNameCacheKey;
        }

        public ReportingConfiguration ExecuteQuery()
        {
            if (!_cache.TryGetValue(_configNameCacheKey, out ReportingConfiguration config))
            {
                config = _db.ReportingConfiguration.FirstOrDefault(x => x.Name.Trim().Equals(_configNameCacheKey.ToString(), StringComparison.OrdinalIgnoreCase));
                if (config == null) throw new InvalidDatabaseConfigurationException("Missing " + _configNameCacheKey + " from the Configurations database");

                _cache.Set(_configNameCacheKey, config, DateTime.Now.AddSeconds(_cacheExpirationInSec));
            }
            _db.Dispose();

            return config;
        }
    }
}
