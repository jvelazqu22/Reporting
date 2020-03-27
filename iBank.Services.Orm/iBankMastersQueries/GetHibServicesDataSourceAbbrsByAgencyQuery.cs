using System.Collections.Generic;
using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetHibServicesDataSourceAbbrsByAgencyQuery : BaseiBankMastersQuery<IList<string>>
    {
        public string Agency { get; set; }

        public GetHibServicesDataSourceAbbrsByAgencyQuery(IMastersQueryable db, string agency)
        {
            _db = db;
            Agency = agency;
        }

        public override IList<string> ExecuteQuery()
        {
            using(_db)
            {
                return _db.MstrAgcySourceExtras.Where(x => x.agency == Agency && x.fieldFunction == "BUILDHIBSERVICESDATA" 
                                                           && x.fieldData.Contains("DONE")).Select(x => x.sourceAbbr).ToList();
            }
        }
    }
}
