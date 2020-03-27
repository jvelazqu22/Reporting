using System.Linq;

using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientQueries.BroadcastQueries
{
    public class GetSavedRptNumberFromBatchNumberQuery : IQuery<int?>
    {
        private int BatchNumber { get; set; }

        private IClientQueryable _db;
        public GetSavedRptNumberFromBatchNumberQuery(IClientQueryable db, int batchNumber)
        {
            _db = db;
            BatchNumber = batchNumber;

        }
        public int? ExecuteQuery()
        {
            using (_db)
            {
                var batch = _db.iBBatch2.FirstOrDefault(x => x.batchnum == BatchNumber);

                return batch == null ? null : batch.savedrptnum;
            }
        }
    }
}
