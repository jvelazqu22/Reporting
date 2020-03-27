using System.Linq;

using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientQueries.BroadcastQueries
{
    public class BatchHistoryRecordExistsQuery : IQuery<bool>
    {
        private readonly IClientQueryable _db;

        private readonly int _batchNumber;

        public BatchHistoryRecordExistsQuery(IClientQueryable db, int batchNumber)
        {
            _db = db;
            _batchNumber = batchNumber;
        }

        public bool ExecuteQuery()
        {
            using (_db)
            {
                return _db.iBBatchHistory.Any(x => x.batchnum == _batchNumber);
            }
        }
    }
}
