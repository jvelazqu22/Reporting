using System;
using System.Collections.Generic;
using System.Linq;

using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankAdminQueries
{
    public class GetAllUserReportsByAgencyQuery : IQuery<IList<userrpts>>
    {
        private readonly string _agency;

        private readonly IClientQueryable _db;

        public GetAllUserReportsByAgencyQuery(IClientQueryable db, string agency)
        {
            _db = db;
            _agency = agency;
        }

        public IList<userrpts> ExecuteQuery()
        {
            using (_db)
            {
                return _db.UserRpt.Where(s => s.agency.Equals(_agency, StringComparison.OrdinalIgnoreCase)).ToList();
            }
        }
    }
}
