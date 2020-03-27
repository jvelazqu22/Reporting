using System.Collections.Generic;
using System.Linq;

using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankAdminQueries
{
    public class GetAllUsersQuery : IQuery<IList<ibuser>>
    {
        private readonly IClientQueryable _db;
        public GetAllUsersQuery(IClientQueryable db)
        {
            _db = db;
        }

        public IList<ibuser> ExecuteQuery()
        {
            using (_db)
            {
                return _db.iBUser.ToList();
            }
        }
    }
}
