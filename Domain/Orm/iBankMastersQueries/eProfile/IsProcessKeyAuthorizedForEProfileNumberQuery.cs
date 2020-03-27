using System.Linq;

using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.eProfile
{
    public class IsProcessKeyAuthorizedForEProfileNumberQuery : IQuery<bool>
    {
        private int ProcessKey { get; }
        private int EProfileNumber { get; }

        private readonly IMastersQueryable _db;

        public IsProcessKeyAuthorizedForEProfileNumberQuery(IMastersQueryable db, int processKey, int eProfileNumber)
        {
            _db = db;
            ProcessKey = processKey;
            EProfileNumber = eProfileNumber;
        }
        public bool ExecuteQuery()
        {
            using (_db)
            {
                var rec = _db.EProfileProcs.FirstOrDefault(x => x.eProfileNo == EProfileNumber && x.ProcessKey == ProcessKey);

                return rec != null;
            }
        }
    }
}
