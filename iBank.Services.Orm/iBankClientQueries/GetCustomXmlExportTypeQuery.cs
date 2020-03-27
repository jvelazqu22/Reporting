using Domain.Interfaces;
using iBank.Services.Orm.Databases.Interfaces;
using System.Linq;

namespace iBank.Services.Orm.iBankClientQueries
{
    public class GetCustomXmlExportTypeQuery : IQuery<xmluserrpt>
    {
        private readonly IClientQueryable _db;
        public int ReportKey { get; set; }
        public GetCustomXmlExportTypeQuery(IClientQueryable db, int reportKey)
        {
            _db = db;
            ReportKey = reportKey;
        }

        public xmluserrpt ExecuteQuery()
        {
            using (_db)
            {
                return _db.XmlUserRpt.FirstOrDefault(s => s.reportkey.Equals(ReportKey));
            }
        }
    }
}
