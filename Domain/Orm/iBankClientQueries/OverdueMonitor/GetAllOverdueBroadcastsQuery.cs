using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientQueries.OverdueMonitor
{
    public class GetAllOverdueBroadcastsQuery : IQuery<IList<ibbatch>>
    {
        private readonly IClientQueryable _db;

        private readonly DateTime _threshold;

        public GetAllOverdueBroadcastsQuery(IClientQueryable db, DateTime threshold)
        {
            _db = db;
            _threshold = threshold;
        }

        public IList<ibbatch> ExecuteQuery()
        {
            using(var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() {  IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    using (_db)
                    {
                        //get all the broadcasts that are over the threshold
                        var batches = _db.iBBatch.Where(x => x.holdrun != "H"
                                                      && !x.errflag
                                                      && x.nextrun < _threshold).ToList();

                        return batches.Where(x => !string.IsNullOrEmpty(x.emailaddr)).ToList();
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
