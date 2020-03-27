using System.Linq;

using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientQueries
{
    public class GetOrganizationNameByKeyQuery : IQuery<string>
    {
        public int OrganizationKey { get; set; }

        private readonly IClientQueryable _db;

        public GetOrganizationNameByKeyQuery(IClientQueryable db, int organizationKey)
        {
            _db = db;
            OrganizationKey = organizationKey;
        }

        public string ExecuteQuery()
        {
            using (_db)
            {
                var org = _db.Organization.FirstOrDefault(x => x.OrgKey == OrganizationKey);

                return org == null ? "" : org.OrgName.Trim();
            }
        }
    }
}
