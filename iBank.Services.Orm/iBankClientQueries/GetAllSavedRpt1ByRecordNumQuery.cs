using System.Collections.Generic;
using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankClientQueries
{
    public class GetAllSavedRpt1ByRecordNumQuery : BaseiBankClientQueryable<IList<savedrpt1>>
    {
        public int? Key { get; set; }
        public GetAllSavedRpt1ByRecordNumQuery(IClientQueryable db, int? key)
        {
            _db = db;
            Key = key;
        }

        public override IList<savedrpt1> ExecuteQuery()
        {
            using (_db)
            {
                return _db.SavedRpt1.Where(x => x.recordnum == Key).ToList();
            }
        }
    }
}
