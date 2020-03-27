using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetReportLogResultsByLogKeyQuery : BaseiBankMastersQuery<ibRptLogResults>
    {
        public int ReportLogKey { get; set; }

        public GetReportLogResultsByLogKeyQuery(IMastersQueryable db, int reportLogKey)
        {
            _db = db;
            ReportLogKey = reportLogKey;
        }
        public override ibRptLogResults ExecuteQuery()
        {
            using(_db)
            {
                return _db.iBRptLogResults.FirstOrDefault(x => x.rptlogno == ReportLogKey);
            }
        }
    }
}
