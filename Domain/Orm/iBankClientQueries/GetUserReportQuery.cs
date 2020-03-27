using System.Linq;

using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientQueries
{
    public class GetUserReportQuery : IQuery<userrpts>
    {
        public int ReportKey { get; set; }
        private readonly IClientQueryable _db;

        public GetUserReportQuery(IClientQueryable db, int reportKey)
        {
            _db = db;
            ReportKey = reportKey;
        }

        public userrpts ExecuteQuery()
        {
            using (_db)
            {
                return _db.UserRpt.FirstOrDefault(s => s.reportkey.Equals(ReportKey));
            }
        }

    }
}
