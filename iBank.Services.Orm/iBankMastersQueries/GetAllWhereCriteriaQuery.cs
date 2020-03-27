using System.Collections.Generic;
using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetAllWhereCriteriaQuery : BaseiBankMastersQuery<Dictionary<int, string>>
    {
        public GetAllWhereCriteriaQuery(IMastersQueryable db)
        {
            _db = db;
        }

        public override Dictionary<int, string> ExecuteQuery()
        {
            using (_db)
            {
                return _db.iBWhcrit.ToDictionary(k => k.ibwcritkey, v => v.webvarname.ToUpper().Trim());
            }
        }
    }
}
