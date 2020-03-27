using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries.BroadcastQueries
{
    public class GetNextAvailableBroadcastKeyQuery : BaseiBankMastersQuery<int>
    {
        public GetNextAvailableBroadcastKeyQuery(IMastersQueryable db)
        {
            _db = db;
        }

        public override int ExecuteQuery()
        {
            using (_db)
            {
                var ids = _db.BcstQue4.Select(x => x.bcstseqno).ToList();

                if (ids.Count == 0) return 1;

                return (from n in ids
                        where !ids.Select(x => x).Contains(n + 1)
                        orderby n
                        select n + 1).First();
            }
        }
    }
}
