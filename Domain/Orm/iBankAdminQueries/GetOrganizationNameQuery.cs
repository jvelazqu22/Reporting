using System.Linq;

using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankAdminQueries
{
    public class GetOrganizationNameQuery : IQuery<string>
    {
        private readonly IClientQueryable _db;

        private readonly int _key;

        public GetOrganizationNameQuery(IClientQueryable db, int key)
        {
            _db = db;
            _key = key;
        }

        public string ExecuteQuery()
        {
            using (_db)
            {
                var rec = _db.Organization.FirstOrDefault(x => x.OrgKey == _key);

                return rec == null ? "" : rec.OrgName;
            }
        }
    }
}
