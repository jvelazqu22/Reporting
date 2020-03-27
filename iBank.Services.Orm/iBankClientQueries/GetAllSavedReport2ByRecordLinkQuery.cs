using System.Collections.Generic;
using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankClientQueries
{
    public class GetAllSavedReport2ByRecordLinkQuery : BaseiBankClientQueryable<IList<savedrpt2>>
    {
        public int? Key { get; set; }
        public GetAllSavedReport2ByRecordLinkQuery(IClientQueryable db, int? key)
        {
            _db = db;
            Key = key;
        }

        public override IList<savedrpt2> ExecuteQuery()
        {
            using (_db)
            {
                return _db.SavedRpt2.Where(x => x.recordlink == Key).ToList();
            }
        }
    }
}
