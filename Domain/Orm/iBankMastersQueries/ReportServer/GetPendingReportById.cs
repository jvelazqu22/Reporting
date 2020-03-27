using System.Linq;
using System.Transactions;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;
using iBank.Entities.MasterEntities;

namespace Domain.Orm.iBankMastersQueries.ReportServer
{
    public class GetPendingReportByIdQuery : IQuery<PendingReports>
    {
        private IMastersQueryable _db;
        private string _reportId;

        public GetPendingReportByIdQuery(IMastersQueryable db, string reportId)
        {
            _db = db;
            _reportId = reportId;
        }

        public PendingReports ExecuteQuery()
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    using (_db)
                    {
                        return _db.PendingReports.FirstOrDefault(w => w.ReportId.Equals(_reportId));
                    }
                }
                finally
                {
                    scope.Complete();
                }
            }

        }
    }
}
