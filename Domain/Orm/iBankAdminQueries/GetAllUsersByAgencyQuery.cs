using System;
using System.Collections.Generic;
using System.Linq;

using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankAdminQueries
{
    public class GetAllUsersByAgencyQuery : IQuery<IList<ibuser>>
    {
        private readonly string _agency;

        private readonly IClientQueryable _db;
        public GetAllUsersByAgencyQuery(IClientQueryable db, string agency)
        {
            _db = db;
            _agency = agency;
        }

        public IList<ibuser> ExecuteQuery()
        {
            using (_db)
            {
                return _db.iBUser.Where(s => s.agency.Equals(_agency, StringComparison.OrdinalIgnoreCase)).ToList();
            }
        }
    }
}
