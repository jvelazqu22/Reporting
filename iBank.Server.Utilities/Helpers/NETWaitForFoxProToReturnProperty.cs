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
    public static class NETWaitForFoxProToReturnProperty
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static int GetMinutesToWaitForFoxProToReturnForTheServer(ICacheService cache, int serverNumber, IAdministrationQueryable administrationQueryable)
        {

            try
            {
                var configurationName = string.Format(Configurations.MINUTES_TO_WAIT_FOR_FOXPRO_TO_RETURN, serverNumber);
                var configCacheKey = GetServerCacheKey(configurationName);
                var serverConfiguration = new GetConfigurationQuery(cache, administrationQueryable, configCacheKey).ExecuteQuery();

                var configValue = serverConfiguration.Value.Trim();

                if (string.IsNullOrEmpty(configValue)) throw new InvalidDatabaseConfigurationException($"Invalid db config value: {configValue} for config name {configCacheKey}");
                if (!int.TryParse(configValue, out var minutesToWaitforFoxproToReturn)) throw new InvalidDatabaseConfigurationException($"Invalid db config value conversion from: {configValue} to integer for config name {configCacheKey} ");
                return minutesToWaitforFoxproToReturn;
            }
            catch (Exception ex)
            {
                LOG.Error(ex);
                return 180;
            }
        }

        public static CacheKeys GetServerCacheKey(string configurationName)
        {
            try
            {
                return (CacheKeys)Enum.Parse(typeof(CacheKeys), configurationName);
            }
            catch (Exception)
            {
                throw new InvalidDatabaseConfigurationException($"Configuration Name {configurationName} is not contained in the list of CacheKeys. Please fix the configuration name or add it to the CacheKeys");
            }
        }
    }
    
}
