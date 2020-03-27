using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using Domain.Helper;

using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankClientQueries.BroadcastQueries
{
    public class GetAllClientOfflineBatchQuery : BaseiBankClientQueryable<IList<ibbatch>>
    {
        public DateTime CycleTimeZone { get; set; }
        private IList<string> Agencies { get; set; }

        public GetAllClientOfflineBatchQuery(IClientQueryable db, DateTime cycleTimeZone, IList<string> agencies)
        {
            _db = db;
            CycleTimeZone = cycleTimeZone;
            Agencies = agencies;
        }

        public override IList<ibbatch> ExecuteQuery()
        {
            using (_db)
            {
                var batches = _db.iBBatch.Where(x => Agencies.Contains(x.agency.Trim().ToUpper())
                                                && !x.errflag
                                                && x.nextrun < CycleTimeZone
                                                && !x.holdrun.Equals(BroadcastCriteria.HoldOnRecord, StringComparison.OrdinalIgnoreCase)
                                                && (x.batchname.Length >= 7 && x.batchname.Substring(0, 7).Equals(BroadcastCriteria.OfflineRecord, StringComparison.OrdinalIgnoreCase))
                                                && !x.outputdest.Equals(BroadcastCriteria.EffectsOutputDest)).ToList();
                
                return batches;
            }
        }
    }
}
