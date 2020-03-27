using System.Collections.Generic;

using iBank.Entities.MasterEntities;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Interfaces.BroadcastServer
{
    public interface IBroadcastQueueRecordRemover
    {
        void RemoveBroadcastFromQueue(bcstque4 broadcastToRemove, ICommandDb masterCommandDb);
    }
}
