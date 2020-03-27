using System;
using System.Linq;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.BroadcastQueries.BroadcastHistoryQueries
{
    public class GetBroadcastHistoryMissingAddedToPoolQuery : IQuery<BroadcastHistory>
    {
        private readonly IMastersQueryable _db;
        private readonly bcstque4 _bcstque4;

        public GetBroadcastHistoryMissingAddedToPoolQuery(IMastersQueryable db, bcstque4 recToUpdate)
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
                    .Where(x => !x.added_to_pool.HasValue)
                    .OrderByDescending(o => o.created_on)
                    .FirstOrDefault();
            }
        }
    }
}
