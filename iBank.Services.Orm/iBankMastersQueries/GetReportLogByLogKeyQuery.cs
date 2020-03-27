using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetReportLogByLogKeyQuery : BaseiBankMastersQuery<ibRptLog>
    {
        public int ReportLogKey { get; set; }

        public GetReportLogByLogKeyQuery(IMastersQueryable db, int reportLogKey)
        {
            _db = db;
            ReportLogKey = reportLogKey;
        }
        public override ibRptLog ExecuteQuery()
        {
            using(_db)
            {
                return _db.iBRptLog.FirstOrDefault(x => x.rptlogno == ReportLogKey);
            }
        }
    }
}
