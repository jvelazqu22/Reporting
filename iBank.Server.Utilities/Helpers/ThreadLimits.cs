using System;
using System.Reflection;
using com.ciswired.libraries.CISLogger;
using Domain.Constants;
using Domain.Exceptions;
using Domain.Helper;
using Domain.Orm.iBankAdministrationQueries.ConfigurationQueries;
using Domain.Services;
using iBank.Repository.SQL.Interfaces;

namespace iBank.Server.Utilities.Helpers
{
    public static class ThreadLimits
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static int GetMaxConcurrentBroadcastsForServer(ICacheService cache, int serverNumber, IAdministrationQueryable administrationQueryable)
        {
            try
            {
                var configurationName = string.Format(Configurations.MAX_CONCURRENT_BROADCASTS_FOR_THIS_SERVER, serverNumber);
                var configNameCacheKey = GetServerNameCacheKey(configurationName);
                var serverConfiguration = new GetConfigurationQuery(cache, administrationQueryable, configNameCacheKey).ExecuteQuery();

                var configValue = serverConfiguration.Value.Trim();

                if (string.IsNullOrEmpty(configValue)) throw new InvalidDatabaseConfigurationException($"Invalid db config value: {configValue} for config name {configNameCacheKey}");
                if (!int.TryParse(configValue, out var threadCount)) throw new InvalidDatabaseConfigurationException($"Invalid db config value conversion from: {configValue} to integer for config name {configNameCacheKey} ");
                return threadCount;
            }
            catch(Exception ex)
            {
                LOG.Error(ex);
                return 3;
            }
        }

        public static int GetMaxConcurrentReportsForServer(ICacheService cache, int serverNumber, IAdministrationQueryable administrationQueryable)
        {
            try
            {
                var configurationName = string.Format(Configurations.MAX_CONCURRENT_REPORTS_FOR_THIS_SERVER, serverNumber);
                var configNameCacheKey = GetServerNameCacheKey(configurationName);
                var serverConfiguration = new GetConfigurationQuery(cache, administrationQueryable, configNameCacheKey).ExecuteQuery();

                var configValue = serverConfiguration.Value.Trim();

                if (string.IsNullOrEmpty(configValue)) throw new InvalidDatabaseConfigurationException($"Invalid db config value: {configValue} for config name {configNameCacheKey}");
                if (!int.TryParse(configValue, out var threadCount)) throw new InvalidDatabaseConfigurationException($"Invalid db config value conversion from: {configValue} to integer for config name {configNameCacheKey} ");

                return threadCount;

            }
            catch (Exception ex)
            {
                LOG.Error(ex);
                return 3;
            }
        }

        public static CacheKeys GetServerNameCacheKey(string serverName)
        {
            try
            {
                return (CacheKeys)Enum.Parse(typeof(CacheKeys), serverName);
            }
            catch (Exception)
            {
                throw new InvalidDatabaseConfigurationException($"ServerName {serverName} is not contained in the list of CacheKeys. Please fix the servername or add it to the CacheKeys");
            }
        }
    }
}
