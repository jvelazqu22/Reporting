using System;
using System.Linq;

using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetRunningReportQuery : BaseiBankMastersQuery<ibRunningRpts>
    {
        public int ReportLogKey { get; set; }

        public GetRunningReportQuery(IMastersQueryable db, int reportLogKey)
        {
            _db = db;
            ReportLogKey = reportLogKey;
        }

        public override ibRunningRpts ExecuteQuery()
        {
            using (_db)
            {

                return _db.iBRunningRpts.FirstOrDefault(s => s.reportlogno == ReportLogKey);

            }
        }
    }
}
