using com.ciswired.libraries.CISLogger;
using Domain.Helper;
using Domain.Interfaces.BroadcastServer;
using Domain.Models.BroadcastServer;
using iBank.Server.Utilities.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using Domain.Services;
using iBank.BroadcastServer.Helper;
using iBank.Repository.SQL.Interfaces;

namespace iBank.BroadcastServer.BroadcastBatch
{

    public class BatchRunner : IBatchRunner
    {
        private static readonly IBroadcastLogger BCST_LOG = new BroadcastLogger(new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType));

        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IBroadcastQueueRecordRemover _queueRemover;

        private readonly IBatchManager _batchManager;

        private readonly IBroadcastReportProcessor _reportProcessor;
        
        private IMasterDataStore MasterDataStore { get; set; }
        private IClientDataStore ClientDataStore { get; set; }
        private readonly IBroadcastRecordUpdatesManager _broadcastRecordUpdatesManager;

        public BatchRunner(IBroadcastQueueRecordRemover queueRemover, IBatchManager batchManager, IBroadcastReportProcessor reportProcessor,
            IMasterDataStore masterDataStore, IClientDataStore clientDataStore, IBroadcastRecordUpdatesManager broadcastRecordUpdatesManager)
        {
            _queueRemover = queueRemover;
            _batchManager = batchManager;
            _reportProcessor = reportProcessor;
            MasterDataStore = masterDataStore;
            ClientDataStore = clientDataStore;
            _broadcastRecordUpdatesManager = broadcastRecordUpdatesManager;
        }

        public void ExecuteBatch(ICacheService cache, IList<BroadcastReportInformation> allReportsInBatch, IRecordTimingDetails recordTimingDetails, 
            BroadcastServerInformation serverConfig, bool runSpecial, bool isOfflineBatch)
        {
            recordTimingDetails.LastRun = DateTime.Now;
            BCST_LOG.InfoLogWithBatchInfo(_batchManager.QueuedRecord, "Starting run of broadcast.");
            var batchOk = _reportProcessor.RunAllReports(cache, allReportsInBatch, recordTimingDetails, serverConfig, isOfflineBatch,
                    _batchManager.UserManager.User, runSpecial, _batchManager.QueuedRecord);
            BCST_LOG.InfoLogWithBatchInfo(_batchManager.QueuedRecord, $"Broadcast done processing. Batch OK = [{batchOk}].");

            if (!batchOk)
            {
                //if applicable, notify a high alert agency
                var highAlertHandler = new HighAlertHandler();
                highAlertHandler.NotificationIfHighAlertAgency(MasterDataStore, _batchManager.QueuedRecord, true);

                //if applicable send error email 
                if (_batchManager.QueuedRecord.send_error_email)
                {
                    var errorHandler = new ErrorHandler();
                    errorHandler.SendErrorEmail(LOG, _batchManager.QueuedRecord, 0, MasterDataStore, serverConfig);
                }
            }
            
            var originalBatch = _batchManager.RecordRetriever.GetClientBroadcastRecord(ClientDataStore.ClientQueryDb, _batchManager.QueuedRecord.batchnum);

            if (originalBatch == null) throw new Exception($"Original batch, # [{_batchManager.QueuedRecord.batchnum}] cannot be found.");
                
            _batchManager.UpdateFinishedOriginalClientBatchInformation(originalBatch, isOfflineBatch, batchOk, recordTimingDetails);
            BCST_LOG.DebugLogWithBatchInfo(_batchManager.QueuedRecord, "ibbatch record for broadcast updated.");

            SetRecordToComplete(serverConfig);
        }

        private void SetRecordToComplete(BroadcastServerInformation serverConfig)
        {
            _broadcastRecordUpdatesManager.ChangeBatchToDone(_batchManager.QueuedRecord, serverConfig.ServerNumber, DateTime.Now);
            var wasRecUpdated = _broadcastRecordUpdatesManager.UpdateQueueRecordInDataStore(MasterDataStore.MastersCommandDb,_batchManager.QueuedRecord);

            // wasRecUpdated should always be true since no processes are competing to update it.
            // there should only be one process trying to update this records. 
            BCST_LOG.DebugLogWithBatchInfo(_batchManager.QueuedRecord, 
                wasRecUpdated
                    ? $"[{serverConfig.ServerFunction}] - Set status to done for broadcast in queue."
                    : $"[{serverConfig.ServerFunction}] - Abandoning, another server has already set broadcast to done.");
        }
    }
}
