using com.ciswired.libraries.CISLogger;
using Domain.Exceptions;
using Domain.Interfaces.BroadcastServer;
using Domain.Models.BroadcastServer;
using iBank.BroadcastServer.BroadcastBatch;
using iBank.BroadcastServer.BroadcastReport;
using iBank.BroadcastServer.Email;
using iBank.BroadcastServer.Timing;
using iBank.BroadcastServer.User;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Logging;

using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Domain;
using Domain.Constants;
using Domain.Helper;
using Domain.Orm.iBankClientQueries;
using Domain.Orm.iBankMastersQueries.Other;
using Domain.Services;
using iBank.BroadcastServer.Helper;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities.Helpers;
using iBank.Services.Implementation.Shared;
using iBankDomain.Exceptions;

namespace iBank.BroadcastServer.Service
{
    public class Consumer
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly IBroadcastLogger BCST_LOG = new BroadcastLogger(new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType));
        private CacheService _cache = new CacheService();

        public void ProcessBatches(Parameters p, LoadedListsParams loadedParams)
        {
            var maxThreads = ThreadLimits.GetMaxConcurrentBroadcastsForServer(_cache, p.ServerConfiguration.ServerNumber, new iBankAdministrationQueryable());
#if DEBUG
            maxThreads = 1;
#endif
            var options = new ParallelOptions { MaxDegreeOfParallelism = maxThreads };
            Parallel.ForEach(p.BatchesToExecute.GetConsumingPartitioner(), options, batch => ProcessBatch(batch, p, loadedParams));
        }

        public void ProcessBatch(bcstque4 batch, Parameters p, LoadedListsParams loadedParams)
        {
            try
            {
                if (p.IsMaintenanceModeRequested) return;

                //make sure current batch doesn't need database(s) that the other long-running broadcast server is(are) using
                if (p.ServerConfiguration.ServerFunction == BroadcastServerFunction.LongRunning)
                {
                    var longrunningHandler = new LongRunningHandler();
                    if (longrunningHandler.IsThereDatabaseConstraint(new MasterDataStore(), _cache, p.ServerConfiguration.ServerNumber, batch)) return;
                }
                
                BCST_LOG.InfoLogWithBatchInfo(batch, "Attempting to execute.");
                ExecutePendingBroadcast(batch, p, loadedParams);
            }
            catch (AggregateException aggEx)
            {
                var innerExceptions = aggEx.Flatten().InnerExceptions;

                //look for exceptions in particular order
                var ex = innerExceptions.FirstOrDefault(x => x is NotTimeToRunBroadcastException);
                if (ex != null)
                {
                    var timeEx = ex as NotTimeToRunBroadcastException;
                    ExceptionHandler.HandleNotTimeToRunBroadcastException(timeEx, p, batch, BCST_LOG);
                    return;
                }

                ex = innerExceptions.FirstOrDefault(x => x is RecoverableSqlException);
                if (ex != null)
                {
                    var recoverable = ex as RecoverableSqlException;
                    ExceptionHandler.HandleRecoverableSqlException(recoverable, p, batch, BCST_LOG);
                    return;
                }

                //default to a general exception
                ex = innerExceptions.FirstOrDefault();
                if (ex != null)
                {
                    new ErrorHandler().LogErrorDontMarkBroadcastInError(ex, LOG, batch, p.MasterDataStore, p.BroadcastQueueRecordRemover, p.ServerConfiguration,
                        MethodBase.GetCurrentMethod());
                }
            }
            catch (NotTimeToRunBroadcastException timeEx)
            {
                ExceptionHandler.HandleNotTimeToRunBroadcastException(timeEx, p, batch, BCST_LOG);
            }
            catch (RecoverableSqlException recoverableEx)
            {
                ExceptionHandler.HandleRecoverableSqlException(recoverableEx, p, batch, BCST_LOG);
            }
            catch (ReportAbandonedException abandonedEx)
            {
                ExceptionHandler.HandleAbandonedReportException(abandonedEx, p, batch, BCST_LOG);
            }
            catch (Exception ex)
            {
                new ErrorHandler().LogErrorDontMarkBroadcastInError(ex, LOG, batch, p.MasterDataStore, p.BroadcastQueueRecordRemover, p.ServerConfiguration,
                    MethodBase.GetCurrentMethod());
            }
        }

        public void ExecutePendingBroadcast(bcstque4 queueRecord, Parameters p, LoadedListsParams loadedParams)
        {
            /*  reports can only be run with the "run new data" option if it is a daily every x hours/x min report
             *  but the UI doesn't actually limit them to that - so here we make sure that option is set correctly based on the 
             *  report type that is being generated
             * */
            if (queueRecord.runnewdata == 1)
            {
                if (queueRecord.weekmonth >= 0)
                {
                    queueRecord.runnewdata = 0;
                }
            }

            p.BatchRecordUpdatesManager.ChangeBatchToRunning(queueRecord, p.ServerConfiguration.ServerNumber, DateTime.Now);
            var queueRecordTimingDetails = new RecordTimingDetails(queueRecord);

            //concurrency mode is set to FIXED on update_token column, so this will be false if someone else already got it
            if (!p.BatchRecordUpdatesManager.UpdateQueueRecordInDataStore(p.MasterDataStore.MastersCommandDb, queueRecord))
            {
                BCST_LOG.DebugLogWithBatchInfo(queueRecord, $"[{p.ServerConfiguration.ServerFunction}] - Abandoning, another server has already claimed broadcast.");
                return;
            }

            var broadcastHistoryHandler = new BroadcastHistoryHandler(queueRecord, p.MasterDataStore);
            broadcastHistoryHandler.UpdateBroadcastHistoryStartRun(p.MasterDataStore.MastersCommandDb);

            if (p.ServerConfiguration.ServerFunction == BroadcastServerFunction.LongRunning)
            {
                //we want a record of every agency that is making use of long running server
                var svc = new LongRunningService(new CacheService(), p.MasterDataStore);
                svc.LogInstance(queueRecord);
            }

            if (!BroadcastBatchTypeConditionals.IsBatchRunOffline(queueRecord.batchname) && !queueRecordTimingDetails.Conditionals.IsVariableRunTime)
            {
                //offline report has own saved date period, variable run time is specific time range - so otherwise we want all day
                var preRunCalculator = new PreRunTimingCalculator();
                preRunCalculator.SetNextReportPeriodEndToEndOfDay(queueRecordTimingDetails);
            }

            IClientDataStore clientDataStore;
            try
            {
                clientDataStore = GetClientDataStore(p, queueRecord);
            }
            catch (ServerAddressNotFoundException ex)
            {
                new ErrorHandler().LogErrorDontMarkBroadcastInError(ex, LOG, queueRecord, p.MasterDataStore, p.BroadcastQueueRecordRemover,
                    p.ServerConfiguration, MethodBase.GetCurrentMethod());
                return;
            }

            AddHistoryRecordIfNeeded(queueRecord, clientDataStore);

            try
            {
                var broadcastManager = CreateSetupAndGetABatchManager(clientDataStore, queueRecordTimingDetails, queueRecord, p);

                var threshold = DateTime.Today.AddMonths(-61);
                if (broadcastManager.QueuedRecord.nxtdstart < threshold
                    && !BroadcastBatchTypeConditionals.IsBatchRunOffline(broadcastManager.QueuedRecord.batchname)
                    && broadcastManager.QueuedRecord.weekmonth != 9
                    && broadcastManager.QueuedRecord.usespcl.HasValue
                    && !broadcastManager.QueuedRecord.usespcl.Value)
                {
                    new ErrorHandler().LogErrorAndMarkBroadcastInError("Invalid date range.", LOG, queueRecord, clientDataStore, p.MasterDataStore,
                        p.BroadcastQueueRecordRemover, p.ServerConfiguration, MethodBase.GetCurrentMethod());
                    return;
                }

                broadcastManager.UserManager.UpdateUserLogin(DateTime.Now, clientDataStore.ClientCommandDb);

                var isOfflineBatch = BroadcastBatchTypeConditionals.IsBatchRunOffline(broadcastManager.QueuedRecord.batchname);
                queueRecordTimingDetails = (RecordTimingDetails)SetupInitialQueueRecordTimingDetails(queueRecordTimingDetails, broadcastManager,
                    isOfflineBatch, p.MasterDataStore, p.BroadcastQueueRecordRemover);

                broadcastManager.QueuedRecord = queueRecordTimingDetails.MapToQueueRecord(broadcastManager.QueuedRecord);

                RunBatch(clientDataStore, broadcastManager, queueRecordTimingDetails, isOfflineBatch, p, loadedParams);
                broadcastHistoryHandler.UpdateBroadcastHistoryFinishedRun(p.MasterDataStore.MastersCommandDb);
            }
            catch (UserDoesNotExistException ex)
            {
                new ErrorHandler().LogErrorAndMarkBroadcastInError(ex, LOG, queueRecord, clientDataStore, p.MasterDataStore, p.BroadcastQueueRecordRemover,
                    p.ServerConfiguration, MethodBase.GetCurrentMethod());
            }
        }

        // TODO: discuss moving this logic inside the BatchManager class possibly in the constructor.
        public IBatchManager CreateSetupAndGetABatchManager(IClientDataStore clientDataStore, IRecordTimingDetails queueRecordTimingDetails, bcstque4 queueRecord,
            Parameters p)
        {
            if (queueRecord.UserNumber == null) throw new UserDoesNotExistException(BroadcastLoggingMessage.UserDoesNotExist);

            var userQuery = p.GetUserByUserNumberQuery ?? new GetUserByUserNumberQuery(clientDataStore.ClientQueryDb, queueRecord.UserNumber ?? 0);
            var user = userQuery.ExecuteQuery();
            var userManager = new UserManager(user);
            var userBroadcastSettings = new UserBroadcastSettings(clientDataStore, p.MasterDataStore, queueRecord.batchnum, queueRecord.agency, user.UserNumber);
            var broadcastRecordRetriever = new BroadcastRecordRetriever();

            var batchManager = p.BatchManager ?? new BatchManager(p.MasterDataStore, clientDataStore, queueRecordTimingDetails.MapToQueueRecord(queueRecord), userManager, userBroadcastSettings,
                p.BatchRecordUpdatesManager, broadcastRecordRetriever);

            if (batchManager.UserManager.User == null) throw new UserDoesNotExistException(BroadcastLoggingMessage.UserDoesNotExist);

            batchManager.SetUserBroadcastSettings();

            return batchManager;
        }

        public IRecordTimingDetails SetupInitialQueueRecordTimingDetails(IRecordTimingDetails queueRecordTimingDetails, IBatchManager batchManager, bool isOfflineBatch,
            IMasterDataStore masterDataStore, IBroadcastQueueRecordRemover queueRemover)
        {
            var preRunCalculator = new PreRunTimingCalculator();
            preRunCalculator.SetInitialRecordStartAndEndDatesToDateTimeSafe(queueRecordTimingDetails);
            preRunCalculator.SetInitialRecordNextReportPeriodStartAndEnd(queueRecordTimingDetails);

            var runSpecial = false;
            if (!isOfflineBatch && batchManager.QueuedRecord.runspcl)
            {
                preRunCalculator.SetReportRangeToSpecialDates(queueRecordTimingDetails);
                runSpecial = true;
            }

            if (queueRecordTimingDetails.Conditionals.IsRunDailyPriorBusinessDay && !runSpecial)
            {
                preRunCalculator.SetRunDailyPriorBusinessDayStartAndEndRange(queueRecordTimingDetails, batchManager);
            }

            if (queueRecordTimingDetails.Conditionals.IsRunDailyNextBusinessDay && !runSpecial)
            {
                preRunCalculator.SetRunDailyNextBusinessDayStartAndEndRange(queueRecordTimingDetails, batchManager);
            }

            return queueRecordTimingDetails;
        }

        public void RunBatch(IClientDataStore clientDataStore, IBatchManager broadcastManager, IRecordTimingDetails recordTimingDetails, 
            bool isOfflineBatch, Parameters p, LoadedListsParams loadedParams)
        {
            var bcstEmail = new BroadcastEmail(broadcastManager, isOfflineBatch, p.ServerConfiguration, p.MasterDataStore, clientDataStore, new Emailer());

            var processCaptions = p.GetAllActiveBroadcastProcessCaptionsQuery != null
                ? p.GetAllActiveBroadcastProcessCaptionsQuery.ExecuteQuery()
                : new GetAllActiveBroadcastProcessCaptionsQuery(p.MasterDataStore.MastersQueryDb).ExecuteQuery();

            var broadcastReportProcessor = p.BroadcastReportProcessor ?? new BroadcastReportProcessor(p.MasterDataStore, clientDataStore, processCaptions, bcstEmail, loadedParams);
            var reportRetriever = p.BatchReportRetriever ?? new BatchReportRetriever(broadcastManager.QueuedRecord.batchnum, clientDataStore, p.MasterDataStore);

            var userNumber = broadcastManager?.QueuedRecord?.UserNumber ?? -1;

            var isDemo = IsDemo(broadcastManager.QueuedRecord.UserNumber.Value);
            var allReportsInBatch = reportRetriever.GetAllReportsInBatch(processCaptions, broadcastManager.QueuedRecord.agency, isDemo, userNumber);
            BCST_LOG.InfoLogWithBatchInfo(broadcastManager.QueuedRecord, $"Retrieved [{allReportsInBatch.Count}] reports.");

            if (allReportsInBatch.Any())
            {
                var runSpecial = !isOfflineBatch && broadcastManager.QueuedRecord.runspcl;
                var batchRunner = p.BatchRunner ?? new BatchRunner(p.BroadcastQueueRecordRemover, broadcastManager, broadcastReportProcessor, p.MasterDataStore, clientDataStore, p.BatchRecordUpdatesManager);
                batchRunner.ExecuteBatch(_cache, allReportsInBatch, recordTimingDetails, p.ServerConfiguration, runSpecial, isOfflineBatch);
            }
            else
            {
                var msg = BCST_LOG.FormatMessageWithBroadcastInfo(broadcastManager.QueuedRecord, BroadcastLoggingMessage.BroadcastDidNotContainReports);

                new ErrorHandler().LogErrorAndMarkBroadcastInError(msg, LOG, broadcastManager.QueuedRecord, clientDataStore, p.MasterDataStore,
                    p.BroadcastQueueRecordRemover, p.ServerConfiguration, MethodBase.GetCurrentMethod());
            }
        }

        private bool IsDemo(int userNumber)
        {
            try
            {
                var temp = ConfigurationManager.AppSettings["DemoUsers"];
                var demoUsers = temp.Split(',').ToList();
                return demoUsers.Contains(userNumber.ToString());
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private IClientDataStore GetClientDataStore(Parameters p, bcstque4 queueRecord)
        {
            var clientServer = p.DatabaseInfoQuery != null && !p.DatabaseInfoQuery.HasDbBeenDisposed
                    ? iBankServerInformationRetrieval.GetServerName(p.DatabaseInfoQuery)
                    : iBankServerInformationRetrieval.GetServerName(new GetDatabaseInfoByDatabaseNameQuery(new iBankMastersQueryable(), queueRecord.dbname.Trim()));

            var clientDb = queueRecord.dbname.Trim();
            return p.ClientDataStore ?? new ClientDataStore(clientServer, clientDb);
        }


        private void AddHistoryRecordIfNeeded(bcstque4 broadcast, IClientDataStore store)
        {
            if (BroadcastBatchTypeConditionals.IsBatchRunOffline(broadcast.batchname)) return;
            var historyHandler = new BatchHistoryHandler(store);
            if (!historyHandler.HistoryRecordExists(broadcast.batchnum ?? 0)) historyHandler.InsertHistoryRecord(broadcast);
        }

    }
}
