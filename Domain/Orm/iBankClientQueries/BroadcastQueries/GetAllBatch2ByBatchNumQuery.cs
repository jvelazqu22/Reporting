using System.Collections.Generic;
using System.Linq;

using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientQueries.BroadcastQueries
{
    public class GetAllBatch2ByBatchNumQuery : IQuery<IList<ibbatch2>>
    {
        public int Key { get; set; }

        private readonly IClientQueryable _db;

        public GetAllBatch2ByBatchNumQuery(IClientQueryable db, int key)
        {
            _db = db;
            Key = key;
        }

        public IList<ibbatch2> ExecuteQuery()
        {
            using (_db)
            {
                return _db.iBBatch2.Where(x => x.batchnum == Key).ToList();
            }
        }
    }
}
