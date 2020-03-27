using System.Collections.Generic;
using System.Linq;

using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientQueries
{
    public class GetAllVendorsByAgencyQuery : IQuery<IList<vendors>>
    {
        public string Agency { get; set; }

        private readonly IClientQueryable _db;

        public GetAllVendorsByAgencyQuery(IClientQueryable db, string agency)
        {
            _db = db;
            Agency = agency;
        }

        public IList<vendors> ExecuteQuery()
        {
            using (_db)
            {
                return _db.Vendors.Where(x => x.agency.Trim() == Agency).ToList();
            }
        }
    }
}
