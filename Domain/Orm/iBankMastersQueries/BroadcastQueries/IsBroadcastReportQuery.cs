using System;
using System.Linq;

using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.BroadcastQueries
{
    public class IsBroadcastReportQuery : IQuery<bool>
    {
        public string ReportId { get; set; }
        private readonly IMastersQueryable _db;

        public IsBroadcastReportQuery(IMastersQueryable db, string reportId)
        {
            _db = db;
            ReportId = reportId;
        }

        public bool ExecuteQuery()
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
