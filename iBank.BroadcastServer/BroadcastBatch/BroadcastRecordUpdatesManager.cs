using AutoMapper;
using com.ciswired.libraries.CISLogger;
using Domain.Helper;
using Domain.Interfaces.BroadcastServer;
using Domain.Orm.iBankClientCommands;
using iBank.Server.Utilities.Logging;
using System;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Reflection;
using Domain;
using Domain.Orm.iBankMastersCommands.Broadcast;
using iBank.BroadcastServer.Helper;
using iBank.Entities.ClientEntities;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace iBank.BroadcastServer.BroadcastBatch
{
    public class BroadcastRecordUpdatesManager : IBroadcastRecordUpdatesManager
    {
        private static readonly IBroadcastLogger LOG = new BroadcastLogger(new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType));

        public BroadcastRecordUpdatesManager()
        {
            if (!Features.AutoMapperInitializer.IsEnabled())
            {
                Mapper.Initialize(cfg => cfg.CreateMap<ibbatch, ibbatchhistory>());
            }
        }

        public void ChangeBatchToRunning(bcstque4 batchToRun, int serverNumber, DateTime now)
        {
            batchToRun.svrstatus = BroadcastCriteria.Running;
            batchToRun.svrnumber = serverNumber;
            batchToRun.starttime = now;
        }

        public void ChangeBatchToDone(bcstque4 batchToRun, int serverNumber, DateTime now)
        {
            batchToRun.svrstatus = BroadcastCriteria.Done;
            batchToRun.svrnumber = serverNumber;
            batchToRun.starttime = now;
        }

        public void UpdateOfflineBatch(ibbatch clientBatch, bool batchOk, DateTime now)
        {
            clientBatch.batchname = batchOk
                    ? clientBatch.batchname.Replace("[RUN]", "[DONE]")
                    : clientBatch.batchname.Replace("[RUN]", "[ERROR]");

            clientBatch.lastrun = now;
            clientBatch.errflag = !batchOk;
        }

        public void UpdateNonOfflineBatch(ibbatch clientBatch, bool batchOk, IRecordTimingDetails broadcastTiming, DateTime now)
        {
            clientBatch.lastrun = now;
            clientBatch.errflag = !batchOk;
            clientBatch.lastdend = batchOk ? broadcastTiming.NextReportPeriodEnd : broadcastTiming.LastReportPeriodEnd;
            clientBatch.lastdstart = batchOk ? broadcastTiming.NextReportPeriodStart : broadcastTiming.LastReportPeriodStart;
        }

        public void SignifyThatTimeValuesSetBySystem(ibbatch broadcast, bool batchOk, bool runSpecial)
        {
            if (batchOk && !runSpecial) broadcast.setby = "S";
        }

        public void UpdateBroadcastInDataStore(IClientDataStore clientDataStore, ibbatch clientBatch)
        {
            var updateClientBatchCmd = new UpdateClientBroadcastBatchCommand(clientDataStore.ClientCommandDb, clientBatch);
            updateClientBatchCmd.ExecuteCommand();

            var historyHandler = new BatchHistoryHandler(clientDataStore);
            historyHandler.InsertHistoryRecord(clientBatch);
        }

    public bool UpdateQueueRecordInDataStore(ICommandDb masterCommandDb, bcstque4 batchToUpdate)
        {
            try
            {
                var updateBatchCmd = new UpdateBroadcastQueueRecordCommand(masterCommandDb, batchToUpdate);
                updateBatchCmd.ExecuteCommand();
                return true;
            }
            catch (DbUpdateConcurrencyException e)
            {
                if (e.InnerException is OptimisticConcurrencyException)
                {
                    LOG.DebugLogWithBatchInfo(batchToUpdate, "Attempt to update queue record failed.", e);
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
