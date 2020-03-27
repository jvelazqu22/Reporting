using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetMaxBroadcastSeqNumberQuery : BaseiBankMastersQuery<int>
    {
        public GetMaxBroadcastSeqNumberQuery(IMastersQueryable db)
        {
            _db = db;
        }
        public override int ExecuteQuery()
        {
            using (_db)
            {
                return _db.BcstQue4.Select(x => x.bcstseqno).DefaultIfEmpty().Max();
            }
        }
    }
}
