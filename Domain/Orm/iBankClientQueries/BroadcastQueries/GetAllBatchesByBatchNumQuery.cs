using System.Collections.Generic;
using System.Linq;

using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientQueries.BroadcastQueries
{
    public class GetAllBatchesByBatchNumQuery : IQuery<IList<ibbatch>>
    {
        public int Key { get; set; }

        private readonly IClientQueryable _db;

        public GetAllBatchesByBatchNumQuery(IClientQueryable db, int key)
        {
            _db = db;
            Key = key;
        }

        public IList<ibbatch> ExecuteQuery()
        {
            using (_db)
            {
                return _db.iBBatch.Where(x => x.batchnum == Key).ToList();
            }
        }
    }
}
