using com.ciswired.libraries.CISLogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Domain.Orm.Classes;
using Domain.Orm.iBankMastersCommands.Broadcast;
using Domain.Orm.iBankMastersQueries.BroadcastQueueManager;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;

namespace iBank.BroadcastServer.QueueManager.Cleaner
{
    public class QueueCleaner
    {
        private readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        private IMasterDataStore _store;

        public QueueCleaner(IMasterDataStore store)
        {
            _store = store;
        }

        public void RemoveRecords(DateTime regularThreshold, DateTime longRunningThreshold, IList<BroadcastServerConfiguration> serverConfigs)
        {
            var broadcastsToRemove = GetOrphanedFinishedBroadcasts().ToList();
            
            var expiredRetrieval = new ExpiredRecordRetrieval(_store);
            var expiredRecords = expiredRetrieval.GetExpiredBroadcasts(regularThreshold, longRunningThreshold, serverConfigs);
            broadcastsToRemove.AddRange(expiredRecords);

            if (broadcastsToRemove.Count == 0) return;

            RemoveExpiredBroadcasts(broadcastsToRemove);
        }

        private IEnumerable<bcstque4> GetOrphanedFinishedBroadcasts()
        {
            //get broadcasts in error or that are not running and not pending
            var orphanQuery = new GetOrphanedBroadcastsQuery(_store.MastersQueryDb);
            return orphanQuery.ExecuteQuery();
        }
        
        private void RemoveExpiredBroadcasts(IList<bcstque4> broadcastsToRemove)
        {
            LOG.Debug($"Cleaning broadcast queue. Removing [{broadcastsToRemove.Count}] records.");
            var cmd = new RemoveBatchFromBroadcastQueueCommand(_store.MastersCommandDb, broadcastsToRemove);
            cmd.ExecuteCommand();
        }
    }
}
