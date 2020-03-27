using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm
{
    public class GetQueuedReportHandoffsQuery : IQuery<List<reporthandoff>>
    {
        private readonly IMastersQueryable _db;

        public GetQueuedReportHandoffsQuery(IMastersQueryable db)
        {
            _db = db;

        }

        public List<reporthandoff> ExecuteQuery()
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions() {IsolationLevel = IsolationLevel.ReadUncommitted}))
            {
                try
                {
                    var pendingReports = _db.ReportHandoff.Where(x => x.svrnumber == 0
                                                                      && x.parmname.Trim().Equals("REPORTSTATUS",
                                                                          StringComparison.OrdinalIgnoreCase)
                                                                      && x.parmvalue.Trim().Equals("PENDING",
                                                                          StringComparison.OrdinalIgnoreCase)
                                                                      && !x.reportid.EndsWith("_NET"))
                                                            .ToList();

                    return pendingReports;
                }
                finally
                {
                    scope.Complete();
                }
            }
        }
    }
}
