using Domain.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Extensions;
using iBank.Entities.ClientEntities;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientQueries.BroadcastQueries
{
    public class GetAllClientBatchQuery : IQuery<List<ibbatch>>
    {
        public DateTime CycleTimeZone { get; set; }
        private IList<string> Agencies { get; set; }

        private readonly IClientQueryable _db;
        private List<ibbatch> _baseBatchList = new List<ibbatch>();

        public GetAllClientBatchQuery(IClientQueryable db, DateTime cycleTimeZone, IList<string> agencies)
        {
            _db = db;
            CycleTimeZone = cycleTimeZone;
            Agencies = agencies;
        }

        public List<ibbatch> ExecuteQuery()
        {
            var batchList = new List<ibbatch>();

            _baseBatchList = GetBaseList().ToList();

            batchList.AddRange(GetAllClientOfflineBatches());
            batchList.AddRange(GetAllClientTravetOfflineBatches());
            batchList.AddRange(GetAllClientStandardBroadcastBatches());
            batchList.AddRange(GetAllClientHotBatches());

            return batchList;
        }

        private List<ibbatch> GetBaseList()
        {
            var batchList = new List<ibbatch>();

            using (var scope = TransactionScopeBuilder.BuildNoLockScope())
            {
                try
                {
                    using (_db)
                    {
                        batchList = _db.iBBatch.Where(x => Agencies.Contains(x.agency.Trim().ToUpper())
                                                           && !x.errflag
                                                           && (x.runspcl || (x.nextrun < CycleTimeZone))
                                                           && !x.holdrun.Equals(BroadcastCriteria.HoldOnRecord, StringComparison.OrdinalIgnoreCase))
                            .ToList();
                    }

                }
                finally
                {
                    scope.Complete();
                }
            }

            return batchList;
        }

        private List<ibbatch> GetAllClientOfflineBatches()
        {
            var batches = _baseBatchList.Where(x => Agencies.Contains(x.agency.Trim().ToUpper())
                                                 && !x.errflag
                                                 && x.nextrun < CycleTimeZone
                                                 && !x.holdrun.Equals(BroadcastCriteria.HoldOnRecord, StringComparison.OrdinalIgnoreCase)
                                                 && (x.batchname.Length >= 7 && x.batchname.Substring(0, 7).Equals(BroadcastCriteria.OfflineRecord, StringComparison.OrdinalIgnoreCase))
                                                 && !x.outputdest.Equals(BroadcastCriteria.EffectsOutputDest))
                .ToList();

            return batches;
        }

        private List<ibbatch> GetAllClientTravetOfflineBatches()
        {
            var batches = _baseBatchList.Where(x => Agencies.Contains(x.agency.Trim().ToUpper())
                                                 && !x.errflag
                                                 && x.nextrun < CycleTimeZone
                                                 && !x.holdrun.Equals(BroadcastCriteria.HoldOnRecord, StringComparison.OrdinalIgnoreCase)
                                                 && (x.batchname.Length >= 6 && x.batchname.Substring(0, 6).Equals(BroadcastCriteria.TravetRecord, StringComparison.OrdinalIgnoreCase))).
                ToList();

            return batches;
        }

        private List<ibbatch> GetAllClientStandardBroadcastBatches()
        {
            var batches = _baseBatchList.Where(x => Agencies.Contains(x.agency.Trim().ToUpper())
                                                 && !x.errflag
                                                 && (x.nextrun < CycleTimeZone || x.runspcl)
                                                 && !x.holdrun.Equals(BroadcastCriteria.HoldOnRecord, StringComparison.OrdinalIgnoreCase)
                                                 && !x.outputdest.Equals(BroadcastCriteria.EffectsOutputDest)
                                                 && !(x.batchname.Length >= 6 && x.batchname.Substring(0, 6).Equals(BroadcastCriteria.TravetRecord, StringComparison.OrdinalIgnoreCase))
                                                 && !(x.batchname.Length >= 7 && x.batchname.Substring(0, 7).Equals(BroadcastCriteria.OfflineRecord, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            return batches;
        }

        private List<ibbatch> GetAllClientHotBatches()
        {
            var batches = _baseBatchList.Where(x => Agencies.Contains(x.agency.Trim().ToUpper())
                                                    && !x.errflag
                                                    && (x.runspcl || (x.nextrun < CycleTimeZone))
                                                    && !x.holdrun.Equals(BroadcastCriteria.HoldOnRecord, StringComparison.OrdinalIgnoreCase)
                                                    && x.outputdest.Equals(BroadcastCriteria.EffectsOutputDest))
                .ToList();

            return batches;
        }

    }
}
