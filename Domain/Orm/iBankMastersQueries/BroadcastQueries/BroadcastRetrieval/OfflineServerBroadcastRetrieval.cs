﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using com.ciswired.libraries.CISLogger;
using Domain.Constants;
using Domain.Extensions;
using Domain.Helper;
using Domain.Services;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;

namespace Domain.Orm.iBankMastersQueries.BroadcastQueries.BroadcastRetrieval
{
    public class OfflineServerBroadcastRetrieval : IBroadcastRetrieval
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IQueryable<broadcast_stage_agencies> _loggingAgencies;
        private IMasterDataStore _store;
        private ICacheService _cache;

        public OfflineServerBroadcastRetrieval(IMasterDataStore store, ICacheService cache,
            IQueryable<broadcast_stage_agencies> loggingAgencies)
        {
            _store = store;
            _cache = cache;
            _loggingAgencies = loggingAgencies;
        }

        public List<bcstque4> GetBroadcasts(IQueryable<bcstque4> queue)
        {
            IList<bcstque4> recs;
            using (var scope = TransactionScopeBuilder.BuildNoLockScope())
            {
                try
                {                 
                    recs = queue.ArePendingBroadcasts()
                            .AreOfflineBroadcasts()
                            .HaveBatchNumber()
                            .AreNotStageAgencyBroadcasts(Broadcasts.StageAgency)
                            .AreNotLoggingAgencyBroadcasts(_loggingAgencies)
                            .AsEnumerable()
                            .AreNotSpecificBatchNumberLoggingBroadcasts(_loggingAgencies.ToList())
                            .ToList();
                }
                finally
                {
                    scope.Complete();
                }

            }

            LOG.Debug($"[{BroadcastServerFunction.Offline}] - Retrieved [{recs.Count}] broadcasts prior to filtering out long running broadcasts.");

            var svc = new LongRunningService(_cache, _store);
            var longRunning = recs.Where(x => svc.IsLongRunningBroadcast(x)).ToList();
            LOG.Debug($"[{BroadcastServerFunction.Offline}] - [{longRunning.Count}] of [{recs.Count}] are considered long running.");

            return recs.AreNotLongRunningBroadcasts(longRunning).ToList();
        }
    }
}
