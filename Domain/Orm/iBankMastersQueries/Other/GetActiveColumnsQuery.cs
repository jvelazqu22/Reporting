using System.Collections.Generic;
using System.Linq;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetActiveColumnsQuery : IQuery<IList<collist2>>
    {
        private IMastersQueryable _db;
        public GetActiveColumnsQuery(IMastersQueryable db)
        {
            _db = db;
        }

        public IList<collist2> ExecuteQuery()
        {
            using (_db)
            {
                return _db.Collist2.Where(s => !s.inactive).ToList();
            }
        }
    }
}
