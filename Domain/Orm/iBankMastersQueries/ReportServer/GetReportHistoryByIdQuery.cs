using System.Linq;
using System.Transactions;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;
using iBank.Entities.MasterEntities;

namespace Domain.Orm.iBankMastersQueries.ReportServer
{
    public class GetReportHistoryByIdQuery : IQuery<ReportHistory>
    {
        private IMastersQueryable _db;
        private string _reportId;

        public GetReportHistoryByIdQuery(IMastersQueryable db, string reportId)
        {
            _db = db;
            _reportId = reportId;
        }

        public ReportHistory ExecuteQuery()
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    using (_db)
                    {
                        return _db.ReportHistory.FirstOrDefault(w => w.reportid.Equals(_reportId));
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
