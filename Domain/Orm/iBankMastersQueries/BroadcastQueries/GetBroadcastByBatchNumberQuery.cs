using System.Linq;

using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.BroadcastQueries
{
    public class GetBroadcastByBatchNumberQuery : IQuery<bcstque4>
    {
        private readonly IMastersQueryable _db;

        private readonly int _batchNumber;

        public GetBroadcastByBatchNumberQuery(IMastersQueryable db, int batchNumber)
        {
            _db = db;
            _batchNumber = batchNumber;
        }

        public bcstque4 ExecuteQuery()
        {
            using (_db)
            {
                return _db.BcstQue4.FirstOrDefault(x => x.batchnum == _batchNumber);
            }
        }
    }
}
