using System;

using iBank.Entities.ClientEntities;
using iBank.Entities.MasterEntities;

namespace Domain.Interfaces.BroadcastServer
{
    public interface IBatchManager
    {
        IUserManager UserManager { get; set; }
        bcstque4 QueuedRecord { get; set; }
        IUserBroadcastSettings UserBroadcastSettings { get; set; }
        IBroadcastRecordRetriever RecordRetriever { get; }

        void SetUserBroadcastSettings();

        void SetErrorFlagInOriginalQueuedBatchRecord(ibbatch originalBatch);

        void UpdateFinishedOriginalClientBatchInformation(ibbatch originalBatch, bool isOfflineReport, bool batchOk, IRecordTimingDetails broadcastRecTiming);
       
        void IncrementOriginalBatchNextRun(DateTime nextRun);
    }
}
