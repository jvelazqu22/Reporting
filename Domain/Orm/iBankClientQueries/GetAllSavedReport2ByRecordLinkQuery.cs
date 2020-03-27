using System.Collections.Generic;
using System.Linq;

using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientQueries
{
    public class GetAllSavedReport2ByRecordLinkQuery : IQuery<IList<savedrpt2>>
    {
        public int? Key { get; set; }

        private readonly IClientQueryable _db;
        public GetAllSavedReport2ByRecordLinkQuery(IClientQueryable db, int? key)
        {
            _db = db;
            Key = key;
        }

        public IList<savedrpt2> ExecuteQuery()
        {
            using (_db)
            {
                return _db.SavedRpt2.Where(x => x.recordlink == Key).ToList();
            }
        }
    }
}
