using System.Linq;

using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientQueries.BroadcastQueries
{
    public class GetClientBroadcastBatchRecordByBatchNumberQuery : IQuery<ibbatch>
    {
        public int? BatchNumber { get; set; }

        private readonly IClientQueryable _db;

        public GetClientBroadcastBatchRecordByBatchNumberQuery(IClientQueryable db, int? batchNumber)
        {
            _db = db;
            BatchNumber = batchNumber;
        }

        public ibbatch ExecuteQuery()
        {
            using (_db)
            {
                return _db.iBBatch.FirstOrDefault(x => x.batchnum == BatchNumber);
            }
        }
    }
}
