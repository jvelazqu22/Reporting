using System.Linq;

using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankAdminQueries
{
    public class GetStyleGroupNameQuery : IQuery<string>
    {
        private readonly IClientQueryable _db;

        private readonly int _key;

        public GetStyleGroupNameQuery(IClientQueryable db, int key)
        {
            _db = db;
            _key = key;
        }

        public string ExecuteQuery()
        {
            using (_db)
            {
                var rec = _db.StyleGroup.FirstOrDefault(x => x.SGroupNbr == _key);

                return rec == null ? "" : rec.SGroupName;
            }
        }
    }
}
