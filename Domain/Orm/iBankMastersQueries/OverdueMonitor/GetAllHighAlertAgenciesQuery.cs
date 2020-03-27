using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.OverdueMonitor
{
    public class GetAllHighAlertAgenciesQuery : IQuery<IList<broadcast_high_alert_agency>>
    {
        private readonly IMastersQueryable _db;

        public GetAllHighAlertAgenciesQuery(IMastersQueryable db)
        {
            _db = db;
        }

        public IList<broadcast_high_alert_agency> ExecuteQuery()
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    using (_db)
                    {
                        //the null check only exists to allow unit test mocking to work correctly
                        return _db.BroadcastHighAlertAgency.Where(x => x != null).ToList();
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
