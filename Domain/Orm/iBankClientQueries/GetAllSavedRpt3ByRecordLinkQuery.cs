using System.Collections.Generic;
using System.Linq;

using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientQueries
{
    public class GetAllSavedReport3ByRecordLinkQuery : IQuery<IList<savedrpt3>>
    {
        public int? Key { get; set; }

        private readonly IClientQueryable _db;

        public GetAllSavedReport3ByRecordLinkQuery(IClientQueryable db, int? key)
        {
            _db = db;
            Key = key;
        }

        public IList<savedrpt3> ExecuteQuery()
        {
            using (_db)
            {
                return _db.SavedRpt3.Where(x => x.recordlink == Key).ToList();
            }
        }
    }
}
