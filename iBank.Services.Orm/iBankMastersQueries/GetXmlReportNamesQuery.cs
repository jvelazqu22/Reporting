using System.Collections.Generic;
using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetXmlReportNamesQuery : BaseiBankMastersQuery<Dictionary<int, string>>
    {
        public GetXmlReportNamesQuery(IMastersQueryable db)
        {
            _db = db;
        }
        public override Dictionary<int, string> ExecuteQuery()
        {
            using (_db)
            {
                return _db.XmlRpt.ToDictionary(x => x.reportkey, x => x.crname);
            }
        }
    }
}
