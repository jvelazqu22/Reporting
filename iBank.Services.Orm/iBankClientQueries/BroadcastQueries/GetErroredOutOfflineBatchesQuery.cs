using Domain.Helper;
using Domain.Interfaces;
using iBank.Services.Orm.Databases.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Orm.iBankClientQueries.BroadcastQueries
{
    public class GetErroredOutOfflineBatchesQuery : IQuery<IList<ibbatch>>
    {
        private IClientQueryable _db;
        private DateTime Threshold { get; }

        public GetErroredOutOfflineBatchesQuery(IClientQueryable clientQueryDb, DateTime threshold)
        {
            _db = clientQueryDb;
            Threshold = threshold;
        }

        public IList<ibbatch> ExecuteQuery()
        {
            using (_db)
            {
                return
                    _db.iBBatch.Where(x => x.batchname.Length >= 7 && x.batchname.Substring(0, 7).Equals(BroadcastCriteria.OfflineRecord, StringComparison.OrdinalIgnoreCase)
                                     && (x.lastrun.HasValue && x.lastrun < Threshold)
                                     && (x.batchname.ToUpper().Contains(BroadcastCriteria.Error))).ToList();
            }
        }
    }
}
