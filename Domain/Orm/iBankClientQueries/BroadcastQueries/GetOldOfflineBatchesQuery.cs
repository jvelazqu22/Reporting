using System;
using System.Collections.Generic;
using System.Linq;

using Domain.Helper;

using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientQueries.BroadcastQueries
{
    public class GetOldOfflineBatchesQuery : IQuery<IList<ibbatch>>
    {
        public DateTime Threshold { get; set; }

        private readonly IClientQueryable _db;

        public GetOldOfflineBatchesQuery(IClientQueryable db, DateTime threshold)
        {
            _db = db;
            Threshold = threshold;
        }

        public IList<ibbatch> ExecuteQuery()
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
