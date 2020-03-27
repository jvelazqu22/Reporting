using System.Linq;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetRunningReportQuery : IQuery<ibRunningRpts>
    {
        public int ReportLogKey { get; set; }

        private readonly IMastersQueryable _db;

        public GetRunningReportQuery(IMastersQueryable db, int reportLogKey)
        {
            _db = db;
            ReportLogKey = reportLogKey;
        }

        public ibRunningRpts ExecuteQuery()
        {
            using (_db)
            {

                return _db.iBRunningRpts.FirstOrDefault(s => s.reportlogno == ReportLogKey);

            }
        }
    }
}
