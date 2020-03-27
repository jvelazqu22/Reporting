using System.Collections.Generic;
using System.Linq;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetHibServicesDataSourceAbbrsByAgencyQuery : IQuery<IList<string>>
    {
        public string Agency { get; set; }
        private readonly IMastersQueryable _db;

        public GetHibServicesDataSourceAbbrsByAgencyQuery(IMastersQueryable db, string agency)
        {
            _db = db;
            Agency = agency;
        }

        public IList<string> ExecuteQuery()
        {
            using(_db)
            {
                return _db.MstrAgcySourceExtras.Where(x => x.agency == Agency && x.fieldFunction == "BUILDHIBSERVICESDATA" 
                                                           && x.fieldData.Contains("DONE")).Select(x => x.sourceAbbr).ToList();
            }
        }
    }
}
