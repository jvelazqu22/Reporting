using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Constants;
using Domain.Extensions;
using Domain.Helper;
using iBank.Entities.MasterEntities;

namespace Domain.Orm.iBankMastersQueries.BroadcastQueries.BroadcastRetrieval
{
    public class LoggingServerBroadcastRetrieval : IBroadcastRetrieval
    {
        private readonly IQueryable<broadcast_stage_agencies> _loggingAgencies;

        public LoggingServerBroadcastRetrieval(IQueryable<broadcast_stage_agencies> loggingAgencies)
        {
            _loggingAgencies = loggingAgencies;
        }

        public List<bcstque4> GetBroadcasts(IQueryable<bcstque4> queue)
        {
            using (var scope = TransactionScopeBuilder.BuildNoLockScope())
            {
                try
                {
                    var agencyWideLoggingBroadcasts = queue.ArePendingBroadcasts()
                        .HaveBatchNumber()
                        .AreNotStageAgencyBroadcasts(Broadcasts.StageAgency)
                        .AreLoggingAgencyBroadcasts(_loggingAgencies)
                        .ToList();

                    var specificLoggingBroadcasts = queue.ArePendingBroadcasts()
                        .HaveBatchNumber()
                        .AreNotStageAgencyBroadcasts(Broadcasts.StageAgency)
                        .AsEnumerable()
                        .AreSpecificBatchNumberLoggingBroadcasts(_loggingAgencies.ToList());

                    return agencyWideLoggingBroadcasts.Concat(specificLoggingBroadcasts).ToList();
                }
                finally
                {
                    scope.Complete();
                }
            }
        }
    }
}
