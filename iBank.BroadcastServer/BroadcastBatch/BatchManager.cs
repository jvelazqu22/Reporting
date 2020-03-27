using com.ciswired.libraries.CISLogger;
using Domain.Interfaces.BroadcastServer;

using iBank.BroadcastServer.Timing;
using iBank.BroadcastServer.User;
using iBank.Server.Utilities.Logging;

using System;
using System.Reflection;

using Domain.Orm.iBankClientQueries;
using Domain.Orm.iBankMastersQueries;
using Domain.Orm.iBankMastersQueries.Agency;
using Domain.Orm.iBankMastersQueries.Other;
using iBank.Entities.ClientEntities;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;

namespace iBank.BroadcastServer.BroadcastBatch
{
    public class BatchManager : IBatchManager
    {
        private static readonly IBroadcastLogger LOG = new BroadcastLogger(new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType));
        private IMasterDataStore MasterDataStore { get; set; }
        private IClientDataStore ClientDataStore { get; set; }
        
        public bcstque4 QueuedRecord { get; set; }

        public IUserBroadcastSettings UserBroadcastSettings { get; set; }

        public IUserManager UserManager { get; set; }
        private IBroadcastRecordUpdatesManager UpdateManager { get; }
        public IBroadcastRecordRetriever RecordRetriever { get; }

        public BatchManager(IMasterDataStore masterDataStore, IClientDataStore clientDataStore, bcstque4 queuedRecord, IUserManager userManager, UserBroadcastSettings userBroadcastSettings,
            IBroadcastRecordUpdatesManager updateManager, IBroadcastRecordRetriever recordRetriever)
        {
            QueuedRecord = queuedRecord;
            MasterDataStore = masterDataStore;
            ClientDataStore = clientDataStore;
            UserManager = userManager;
            this.UserBroadcastSettings = userBroadcastSettings;
            UpdateManager = updateManager;
            RecordRetriever = recordRetriever;
        }

        public void SetUserBroadcastSettings()
        {
            var originalBatch = RecordRetriever.GetClientBroadcastRecord(ClientDataStore.ClientQueryDb, QueuedRecord.batchnum);

            var getSiteDefaultLanguageQuery = new GetSiteDefaultLanguageQuery(MasterDataStore.MastersQueryDb, QueuedRecord.agency);
            var getUserLanguageQuery = new GetUserLanguageByUserNumber(ClientDataStore.ClientQueryDb, UserManager.User.UserNumber);
            UserBroadcastSettings.BroadcastLanguage = UserManager.GetUserBroadcastLanguage(originalBatch, getSiteDefaultLanguageQuery, getUserLanguageQuery);
            
            UserBroadcastSettings.SetBroadcastLogging();
            UserBroadcastSettings.SetBroadcastStyle(UserManager.User.SGroupNbr);
            UserBroadcastSettings.SetLanguageVariables();
        }

        public void UpdateFinishedOriginalClientBatchInformation(ibbatch originalBatch, bool isOfflineReport, bool batchOk, IRecordTimingDetails broadcastRecTiming)
        {
            var originalBatchRecordUpdatesManager = new BroadcastRecordUpdatesManager();
            
            if (isOfflineReport)
            {
                originalBatchRecordUpdatesManager.UpdateOfflineBatch(originalBatch, batchOk, DateTime.Now);
            }
            else 
            {
                var originalLastStart = broadcastRecTiming.LastReportPeriodStart;
                if (!QueuedRecord.runspcl)
                {
                    originalBatchRecordUpdatesManager.UpdateNonOfflineBatch(originalBatch, batchOk, broadcastRecTiming, DateTime.Now);
                }
                else
                {
                    originalBatch.runspcl = false;
                }

                LogTimingData(QueuedRecord, true);

                var postRunCalculator = new PostRunTimingCalculator();
                var getAgencyQuery = new GetAgencyRecordByAgencyNameQuery(MasterDataStore.MastersQueryDb, QueuedRecord.agency.Trim());
                var getCorpAcctQuery = new GetCorpAcctByAgencyQuery(MasterDataStore.MastersQueryDb, QueuedRecord.agency.Trim());
                postRunCalculator.SetTiming(broadcastRecTiming, QueuedRecord.runspcl, batchOk, getAgencyQuery, getCorpAcctQuery, originalLastStart);

                UpdateManager.SignifyThatTimeValuesSetBySystem(originalBatch, batchOk, QueuedRecord.runspcl);

                originalBatch = broadcastRecTiming.MapToBatchRecord(originalBatch, QueuedRecord.runspcl);
                LogTimingData(originalBatch, false);
            }
            
            UpdateManager.UpdateBroadcastInDataStore(ClientDataStore, originalBatch);
        }
        
        public void IncrementOriginalBatchNextRun(DateTime nextRun)
        {
            var originalBatch = RecordRetriever.GetClientBroadcastRecord(ClientDataStore.ClientQueryDb, QueuedRecord.batchnum);

            if (originalBatch == null) { return; }

            originalBatch.nextrun = nextRun.AddDays(1);

            UpdateManager.UpdateBroadcastInDataStore(ClientDataStore, originalBatch);
        }

        public void SetErrorFlagInOriginalQueuedBatchRecord(ibbatch originalBatch)
        {
            if (originalBatch != null)
            {
                originalBatch.errflag = true;

                UpdateManager.UpdateBroadcastInDataStore(ClientDataStore, originalBatch);
            }
        }

        private void LogTimingData(bcstque4 broadcast, bool preRun)
        {
            var run = preRun ? "Prerun " : "Postrun ";
            var msg = $"{run} timing: Report Period [{broadcast.nxtdstart} - {broadcast.nxtdend}] | Next Run [{broadcast.nextrun}]";
            LOG.DebugLogWithBatchInfo(broadcast, msg);
        }

        private void LogTimingData(ibbatch broadcast, bool preRun)
        {
            var run = preRun ? "Prerun " : "Postrun ";
            var msg = $"{run} timing: Report Period [{broadcast.nxtdstart} - {broadcast.nxtdend}] | Next Run [{broadcast.nextrun}]";
            LOG.DebugLogWithBatchInfo(broadcast, msg);
        }
    }
}
