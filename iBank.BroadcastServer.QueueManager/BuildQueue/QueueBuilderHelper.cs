using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Reflection;
using com.ciswired.libraries.CISLogger;
using Domain;
using Domain.Orm.iBankClientQueries;
using Domain.Orm.iBankMastersCommands.Broadcast;
using Domain.Orm.iBankMastersQueries.BroadcastQueries;
using Domain.Orm.iBankMastersQueries.Other;
using Domain.Services;
using iBank.BroadcastServer.QueueManager.Factories;
using iBank.Entities.ClientEntities;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBank.Repository.SQL.Repository;

namespace iBank.BroadcastServer.QueueManager.BuildQueue
{
    public class QueueBuilderHelper
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private IQueryable<broadcast_stage_agencies> _broadcastStageAgencies;
        private LongRunningService _longRunningService;
        private IMasterDataStore _masterDataStore;

        private IList<broadcast_stage_agencies> _broadcastStageAgenciesList;
        private Dictionary<string, int> _longRunningThreshold;
        private int _defaultThreshold;

        public QueueBuilderHelper(IQueryable<broadcast_stage_agencies> broadcastStageAgencies, LongRunningService longRunningService, IMasterDataStore masterDataStore)
        {
            _broadcastStageAgencies = broadcastStageAgencies;
            _longRunningService = longRunningService;
            _masterDataStore = masterDataStore;
        }

        public QueueBuilderHelper(IList<broadcast_stage_agencies> broadcastStageAgenciesList, Dictionary<string, int> longRunningThresholds, int defaultThreshold, IMasterDataStore masterDataStore)
        {
            _broadcastStageAgenciesList = broadcastStageAgenciesList;
            _longRunningThreshold = longRunningThresholds;
            _defaultThreshold = defaultThreshold;
            _masterDataStore = masterDataStore;
        }

        public bool IsTimeToRunBatchInTimeZone(int userNumber, ibbatch batch, IClientDataStore clientDataStore)
        {
            var user = new GetUserByUserNumberQuery(clientDataStore.ClientQueryDb, userNumber).ExecuteQuery();
            if (user == null) return false;

            if (user.TimeZone == "EST") return true;
            var getTimeZoneQuery = new GetUserTimeZoneByLangCodeQuery(new iBankMastersQueryable(), user.TimeZone, "EN");
            var zone = getTimeZoneQuery.ExecuteQuery();

            if (zone == null) return true;

            var userLocalTime = DateTime.Now.AddHours(zone.GMTDiff + 5);

            if (userLocalTime < batch.nextrun && !batch.runspcl)
            {
                LOG.Debug($"Not time to run batch in user [{userNumber}] local time zone of [{zone.TimeZoneCode}], user local time is [{userLocalTime}]");
                return false;
            }

            return true;
        }

        public bool IsBatchAlreadyAdded(int? batchnum, string databaseName)
        {
            var batchCheckQuery = new GetBroadcastRecordsByBatchNumberAndDatabaseQuery(_masterDataStore.MastersQueryDb, batchnum, databaseName);
            var batchCheck = batchCheckQuery.ExecuteQuery();

            return batchCheck.Any();
        }

        public void AddRecordToQueue(ibbatch batch, string databaseName)
        {
            var nextAvailableBroadcastKeyQuery = new GetNextAvailableBroadcastKeyQuery(_masterDataStore.MastersQueryDb);
            var nextAvailableBroadcastKey = nextAvailableBroadcastKeyQuery.ExecuteQuery();

            InsertNewBroadcastIntoQueueAndHistoryTables(batch, nextAvailableBroadcastKey, databaseName);
        }


        public void InsertNewBroadcastIntoQueueAndHistoryTables(ibbatch batch, int broadcastSequenceNumber, string databaseName)
        {
            var broadcast = InsertNewBroadcastIntoQueue(batch, broadcastSequenceNumber, databaseName);
            if (broadcast != null) InsertNewBroadcastIntoBroadcastHistory(broadcast);
        }

        public bcstque4 InsertNewBroadcastIntoQueue(ibbatch batch, int broadcastSequenceNumber, string databaseName)
        {
            var bcst = new bcstque4();

            var workshop= new BroadcastQueueRecordWorkshop(batch, broadcastSequenceNumber, databaseName, _broadcastStageAgenciesList, _longRunningThreshold, _defaultThreshold);
            bcst = workshop.Build();
            
            try
            {
                var addQueueRecordCmd = new AddBroadcastQueueRecordCommand(_masterDataStore.MastersCommandDb, bcst);
                addQueueRecordCmd.ExecuteCommand();
                LOG.Debug($"New record added to queue: batchnum [{bcst.batchnum}]");
                return bcst;
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
            

            return null;
        }

        public void InsertNewBroadcastIntoBroadcastHistory(bcstque4 broadcast)
        {
            try
            {
                var recordFactory = new BroadcastHistoryRecordFactory(broadcast);
                var broadcastHistory = recordFactory.Build();

                var addQueueRecordCmd = new AddBroadcastHistoryRecordCommand(_masterDataStore.MastersCommandDb, broadcastHistory);
                addQueueRecordCmd.ExecuteCommand();
                LOG.Debug($"New record added to the broadcast history: batchnum [{broadcastHistory.batchnum}]");
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
        }

    }
}
