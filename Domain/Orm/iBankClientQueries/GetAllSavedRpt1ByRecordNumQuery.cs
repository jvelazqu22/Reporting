using System.Collections.Generic;
using System.Linq;

using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientQueries
{
    public class GetAllSavedRpt1ByRecordNumQuery : IQuery<IList<savedrpt1>>
    {
        public int? Key { get; set; }

        private readonly IClientQueryable _db;

        public GetAllSavedRpt1ByRecordNumQuery(IClientQueryable db, int? key)
        {
            _db = db;
            Key = key;
        }

        public IList<savedrpt1> ExecuteQuery()
        {
            using (_db)
            {
                return _db.SavedRpt1.Where(x => x.recordnum == Key).ToList();
            }
        }
    }
}
