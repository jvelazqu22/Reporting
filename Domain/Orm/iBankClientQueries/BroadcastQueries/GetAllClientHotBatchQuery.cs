using Domain.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientQueries.BroadcastQueries
{
    public class GetAllClientHotBatchQuery : IQuery<IList<ibbatch>>
    {
        public DateTime CycleTimeZone { get; set; }
        private IList<string> Agencies { get; set; }

        private readonly IClientQueryable _db;

        public GetAllClientHotBatchQuery(IClientQueryable db, DateTime cycleTimeZone, IList<string> agencies)
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
                                              && x.outputdest.Equals(BroadcastCriteria.EffectsOutputDest)
                                              && !x.holdrun.Equals(BroadcastCriteria.HoldOnRecord, StringComparison.OrdinalIgnoreCase)
                                              && (x.runspcl || (x.nextrun < CycleTimeZone))).ToList();
            }
        }

        public IList<ibbatch> DebugExecuteQuery()
        {
            using (_db)
            {
                var batches = _db.iBBatch.Where(x => !x.errflag && (x.runspcl || (x.nextrun < CycleTimeZone))).ToList();
                batches = batches.Where(x => Agencies.Contains(x.agency.Trim().ToUpper())
                                                     && !x.errflag
                                                     && x.outputdest.Equals(BroadcastCriteria.EffectsOutputDest)
                                                     && !x.holdrun.Equals(BroadcastCriteria.HoldOnRecord, StringComparison.OrdinalIgnoreCase)
                                                     && (x.runspcl || (x.nextrun < CycleTimeZone))).ToList();

                return batches;
            }
        }
    }
}
