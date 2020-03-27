using System.Linq;

using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientQueries
{
    public class GetCustomXmlExportTypeQuery : IQuery<xmluserrpts>
    {
        private readonly IClientQueryable _db;
        public int ReportKey { get; set; }
        public GetCustomXmlExportTypeQuery(IClientQueryable db, int reportKey)
        {
            _db = db;
            ReportKey = reportKey;
        }

        public xmluserrpts ExecuteQuery()
        {
            using (_db)
            {
                return _db.XmlUserRpt.FirstOrDefault(s => s.reportkey.Equals(ReportKey));
            }
        }
    }
}
