using Domain.Helper;

using System.Collections.Generic;

using iBank.Entities.ClientEntities;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Interfaces.BroadcastServer
{
    public interface IBroadcastRecordRetriever
    {
        ibbatch GetClientBroadcastRecord(IClientQueryable clientQueryDb, int? batchNumber);

        IList<bcstque4> GetPendingBroadcasts(BroadcastServerFunction function, IQuery<IList<bcstque4>> getPendingBroadcastsQuery);
    }
}
