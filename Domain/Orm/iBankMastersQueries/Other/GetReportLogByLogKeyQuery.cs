using System.Linq;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetReportLogByLogKeyQuery : IQuery<ibRptLog>
    {
        public int ReportLogKey { get; set; }
        private readonly IMastersQueryable _db;

        public GetReportLogByLogKeyQuery(IMastersQueryable db, int reportLogKey)
        {
            _db = db;
            ReportLogKey = reportLogKey;
        }
        public ibRptLog ExecuteQuery()
        {
            using(_db)
            {
                return _db.iBRptLog.FirstOrDefault(x => x.rptlogno == ReportLogKey);
            }
        }
    }
}
