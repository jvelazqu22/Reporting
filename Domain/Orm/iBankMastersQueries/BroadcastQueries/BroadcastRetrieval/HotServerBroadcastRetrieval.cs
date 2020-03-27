using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Constants;
using Domain.Extensions;
using Domain.Helper;
using iBank.Entities.MasterEntities;

namespace Domain.Orm.iBankMastersQueries.BroadcastQueries.BroadcastRetrieval
{
    public class HotServerBroadcastRetrieval : IBroadcastRetrieval
    {
        private readonly IQueryable<broadcast_stage_agencies> _loggingAgencies;

        public HotServerBroadcastRetrieval(IQueryable<broadcast_stage_agencies> loggingAgencies)
        {
            _loggingAgencies = loggingAgencies;
        }

        public List<bcstque4> GetBroadcasts(IQueryable<bcstque4> queue)
        {
            using (var scope = TransactionScopeBuilder.BuildNoLockScope())
            {
                try
                {
                    return queue.ArePendingBroadcasts()
                    .AreEffectsBroadcasts()
                    .HaveBatchNumber()
                    .AreNotStageAgencyBroadcasts(Broadcasts.StageAgency)
                    .AreNotLoggingAgencyBroadcasts(_loggingAgencies)
                    .AsEnumerable()
                    .AreNotSpecificBatchNumberLoggingBroadcasts(_loggingAgencies.ToList())
                    .ToList();
                }
                finally
                {
                    scope.Complete(); ;
                }
            }
        }
    }
}
