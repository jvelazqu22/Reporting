using System;
using System.Collections.Generic;
using System.Linq;

using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankAdminQueries
{
    public class GetAllSavedReportsByAgencyQuery : IQuery<IList<savedrpt1>>
    {
        private readonly string _agency;

        private readonly IClientQueryable _db;
        public GetAllSavedReportsByAgencyQuery(IClientQueryable db, string agency)
        {
            _db = db;
            _agency = agency;
        }

        public IList<savedrpt1> ExecuteQuery()
        {
            using (_db)
            {
                return _db.SavedRpt1.Where(s => s.agency.Equals(_agency,StringComparison.OrdinalIgnoreCase)).ToList();
            }
        }
    }
}
