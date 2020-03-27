using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using Domain.Helper;

using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.OverdueMonitor
{
    public class GetAllActiveDatabasesQuery : IQuery<IList<DatabaseAddress>>
    {
        private readonly IMastersQueryable _db;

        public GetAllActiveDatabasesQuery(IMastersQueryable db)
        {
            _db = db;
        }

        public IList<DatabaseAddress> ExecuteQuery()
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    using (_db)
                    {
                        //get the databases from mstragcy
                        var mstrAgcyDatabases = _db.MstrAgcy.Where(x => x.active
                                                                        && x.bcactive.HasValue
                                                                        && x.bcactive.Value
                                                                        && !string.IsNullOrEmpty(x.databasename))
                                                            .Select(x => x.databasename.Trim())
                                                            .Distinct().ToList();

                        //get the databases from mstrcorpaccts
                        var mstrCorpDatabases = _db.MstrCorpAccts.Where(x => x.active
                                                                        && x.bcactive
                                                                        && !string.IsNullOrEmpty(x.databasename))
                                                                  .Select(x => x.databasename.Trim())
                                                                  .Distinct().ToList();
                        
                        //combine the lists
                        mstrAgcyDatabases.AddRange(mstrCorpDatabases);
                        var combinedDatabases = mstrCorpDatabases.Distinct();

                        //now get the server address from iBankDatabases
                        return _db.iBankDatabases.Where(x => combinedDatabases.Contains(x.databasename.Trim()))
                                                           .Select(x => new DatabaseAddress {  DatabaseName = x.databasename, ServerAddress = x.server_address})
                                                           .ToList();
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
