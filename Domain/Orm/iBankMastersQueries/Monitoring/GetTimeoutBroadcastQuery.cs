using System.Linq;

using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Monitoring
{
    public class GetTimeoutBroadcastQuery : IQuery<timeout_broadcasts>
    {
        private readonly IMastersQueryable _db;

        private readonly bcstque4 _bcst;

        public GetTimeoutBroadcastQuery(IMastersQueryable db, bcstque4 bcst)
        {
            _db = db;
            _bcst = bcst;
        }

        public timeout_broadcasts ExecuteQuery()
        {
            using (_db)
            {
                return _db.TimeoutBroadcasts.FirstOrDefault(x => x.agency.Equals(_bcst.agency.Trim())
                                                                 && x.batchname.Equals(_bcst.batchname)
                                                                 && x.batchnum == (_bcst.batchnum ?? -1)
                                                                 && x.nextrun == _bcst.nextrun
                                                                 && x.UserNumber == _bcst.UserNumber
                                                                 && x.database_name.Equals(_bcst.dbname));
            }
        }
    }
}
