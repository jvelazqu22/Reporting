using System;
using System.Linq;
using System.Transactions;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.ReportQueueManager
{
    public class IsOnPendingReportsTableQuery : IQuery<bool>
    {
        private readonly IMastersQueryable _db;
        private readonly string _reportId;

        public IsOnPendingReportsTableQuery(IMastersQueryable db, string reportId)
        {
            _db = db;
            _reportId = reportId.Trim();
        }

        public bool ExecuteQuery()
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions() {IsolationLevel = IsolationLevel.ReadUncommitted}))
            {
                try
                {
                    var rec = _db.PendingReports.FirstOrDefault(x => x.ReportId.Equals(_reportId, StringComparison.OrdinalIgnoreCase));

                    return rec != null;
                }
                finally
                {
                    scope.Complete();
                }
            }
        }
    }
}
