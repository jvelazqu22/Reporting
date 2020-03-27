using com.ciswired.libraries.CISLogger;
using Domain.Models.BroadcastServer;
using iBank.BroadcastServer.BroadcastBatch;
using iBank.Server.Utilities.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Domain.Orm.iBankMastersCommands.Broadcast;
using Domain.Orm.iBankMastersQueries.BroadcastQueries;
using Domain.Orm.iBankMastersQueries.BroadcastQueries.BroadcastHistoryQueries;
using Domain.Services;
using iBank.Entities.MasterEntities;

namespace iBank.BroadcastServer.Service
{
    public class Producer
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly IBroadcastLogger BCST_LOG = new BroadcastLogger(new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType));

        public void LoadBatchesToExecute(MaintenanceModeState maintenanceModeState, Parameters p)
        {
            var cache = new CacheService();
            while (!p.IsMaintenanceModeRequested)
            {
                try
                {
                    var broadcastsToProcess = GetPoolEligibleBroadcasts(cache, p);

                    AddBroadcastsToPool(broadcastsToProcess, p);
                    OptimizeForGC(broadcastsToProcess);

                    SetMaintenanceMode(p, maintenanceModeState);
                    if (p.IsMaintenanceModeRequested) break;

                    //wait before checking the queue again
                    Thread.Sleep(2000);
                }
                catch (Exception ex)
                {
                    p.BatchesToExecute.CompleteAdding();
                    ErrorLogger.LogException(null, "", ex, MethodBase.GetCurrentMethod(), ServerType.BroadcastServer, LOG);
                    break;
                }
            }
        }

        private void OptimizeForGC(List<bcstque4> queue)
        {
            queue.Clear();
            queue.Capacity = 0;
            queue = null;
        }

        private List<bcstque4> GetPoolEligibleBroadcasts(ICacheService cache, Parameters p)
        {
            var queueBatchRetriever = new BroadcastRecordRetriever();
            var getPendingBroadcastsQuery = new GetPendingBroadcastQuery(p.MasterDataStore, p.UnilanguageCode,
                p.ServerConfiguration.ServerFunction, cache);
            var bcsts = queueBatchRetriever.GetPendingBroadcasts(p.ServerConfiguration.ServerFunction,
                getPendingBroadcastsQuery);
            return bcsts.ToList();
        }

        private void AddBroadcastsToPool(IList<bcstque4> broadcastsToProcess, Parameters p)
        {
            foreach (var bcst in broadcastsToProcess)
            {
                if (!p.BatchesToExecute.Any(x => x.batchnum == bcst.batchnum && x.agency.Equals(bcst.agency, StringComparison.OrdinalIgnoreCase)))
                {
                    p.BatchesToExecute.Add(bcst);
                    UpdateBroadcastHistoryRecord(bcst, p);
                    //give it a moment to get added to the pool
                    Thread.Sleep(100);
                }
            }
        }

        private void UpdateBroadcastHistoryRecord(bcstque4 bcst, Parameters p)
        {
            try
            {
                var recordToUpdate = new GetBroadcastHistoryMissingAddedToPoolQuery(p.MasterDataStore.MastersQueryDb, bcst).ExecuteQuery();
                if (recordToUpdate != null)
                {
                    recordToUpdate.added_to_pool = DateTime.Now;
                    var updateBroadcastHistoryCommand = new UpdateBroadcastHistoryRecordCommand(p.MasterDataStore.MastersCommandDb, recordToUpdate);
                    updateBroadcastHistoryCommand.ExecuteCommand();
                }
            }
            catch (Exception e)
            {
                LOG.Debug("Attempt to update broadcast history record failed.", e);
            }
        }

        private void SetMaintenanceMode(Parameters p, MaintenanceModeState state)
        {
            p.IsMaintenanceModeRequested = state.IsMaintenanceModeRequested();

            if (p.IsMaintenanceModeRequested)
            {
                LOG.Info("Maintenance mode requested. No longer adding batches.");
                p.BatchesToExecute.CompleteAdding();
            }
        }
    }
}
