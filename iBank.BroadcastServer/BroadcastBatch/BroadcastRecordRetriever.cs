using Domain.Helper;
using Domain.Interfaces.BroadcastServer;
using Domain.Orm.iBankClientQueries.BroadcastQueries;

using System.Collections.Generic;
using System.Linq;

using iBank.Entities.ClientEntities;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace iBank.BroadcastServer.BroadcastBatch
{
    public class BroadcastRecordRetriever : IBroadcastRecordRetriever
    {
        public ibbatch GetClientBroadcastRecord(IClientQueryable clientQueryDb, int? batchNumber)
        {
            var originalBatchQuery = new GetClientBroadcastBatchRecordByBatchNumberQuery(clientQueryDb, batchNumber);
            var originalBatch = originalBatchQuery.ExecuteQuery();
            return originalBatch;
        }

        public IList<bcstque4> GetPendingBroadcasts(BroadcastServerFunction function, IQuery<IList<bcstque4>> getPendingBroadcastsQuery)
        {
            var pendingBroadcasts = getPendingBroadcastsQuery.ExecuteQuery();

            if (function == BroadcastServerFunction.Primary)
            {
                var filter = new QueueFilter();
                return filter.ReorderBatchesToPutOfflineFirst(pendingBroadcasts);
            }
            else
            {
                return pendingBroadcasts.OrderBy(x => x.nextrun).ThenBy(x => x.bcstseqno).ToList();
            }
        }
    }
}
