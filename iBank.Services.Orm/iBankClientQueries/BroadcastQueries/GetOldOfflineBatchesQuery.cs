using System;
using System.Collections.Generic;
using System.Linq;

using Domain.Helper;

using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankClientQueries.BroadcastQueries
{
    public class GetOldOfflineBatchesQuery : BaseiBankClientQueryable<IList<ibbatch>>
    {
        public DateTime Threshold { get; set; }

        public GetOldOfflineBatchesQuery(IClientQueryable db, DateTime threshold)
        {
            _db = db;
            Threshold = threshold;
        }

        public override IList<ibbatch> ExecuteQuery()
        {
            using (_db)
            {
                return _db.iBBatch.Where(x => (x.batchname.Length >= 7 && x.batchname.Substring(0, 7).Equals(BroadcastCriteria.OfflineRecord, StringComparison.OrdinalIgnoreCase))
                                              && (x.lastrun.HasValue && x.lastrun < Threshold)
                                              && (x.batchname.Length >= 4 && x.batchname.ToUpper().Contains(BroadcastCriteria.Done) 
                                                    || (x.batchname.Length >= 7 && x.batchname.ToUpper().Contains(BroadcastCriteria.Pending))))
                                    .ToList();
            }
        }
    }
}
