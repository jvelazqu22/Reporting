using System;
using System.Reflection;

using com.ciswired.libraries.CISLogger;

using Domain.Helper;
using Domain.Interfaces.BroadcastServer;
using Domain.Models.BroadcastServer;

using iBank.BroadcastServer.BroadcastBatch;
using iBank.BroadcastServer.Email;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Logging;

namespace iBank.BroadcastServer.Helper
{
    public interface IErrorHandler
    {
        void LogErrorDontMarkBroadcastInError(Exception ex, ILogger log, bcstque4 queueRecord, IMasterDataStore masterDataStore,
                                                              IBroadcastQueueRecordRemover queueRemover, BroadcastServerInformation serverConfig, MethodBase callingMethod);

        void LogErrorAndMarkBroadcastInError(Exception ex, ILogger log, bcstque4 queueRecord, IClientDataStore clientDataStore, IMasterDataStore masterDataStore,
                                                             IBroadcastQueueRecordRemover queueRemover, BroadcastServerInformation serverConfig, MethodBase callingMethod);

        void LogErrorAndMarkBroadcastInError(string errorMessage, ILogger log, bcstque4 queueRecord, IClientDataStore clientDataStore, IMasterDataStore masterDataStore,
                                                             IBroadcastQueueRecordRemover queueRemover, BroadcastServerInformation serverConfig, MethodBase callingMethod);

        void SendErrorEmail(ILogger log, bcstque4 queueRecord, int errorNumber, IMasterDataStore masterDataStore, BroadcastServerInformation serverConfig);

        void SendErrorEmail(ILogger log, bcstque4 queueRecord, int errorNumber, IMasterDataStore masterDataStore, BroadcastServerInformation serverConfig, BroadcastReportInformation report);
    }

    public class ErrorHandler : IErrorHandler
    {
        public void LogErrorDontMarkBroadcastInError(Exception ex, ILogger log, bcstque4 queueRecord, IMasterDataStore masterDataStore,
            IBroadcastQueueRecordRemover queueRemover, BroadcastServerInformation serverConfig, MethodBase callingMethod)
        {
            var broadcastLogger = new BroadcastLogger(log);
            //log
            var msg = broadcastLogger.FormatBroadcastInfo(queueRecord);
            var errorNumber = ErrorLogger.LogException(queueRecord.UserNumber, queueRecord.agency, ex, msg, callingMethod,
                ServerType.BroadcastServer, log);

            //remove from queue
            queueRemover.RemoveBroadcastFromQueue(queueRecord, masterDataStore.MastersCommandDb);

            if (queueRecord.send_error_email)
            {
                SendErrorEmail(log, queueRecord, errorNumber, masterDataStore, serverConfig);
            }
        }

        public void LogErrorAndMarkBroadcastInError(Exception ex, ILogger log, bcstque4 queueRecord, IClientDataStore clientDataStore, IMasterDataStore masterDataStore,
            IBroadcastQueueRecordRemover queueRemover, BroadcastServerInformation serverConfig, MethodBase callingMethod)
        {
            var broadcastLogger = new BroadcastLogger(log);
            //log
            var msg = broadcastLogger.FormatBroadcastInfo(queueRecord);
            var errorNumber = ErrorLogger.LogException(queueRecord.UserNumber, queueRecord.agency, ex, msg, callingMethod,
                ServerType.BroadcastServer, log);

            //mark broadcast as in error
            var originalBroadcast = new BroadcastRecordRetriever().GetClientBroadcastRecord(clientDataStore.ClientQueryDb, queueRecord.batchnum);
            originalBroadcast.errflag = true;
            var updateManager = new BroadcastRecordUpdatesManager();
            updateManager.UpdateBroadcastInDataStore(clientDataStore, originalBroadcast);

            //remove from queue
            queueRemover.RemoveBroadcastFromQueue(queueRecord, masterDataStore.MastersCommandDb);

            if (queueRecord.send_error_email)
            {
                SendErrorEmail(log, queueRecord, errorNumber, masterDataStore, serverConfig);
            }

            var highAlert = new HighAlertHandler();
            highAlert.NotificationIfHighAlertAgency(masterDataStore, queueRecord, true);
        }

        public void LogErrorAndMarkBroadcastInError(string errorMessage, ILogger log, bcstque4 queueRecord, IClientDataStore clientDataStore, IMasterDataStore masterDataStore,
            IBroadcastQueueRecordRemover queueRemover, BroadcastServerInformation serverConfig, MethodBase callingMethod)
        {
            //log
            var errorNumber = ErrorLogger.LogWarning(queueRecord.UserNumber, queueRecord.agency, errorMessage, callingMethod,
                ServerType.BroadcastServer, log);

            //mark broadcast as in error
            var originalBroadcast = new BroadcastRecordRetriever().GetClientBroadcastRecord(clientDataStore.ClientQueryDb, queueRecord.batchnum);
            originalBroadcast.errflag = true;
            var updateManager = new BroadcastRecordUpdatesManager();
            updateManager.UpdateBroadcastInDataStore(clientDataStore, originalBroadcast);

            //remove from queue
            queueRemover.RemoveBroadcastFromQueue(queueRecord, masterDataStore.MastersCommandDb);

            if (queueRecord.send_error_email)
            {
                SendErrorEmail(log, queueRecord, errorNumber, masterDataStore, serverConfig);
            }

            var highAlert = new HighAlertHandler();
            highAlert.NotificationIfHighAlertAgency(masterDataStore, queueRecord, true);
        }

        public void SendErrorEmail(ILogger log, bcstque4 queueRecord, int errorNumber, IMasterDataStore masterDataStore, BroadcastServerInformation serverConfig)
        {
            try
            {
                log.Debug("Sending admin email for error.");
                var adminEmailer = new AdminEmailer(queueRecord.bcsenderemail, masterDataStore, new Emailer());
                adminEmailer.SendErrorEmail(serverConfig, queueRecord, errorNumber, DateTime.Now);
            }
            catch (Exception e)
            {
                ErrorLogger.LogException(queueRecord.UserNumber, queueRecord.agency, e, MethodBase.GetCurrentMethod(), ServerType.BroadcastServer, log);
            }
        }

        public void SendErrorEmail(ILogger log, bcstque4 queueRecord, int errorNumber, IMasterDataStore masterDataStore, BroadcastServerInformation serverConfig, BroadcastReportInformation report)
        {
            try
            {
                log.Debug("Sending admin email for error.");
                var adminEmailer = new AdminEmailer(queueRecord.bcsenderemail, masterDataStore, new Emailer());
                adminEmailer.SendErrorEmail(serverConfig, queueRecord, errorNumber, DateTime.Now, report);
            }
            catch (Exception e)
            {
                ErrorLogger.LogException(queueRecord.UserNumber, queueRecord.agency, e, MethodBase.GetCurrentMethod(), ServerType.BroadcastServer, log);
            }
        }
    }
}
