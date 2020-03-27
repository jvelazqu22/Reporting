using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Extensions;
using Domain.Helper;
using Domain.Orm.Classes;
using Domain.Orm.iBankMastersQueries.BroadcastQueueManager;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;

namespace iBank.BroadcastServer.QueueManager.Cleaner
{
    public class ExpiredRecordRetrieval
    {
        private readonly IMasterDataStore _store;

        public ExpiredRecordRetrieval(IMasterDataStore store)
        {
            _store = store;
        }
        
        public IList<bcstque4> GetExpiredBroadcasts(DateTime regularThreshold, DateTime longRunningThreshold, 
            IList<BroadcastServerConfiguration> serverConfigs)
        {
            var broadcastsToRemove = new List<bcstque4>();
            var expiredRunningBroadcasts = GetPossiblyExpiredRunningBroadcasts(regularThreshold).ToList();

            //remove broadcasts where the server number is set to 0
            var serverZeroBroadcasts = expiredRunningBroadcasts.Where(x => x.svrnumber == 0).ToList();
            broadcastsToRemove.AddRange(serverZeroBroadcasts);
            expiredRunningBroadcasts = expiredRunningBroadcasts.Except(serverZeroBroadcasts).ToList();

            //remove broadcasts that are running on a long running server
            var longRunningServers = serverConfigs.GetCorrespondingServerNumbers(BroadcastServerFunction.LongRunning);
            var expiredLongRunning = expiredRunningBroadcasts.Where(x => longRunningServers.Contains(x.svrnumber))
                                                             .Where(x => x.starttime <= longRunningThreshold);
            broadcastsToRemove.AddRange(expiredLongRunning);
            expiredRunningBroadcasts = expiredRunningBroadcasts.Where(x => !longRunningServers.Contains(x.svrnumber)).ToList();

            broadcastsToRemove.AddRange(expiredRunningBroadcasts);

            return broadcastsToRemove;
        }

        private IEnumerable<bcstque4> GetPossiblyExpiredRunningBroadcasts(DateTime regularThreshold)
        {
            var runningBcstsQuery = new GetMinThresholdRunningBroadcastsQuery(_store.MastersQueryDb, regularThreshold);
            return runningBcstsQuery.ExecuteQuery();
        }
    }
}
