using CODE.Framework.Core.Utilities.Extensions;
using com.ciswired.libraries.CISLogger;
using Domain;
using Domain.Helper;
using Domain.Orm.Classes;
using Domain.Orm.iBankAdministrationQueries;
using Domain.Orm.iBankMastersQueries.BroadcastQueries;
using Domain.Services;
using iBank.BroadcastServer.QueueManager.BuildQueue;
using iBank.BroadcastServer.QueueManager.Cleaner;
using iBank.Repository.SQL.Interfaces;
using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities.Retrievers;
using iBankDomain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;

namespace iBank.BroadcastServer.QueueManager
{
    public class QueueManagementServer
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public void BuildQueue(ICacheService cache)
        {
            var getAgencyDatabases = new GetActiveBroadcastDatabasesQuery(new iBankMastersQueryable());
            var getMstrCorpDatabases = new GetMasterCorpAcctsBroadcastDatabasesQuery(new iBankMastersQueryable());
            var databaseQueries = new List<IQuery<IList<DatabaseInformation>>> { getAgencyDatabases, getMstrCorpDatabases };
            var databasesToBuildFrom = GetDatabases(databaseQueries);

            var agencyInformationRetriever = new AgencyInformationRetriever();
            var getActiveBroadcastAgenciesQuery = new GetActiveBroadcastAgenciesQuery(new iBankMastersQueryable());
            var getMasterCorpActiveBroadcastAgenciesQuery = new GetMasterCorpActiveBroadcastAgenciesQuery(new iBankMastersQueryable());
            var agencyQueries = new List<IQuery<IList<AgencyInformation>>> { getActiveBroadcastAgenciesQuery, getMasterCorpActiveBroadcastAgenciesQuery };
            var agencies = agencyInformationRetriever.GetAgencies(agencyQueries);

            var masterStore = new MasterDataStore();
            var broadcastStageAgencies = masterStore.MastersQueryDb.BroadcastStageAgencies;

            //ToList so I don't have to run it on each bcstque4 record
            var broadcastStageAgenciesList = broadcastStageAgencies.ToList();

            //Get long-running threshold list 
            var longRunningThresholdsHelper = new LongRunningThresholdsHelper(masterStore, cache);
            var longRunningThresholds = longRunningThresholdsHelper.GetThresholds();
            var defaultThreshold = longRunningThresholdsHelper.GetDefaultThreshold(longRunningThresholds);

            var dbs = databasesToBuildFrom.Aggregate("", (current, d) => current + (d.DatabaseName + ", "));
            LOG.Debug($"Building queue from [{databasesToBuildFrom.Count}] databases: [{dbs}]");
            foreach (var database in databasesToBuildFrom)
            {
                try
                {
                    LOG.Debug($"Building queue from database [{database.DatabaseName}]");
                    QueueBuilder builder = new QueueBuilder(masterStore, database, agencies, broadcastStageAgenciesList, longRunningThresholds, defaultThreshold);
                    builder.BuildQueue();
                }
                catch (Exception e)
                {
                    LOG.Error($"Exception encountered while building queue from db [{database.DatabaseName}], time zone offset [{database.TimeZoneOffset}]", e);
                }
            }
        }

        public void CleanQueue()
        {
            
            var thresholdInMin = ConfigurationManager.AppSettings["RemoveRunningRowsAfterXMinutes"].TryIntParse(240);
            var longRunningThresholdInMin = 60 * 6;

#if GSA_BUILD
            var daysToKeepRunningRow = 7;
            thresholdInMin = (60 * 24) * daysToKeepRunningRow;
            longRunningThresholdInMin = (60 * 24) * daysToKeepRunningRow;
#endif
            var now = DateTime.Now;
            var threshold = now.AddMinutes(-thresholdInMin);
            var longRunningThreshold = now.AddMinutes(-longRunningThresholdInMin);

            var serverConfigs = GetConfigurations(new iBankAdministrationQueryable());

            var cleaner = new QueueCleaner(new MasterDataStore());
            cleaner.RemoveRecords(threshold, longRunningThreshold, serverConfigs);
        }

        private IList<BroadcastServerConfiguration> GetConfigurations(IAdministrationQueryable db)
        {
            var query = new GetBroadcastServerConfigurationQuery(db);
            return query.ExecuteQuery();
        }

        private IList<DatabaseInformation> GetDatabases(IList<IQuery<IList<DatabaseInformation>>> getDatabaseInformationQueries)
        {
            var serverInformation = new DatabaseInformationRetriever();
            return serverInformation.GetDatabases(getDatabaseInformationQueries);
        }
    }
}
