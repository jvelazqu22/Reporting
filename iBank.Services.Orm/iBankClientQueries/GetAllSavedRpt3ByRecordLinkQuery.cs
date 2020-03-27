using System.Collections.Generic;
using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankClientQueries
{
    public class GetAllSavedReport3ByRecordLinkQuery : BaseiBankClientQueryable<IList<savedrpt3>>
    {
        public int? Key { get; set; }
        public GetAllSavedReport3ByRecordLinkQuery(IClientQueryable db, int? key)
        {
            _db = db;
            Key = key;
        }

        public override IList<savedrpt3> ExecuteQuery()
        {
            using (_db)
            {
                return _db.SavedRpt3.Where(x => x.recordlink == Key).ToList();
            }
        }
    }
}
