using System.Linq;

using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.BroadcastQueries
{
    public class IsExistingBroadcastKeyQuery : IQuery<bool>
    {
        public string Key { get; set; }
        private readonly IMastersQueryable _db;

        public IsExistingBroadcastKeyQuery(IMastersQueryable db, string key)
        {
            _db = db;
            Key = key;
        }

        public bool ExecuteQuery()
        {
            using (_db)
            {
                return _db.BcstRptInstance.Any(x => x.bcstrkey.Equals(Key));
            }
        }
    }
}
