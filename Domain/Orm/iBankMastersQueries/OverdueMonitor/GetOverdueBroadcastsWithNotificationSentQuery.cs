using System.Collections.Generic;
using System.Linq;

using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.OverdueMonitor
{
    public class GetExistingOverdueBroadcastRecordsQuery : IQuery<IList<overdue_broadcasts>>
    {
        private readonly IMastersQueryable _db;

        public GetExistingOverdueBroadcastRecordsQuery(IMastersQueryable db)
        {
            _db = db;
        }

        public IList<overdue_broadcasts> ExecuteQuery()
        {
            using (_db)
            {
                //null check to allow unit test mocking to work
                return _db.OverdueBroadcasts.Where(x => x != null).ToList();
            }
        }
    }
}
