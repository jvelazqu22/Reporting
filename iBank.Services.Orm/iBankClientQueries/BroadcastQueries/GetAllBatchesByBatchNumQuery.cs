using System.Collections.Generic;
using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankClientQueries.BroadcastQueries
{
    public class GetAllBatchesByBatchNumQuery : BaseiBankClientQueryable<IList<ibbatch>>
    {
        public int Key { get; set; }

        public GetAllBatchesByBatchNumQuery(IClientQueryable db, int key)
        {
            _db = db;
            Key = key;
        }

        public override IList<ibbatch> ExecuteQuery()
        {
            using (_db)
            {
                return _db.iBBatch.Where(x => x.batchnum == Key).ToList();
            }
        }
    }
}
