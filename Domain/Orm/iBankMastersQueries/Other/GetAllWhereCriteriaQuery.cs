using System.Collections.Generic;
using System.Linq;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetAllWhereCriteriaQuery : IQuery<Dictionary<int, string>>
    {
        private readonly IMastersQueryable _db;

        public GetAllWhereCriteriaQuery(IMastersQueryable db)
        {
            _db = db;
        }

        public Dictionary<int, string> ExecuteQuery()
        {
            using (_db)
            {
                return _db.iBWhcrit.ToDictionary(k => k.ibwcritkey, v => v.webvarname.ToUpper().Trim());
            }
        }
    }
}
