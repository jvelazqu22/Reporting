using com.ciswired.libraries.CISLogger;
using Domain.Exceptions;
using Domain.Orm.Classes;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Logging;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Reflection;
using Domain.Services;
using iBank.Entities.ClientEntities;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBank.Repository.SQL.Repository;

namespace iBank.BroadcastServer.QueueManager.BuildQueue
{
    public class QueueBuilder
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public DatabaseInformation Database { get; set; }
        public IList<AgencyInformation> Agencies { get; set; }

        private IMasterDataStore _masterDataStore;
        private IQueryable<broadcast_stage_agencies> _broadcastStageAgencies;
        private LongRunningService _longRunningService;
        private QueueBuilderHelper _queueBuilderHelper;

        private IList<broadcast_stage_agencies> _broadcastStageAgenciesList;
        private Dictionary<string, int> _longRunningThresholds;


        public QueueBuilder(IMasterDataStore masterDataStore, DatabaseInformation database, IList<AgencyInformation> agencies, 
            IQueryable<broadcast_stage_agencies> broadcastStageAgencies, ICacheService cache)
        {
            _masterDataStore = masterDataStore;
            Database = database;
            Agencies = agencies;
            _broadcastStageAgencies = broadcastStageAgencies;
            _longRunningService = new LongRunningService(cache, masterDataStore);
            _queueBuilderHelper = new QueueBuilderHelper(_broadcastStageAgencies, _longRunningService, _masterDataStore);
        }

        public QueueBuilder(IMasterDataStore masterDataStore, DatabaseInformation database, IList<AgencyInformation> agencies,
            List<broadcast_stage_agencies> broadcastStageAgenciesList, Dictionary<string, int> longRunningThresholds, int defaultThreshold)
        {
            _masterDataStore = masterDataStore;
            Database = database;
            Agencies = agencies;
            _broadcastStageAgenciesList = broadcastStageAgenciesList;
            _longRunningThresholds = longRunningThresholds;
            _queueBuilderHelper = new QueueBuilderHelper(broadcastStageAgenciesList, longRunningThresholds, defaultThreshold, masterDataStore);
        }


        public void BuildQueue()
        {
            var cycleDateTime = DateTime.Now;
            var databaseName = Database.DatabaseName;

            var serverName = "";
            try
            {
                serverName = iBankServerInformationRetrieval.GetServerName(databaseName);
            }
            catch (ServerAddressNotFoundException ex)
            {
                LOG.Warn(ex.Message, ex);
                return;
            }

            var cycleTimeZone = GetCycleTimeZone(Database.TimeZoneOffset, cycleDateTime);
            LOG.Debug($"Using [{cycleTimeZone}] as threshold time.");

            var clientDataStore = new ClientDataStore(serverName, Database.DatabaseName);

            LOG.Debug($"Working server [{serverName}], database [{Database.DatabaseName}]");
            //get the agencies that match up
            var agenciesInDb = GetAgenciesInCurrentDatabase(Agencies, Database);
            
            //get the batches for those agencies
            var batches = GetClientBroadcastBatches(cycleDateTime, agenciesInDb, clientDataStore).ToList();


            LOG.Debug($"Retrieved [{batches.Count}] batches initially.");
            //remove batches were the email address is empty
            var numberWithEmptyEmails = batches.RemoveAll(x => string.IsNullOrWhiteSpace(x.emailaddr));

            if (numberWithEmptyEmails > 0)
            {
                LOG.Debug($"Did not queue [{numberWithEmptyEmails}] batches due to empty email addresses; processed on db [{Database.DatabaseName}].");
            }
            

            var offlineStuff = batches.RemoveAll(x => BroadcastBatchTypeConditionals.IsBatchRunOffline(x.batchname)
                                        && (BroadcastBatchTypeConditionals.IsBatchPending(x.batchname)
                                            || BroadcastBatchTypeConditionals.IsBatchDone(x.batchname)
                                            || BroadcastBatchTypeConditionals.IsBatchInError(x.batchname)));
            if (offlineStuff > 0)
            {
                LOG.Debug($"Did not queue [{offlineStuff}] offline batches that were pending, done or in error on db [{Database.DatabaseName}]");
            }

            LOG.Debug($"[{batches.Count}] batches to process.");
            
            //process those batches
            foreach (var batch in batches)
            {
                try
                {
                    LOG.Debug($"Processing batchname [{batch.batchname}], database [{databaseName}]");
                    ProcessBatch(batch, databaseName, clientDataStore);
                }
                catch (Exception e)
                {
                    LOG.Error($"Exception encountered working batch [{batch.batchname}] from db [{Database.DatabaseName}]: [{e}]", e);
                }
            }
        }

        private IList<string> GetAgenciesInCurrentDatabase(IEnumerable<AgencyInformation> agencies, DatabaseInformation database)
        {
            return agencies.Where(a => a.Active
                                        && a.BcActive
                                        && a.DatabaseName.Trim().Equals(database.DatabaseName, StringComparison.OrdinalIgnoreCase)
                                        && a.TimeZoneOffset == database.TimeZoneOffset)
                                       .Select(x => x.Agency.Trim().ToUpper())
                                       .ToList();
        }

        private DateTime GetCycleTimeZone(int timeZoneOffSet, DateTime cycleDateTime)
        {
            var offsetInSeconds = 60 * 60 * timeZoneOffSet;

            return cycleDateTime.AddSeconds(offsetInSeconds);
        }

        private IList<ibbatch> GetClientBroadcastBatches(DateTime cycleTimeZone, IList<string> agencyNames, IClientDataStore clientDataStore)
        {
            var clientBatches = new List<ibbatch>();
            var retriever = new ClientBroadcastsRetriever(clientDataStore, cycleTimeZone, agencyNames);

            clientBatches = retriever.GetAllClientBatches();

            return clientBatches;
        }

        private void ProcessBatch(ibbatch batch, string databaseName, IClientDataStore clientDataStore)
        {
            if (_queueBuilderHelper.IsBatchAlreadyAdded(batch.batchnum, databaseName)) return;

            if (!_queueBuilderHelper.IsTimeToRunBatchInTimeZone(batch.UserNumber, batch, clientDataStore)) return;

            try
            {
                _queueBuilderHelper.AddRecordToQueue(batch, databaseName);
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        var msg = $"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}";
                        LOG.Error(msg, dbEx);
                    }
                }
            }
            catch (Exception ex)
            {
                var msg = $"Exception encountered adding batch [{batch.batchname}] from db [{databaseName}]: [{ex.Message}]";
                ErrorLogger.LogException(batch.UserNumber, batch.agency, ex, msg, MethodBase.GetCurrentMethod(), ServerType.BroadcastQueueManager, LOG);
            }
        }
    }
}
