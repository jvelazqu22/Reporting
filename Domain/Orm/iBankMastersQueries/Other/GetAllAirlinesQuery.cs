using System.Collections.Generic;
using System.Linq;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetAllAirlinesQuery : IQuery<IList<airlines>>
    {
        private IMastersQueryable _db;
        public GetAllAirlinesQuery(IMastersQueryable db)
        {
            _db = db;
        }

        public IList<airlines> ExecuteQuery()
        {
            using (_db)
            {
                return _db.Airlines.OrderBy(x => x.airlinenbr).ToList();
            }
        }
    }
}
