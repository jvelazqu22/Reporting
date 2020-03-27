using System.Collections.Generic;
using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries.BroadcastQueries
{
    public class GetAllQueuedRecordsQuery : BaseiBankMastersQuery<IList<bcstque4>>
    {
        public GetAllQueuedRecordsQuery(IMastersQueryable db)
        {
            _db = db;
        }

        public override IList<bcstque4> ExecuteQuery()
        {
            using (_db)
            {
                return _db.BcstQue4.ToList();
            }
        }
    }
}
