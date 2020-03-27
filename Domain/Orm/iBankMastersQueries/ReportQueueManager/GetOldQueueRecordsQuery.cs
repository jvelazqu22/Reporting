using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Extensions;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.ReportQueueManager
{
    public class GetOldQueueRecordsQuery : IQuery<IList<PendingReports>>
    {
        private readonly IMastersQueryable _db;
        private readonly DateTime _threshold;

        public GetOldQueueRecordsQuery(IMastersQueryable db, DateTime threshold)
        {
            _db = db;
            _threshold = threshold;
        }

        public IList<PendingReports> ExecuteQuery()
        {
            using (var scope = TransactionScopeBuilder.BuildNoLockScope())
            {
                try
                {
                    using (_db)
                    {
                        return (from pr in _db.PendingReports
                                where pr.TimeStamp < _threshold
                                select pr).ToList();

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
