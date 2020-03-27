using System.Linq;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetReportLogResultsByLogKeyQuery : IQuery<ibRptLogResults>
    {
        public int ReportLogKey { get; set; }
        private readonly IMastersQueryable _db;

        public GetReportLogResultsByLogKeyQuery(IMastersQueryable db, int reportLogKey)
        {
            _db = db;
            ReportLogKey = reportLogKey;
        }
        public ibRptLogResults ExecuteQuery()
        {
            using(_db)
            {
                return _db.iBRptLogResults.FirstOrDefault(x => x.rptlogno == ReportLogKey);
            }
        }
    }
}
