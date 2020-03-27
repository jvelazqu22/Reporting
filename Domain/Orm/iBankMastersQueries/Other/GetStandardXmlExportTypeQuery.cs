using System.Linq;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetStandardXmlExportTypeQuery : IQuery<xmlrpts>
    {
        private readonly IMastersQueryable _db;
        public int ReportKey { get; set; }
        public GetStandardXmlExportTypeQuery(IMastersQueryable db, int reportKey)
        {
            _db = db;
            ReportKey = reportKey;
        }

        public xmlrpts ExecuteQuery()
        {
            using (_db)
            {
                return _db.XmlRpt.FirstOrDefault(s => s.reportkey.Equals(ReportKey));
            }
        }
    }
}
