using System;
using System.Collections.Generic;
using System.Linq;

using Domain.Helper;

using iBank.Entities.MasterEntities;

namespace iBank.BroadcastServer.BroadcastBatch
{
    public class QueueFilter
    {
        public IList<bcstque4> ReorderBatchesToPutOfflineFirst(IList<bcstque4> pendingBatches)
        {
            //break the batches into offline and not offline
            var offlineRecords = pendingBatches.Where(x => x.batchname.Length >= 7
                                                        && x.batchname.Substring(0, 7).Equals(BroadcastCriteria.OfflineRecord, 
                                                                                        StringComparison.OrdinalIgnoreCase)).ToList();

            var nonOfflineRecords = pendingBatches.Where(x => !(x.batchname.Length >= 7
                                                                && x.batchname.Substring(0, 7).Equals(BroadcastCriteria.OfflineRecord,
                                                                    StringComparison.OrdinalIgnoreCase))).ToList();

            //put back together with offline first
            var reorderedBatches = new List<bcstque4>();
            reorderedBatches.AddRange(offlineRecords.OrderBy(x => x.nextrun).ThenBy(x => x.bcstseqno));
            reorderedBatches.AddRange(nonOfflineRecords.OrderBy(x => x.nextrun).ThenBy(x => x.bcstseqno));

            return reorderedBatches;
        }
    }
}
