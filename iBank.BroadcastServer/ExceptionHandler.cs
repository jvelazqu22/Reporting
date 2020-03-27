using Domain.Constants;
using Domain.Exceptions;
using Domain.Models.BroadcastServer;

using iBank.BroadcastServer.Helper;
using iBank.Entities.MasterEntities;
using iBank.Server.Utilities.Logging;

using iBankDomain.Exceptions;

namespace iBank.BroadcastServer
{
    public static class ExceptionHandler
    {
        public static void HandleRecoverableSqlException(RecoverableSqlException ex, Parameters p, bcstque4 batch, IBroadcastLogger log)
        {
            log.WarnLogWithBatchInfo(batch, BroadcastLoggingMessage.DatabaseErrorRemoveFromQueue, ex);
            p.BroadcastQueueRecordRemover.RemoveBroadcastFromQueue(batch, p.MasterDataStore.MastersCommandDb);

            var highAlert = new HighAlertHandler();
            highAlert.NotificationIfHighAlertAgency(p.MasterDataStore, batch, false);
        }

        public static void HandleNotTimeToRunBroadcastException(NotTimeToRunBroadcastException ex, Parameters p, bcstque4 batch, IBroadcastLogger log)
        {
            log.WarnLogWithBatchInfo(batch, ex.Message);
            p.BroadcastQueueRecordRemover.RemoveBroadcastFromQueue(batch, p.MasterDataStore.MastersCommandDb);
        }

        public static void HandleAbandonedReportException(ReportAbandonedException ex, Parameters p, bcstque4 batch, IBroadcastLogger log)
        {
            log.WarnLogWithBatchInfo(batch, ex.Message, ex);
            p.BroadcastQueueRecordRemover.RemoveBroadcastFromQueue(batch, p.MasterDataStore.MastersCommandDb);
        }
    }
}
