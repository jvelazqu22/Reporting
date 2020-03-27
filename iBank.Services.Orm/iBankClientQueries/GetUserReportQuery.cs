
using System.Linq;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankClientQueries
{
    public class GetUserReportQuery : BaseiBankClientQueryable<userrpt>
    {
        public int ReportKey { get; set; }
        public GetUserReportQuery(IClientQueryable db, int reportKey)
        {
            _db = db;
            ReportKey = reportKey;
        }

        public override userrpt ExecuteQuery()
        {
            using (_db)
            {
                return _db.UserRpt.FirstOrDefault(s => s.reportkey.Equals(ReportKey));
            }
        }

    }
}
