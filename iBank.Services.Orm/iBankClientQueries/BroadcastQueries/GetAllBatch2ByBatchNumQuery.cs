using System.Collections.Generic;
using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankClientQueries.BroadcastQueries
{
    public class GetAllBatch2ByBatchNumQuery : BaseiBankClientQueryable<IList<ibbatch2>>
    {
        public int Key { get; set; }

        public GetAllBatch2ByBatchNumQuery(IClientQueryable db, int key)
        {
            _db = db;
            Key = key;
        }

        public override IList<ibbatch2> ExecuteQuery()
        {
            using (_db)
            {
                return _db.iBBatch2.Where(x => x.batchnum == Key).ToList();
            }
        }
    }
}
