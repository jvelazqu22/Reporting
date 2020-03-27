using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Helper;
using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientQueries.BroadcastQueries
{
    public class GetAllClientStandardBroadcastBatchQuery : IQuery<IList<ibbatch>>
    {
        public DateTime CycleTimeZone { get; set; }
        private IList<string> Agencies { get; set; }

        private readonly IClientQueryable _db;

        public GetAllClientStandardBroadcastBatchQuery(IClientQueryable db, DateTime cycleTimeZone, IList<string> agencies)
        {
            _db = db;
            CycleTimeZone = cycleTimeZone;
            Agencies = agencies;
        }

        public IList<ibbatch> ExecuteQuery()
        {
#if DEBUG
            return DebugExecuteQuery();
#endif
            using (_db)
            {
                return _db.iBBatch.Where(x => Agencies.Contains(x.agency.Trim().ToUpper())
                                              && !x.errflag
                                              && !x.holdrun.Equals(BroadcastCriteria.HoldOnRecord, StringComparison.OrdinalIgnoreCase)
                                              && !x.outputdest.Equals(BroadcastCriteria.EffectsOutputDest)
                                              && (x.nextrun < CycleTimeZone || x.runspcl)
                                              && !(x.batchname.Length >= 6
                                                   && x.batchname.Substring(0, 6).Equals(BroadcastCriteria.TravetRecord, StringComparison.OrdinalIgnoreCase))
                                              && !(x.batchname.Length >= 7
                                                   && x.batchname.Substring(0, 7).Equals(BroadcastCriteria.OfflineRecord, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }
        }

        public IList<ibbatch> DebugExecuteQuery()
        {
            using (_db)
            {
                var batches = _db.iBBatch.Where(x => !x.errflag && (x.nextrun < CycleTimeZone || x.runspcl)).ToList();
                batches = batches.Where(x => Agencies.Contains(x.agency.Trim().ToUpper())
                                              && !x.errflag
                                              && !x.holdrun.Equals(BroadcastCriteria.HoldOnRecord, StringComparison.OrdinalIgnoreCase)
                                              && !x.outputdest.Equals(BroadcastCriteria.EffectsOutputDest)
                                              && (x.nextrun < CycleTimeZone || x.runspcl)
                                              && !(x.batchname.Length >= 6
                                                   && x.batchname.Substring(0, 6).Equals(BroadcastCriteria.TravetRecord, StringComparison.OrdinalIgnoreCase))
                                              && !(x.batchname.Length >= 7
                                                   && x.batchname.Substring(0, 7).Equals(BroadcastCriteria.OfflineRecord, StringComparison.OrdinalIgnoreCase)))
                    .ToList();

                return batches;
            }
        }
    }
}
