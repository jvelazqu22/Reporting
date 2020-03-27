using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Constants;
using Domain.Extensions;
using Domain.Helper;
using iBank.Entities.MasterEntities;

namespace Domain.Orm.iBankMastersQueries.BroadcastQueries.BroadcastRetrieval
{
    public class StageServerBroadcastRetrieval : IBroadcastRetrieval
    {
        public List<bcstque4> GetBroadcasts(IQueryable<bcstque4> queue)
        {
            using (var scope = TransactionScopeBuilder.BuildNoLockScope())
            {               
                return queue.ArePendingBroadcasts()
                .HaveBatchNumber()
                .AreStageAgencyBroadcasts(Broadcasts.StageAgency)
                .ToList();
            }

        }
    }
}
