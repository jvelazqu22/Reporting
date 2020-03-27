using System;

using iBank.Entities.ClientEntities;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Interfaces.BroadcastServer
{
    public interface IBroadcastRecordUpdatesManager
    {
        void ChangeBatchToRunning(bcstque4 batchToRun, int serverNumber, DateTime now);
        void ChangeBatchToDone(bcstque4 batchToRun, int serverNumber, DateTime now);

        void UpdateOfflineBatch(ibbatch clientBatch, bool batchOk, DateTime now);

        void UpdateNonOfflineBatch(ibbatch clientBatch, bool batchOk, IRecordTimingDetails broadcastTiming, DateTime now);

        void SignifyThatTimeValuesSetBySystem(ibbatch broadcast, bool batchOk, bool runSpecial);

        void UpdateBroadcastInDataStore(IClientDataStore clientDataStore, ibbatch clientBatch);

        bool UpdateQueueRecordInDataStore(ICommandDb masterCommandDb, bcstque4 batchToUpdate);
    }
}
