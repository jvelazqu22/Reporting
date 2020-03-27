using Domain.Interfaces;
using iBank.Services.Orm.Databases.Interfaces;
using System.Linq;

namespace iBank.Services.Orm.iBankMastersQueries
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
