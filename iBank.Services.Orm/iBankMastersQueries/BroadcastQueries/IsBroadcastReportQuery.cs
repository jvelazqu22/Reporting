using System;
using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries.BroadcastQueries
{
    public class IsBroadcastReportQuery : BaseiBankMastersQuery<bool>
    {
        public string ReportId { get; set; }

        public IsBroadcastReportQuery(IMastersQueryable db, string reportId)
        {
            _db = db;
            ReportId = reportId;
        }

        public override bool ExecuteQuery()
        {
            using (_db)
            {
                return _db.ReportHandoff.Where(x => x.reportid == ReportId && x.parmname.Trim().Equals("DOTNET_BCST", StringComparison.OrdinalIgnoreCase) 
                                                    && x.parmvalue.Trim().Equals("YES", StringComparison.OrdinalIgnoreCase))
                           .ToList().Count > 0;
            }
        }
    }
}
