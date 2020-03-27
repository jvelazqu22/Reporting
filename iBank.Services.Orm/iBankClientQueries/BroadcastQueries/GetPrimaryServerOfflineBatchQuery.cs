using System;
using System.Collections.Generic;
using System.Linq;

using Domain.Helper;

using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankClientQueries.BroadcastQueries
{
    public class GetPrimaryServerOfflineBatchQuery : BaseiBankClientQueryable<IList<ibbatch>>
    {
        public DateTime CycleTimeZone { get; set; }

        public GetPrimaryServerOfflineBatchQuery(IClientQueryable db, DateTime cycleTimeZone)
        {
            _db = db;
            CycleTimeZone = cycleTimeZone;
        }

        public override IList<ibbatch> ExecuteQuery()
        {
            using (_db)
            {
                return _db.iBBatch.Where(x => !x.errflag
                                                && !x.batchname.Contains(BroadcastCriteria.TravetRecord)
                                                && !x.outputdest.Equals(BroadcastCriteria.EffectsOutputDest)
                                                && (x.runspcl 
                                                    || (x.nextrun < CycleTimeZone && !x.holdrun.Equals(BroadcastCriteria.HoldOnRecord)
                                                    || (x.batchname.Length >= 7 && x.batchname.Substring(0, 7)
                                                                                              .Equals(BroadcastCriteria.OfflineRecord, 
                                                                                                        StringComparison.OrdinalIgnoreCase)))
                                                    )
                    ).ToList();
            }
        }
    }
}
