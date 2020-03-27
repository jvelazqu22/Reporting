using System.Collections.Generic;
using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankClientQueries
{
    public class GetAllVendorsByAgencyQuery : BaseiBankClientQueryable<IList<vendor>>
    {
        public string Agency { get; set; }

        public GetAllVendorsByAgencyQuery(IClientQueryable db, string agency)
        {
            _db = db;
            Agency = agency;
        }

        public override IList<vendor> ExecuteQuery()
        {
            using (_db)
            {
                return _db.Vendors.Where(x => x.agency.Trim() == Agency).ToList();
            }
        }
    }
}
