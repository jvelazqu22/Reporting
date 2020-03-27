using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankClientQueries.BroadcastQueries
{
    public class GetClientBroadcastBatchRecordByBatchNumberQuery : BaseiBankClientQueryable<ibbatch>
    {
        public int? BatchNumber { get; set; }

        public GetClientBroadcastBatchRecordByBatchNumberQuery(IClientQueryable db, int? batchNumber)
        {
            _db = db;
            BatchNumber = batchNumber;
        }

        public override ibbatch ExecuteQuery()
        {
            using (_db)
            {
                return _db.iBBatch.FirstOrDefault(x => x.batchnum == BatchNumber);
            }
        }
    }
}
