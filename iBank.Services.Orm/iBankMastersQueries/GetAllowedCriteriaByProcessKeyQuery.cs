using System.Collections.Generic;
using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetAllowedCriteriaByProcessKeyQuery : BaseiBankMastersQuery<IList<string>>
    {
        public int ProcessKey { get; set; }

        public GetAllowedCriteriaByProcessKeyQuery(IMastersQueryable db, int processKey)
        {
            _db = db;
            ProcessKey = processKey;
        }

        public override IList<string> ExecuteQuery()
        {
            using(_db)
            {
                return _db.iBProccrit.Where(s => s.processkey == ProcessKey)
                    .Join(_db.iBWhcrit, p => p.ibwcritkey, w => w.ibwcritkey,
                        (p, w) => w.webvarname.Trim().ToUpper()).ToList();
            }
        }
    }
}
