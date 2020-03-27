using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries.BroadcastQueries
{
    public class IsExistingBroadcastKeyQuery : BaseiBankMastersQuery<bool>
    {
        public string Key { get; set; }

        public IsExistingBroadcastKeyQuery(IMastersQueryable db, string key)
        {
            _db = db;
            Key = key;
        }

        public override bool ExecuteQuery()
        {
            using (_db)
            {
                return _db.BcstRptInstance.Any(x => x.bcstrkey.Equals(Key));
            }
        }
    }
}
