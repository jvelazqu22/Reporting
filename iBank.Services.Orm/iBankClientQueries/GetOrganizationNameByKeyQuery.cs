using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankClientQueries
{
    public class GetOrganizationNameByKeyQuery : BaseiBankClientQueryable<string>
    {
        public int OrganizationKey { get; set; }

        public GetOrganizationNameByKeyQuery(IClientQueryable db, int organizationKey)
        {
            _db = db;
            OrganizationKey = organizationKey;
        }

        public override string ExecuteQuery()
        {
            using (_db)
            {
                var org = _db.Organization.FirstOrDefault(x => x.OrgKey == OrganizationKey);

                return org == null ? "" : org.OrgName.Trim();
            }
        }
    }
}
