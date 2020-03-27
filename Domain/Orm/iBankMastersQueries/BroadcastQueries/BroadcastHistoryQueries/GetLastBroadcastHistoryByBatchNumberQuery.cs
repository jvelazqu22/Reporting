using System;
using System.Linq;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.BroadcastQueries.BroadcastHistoryQueries
{
    public class GetLastBroadcastHistoryByBatchNumberQuery : IQuery<BroadcastHistory>
    {
        private readonly IMastersQueryable _db;
        private readonly bcstque4 _bcstque4;

        public GetLastBroadcastHistoryByBatchNumberQuery(IMastersQueryable db, bcstque4 recToUpdate)
        {
            _db = db;
            _bcstque4 = recToUpdate;
        }

        public BroadcastHistory ExecuteQuery()
        {
            using (_db)
            {
                return _db.BroadcastHistory
                    .Where(x => x.batchname.Equals(_bcstque4.batchname))
                    .Where(x => x.batchnum == _bcstque4.batchnum)
                    .Where(x => x.agency.Equals(_bcstque4.agency, StringComparison.OrdinalIgnoreCase))
                    .Where(x => x.start_of_run.HasValue)
                    .Where(x => !x.finished_run.HasValue)
                    .OrderByDescending(o => o.start_of_run)
                    .FirstOrDefault();
            }
        }
    }
}
