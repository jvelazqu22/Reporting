using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.BroadcastQueries
{
    public class GetBroadcastRecordsByBatchNumberAndDatabaseQuery : IQuery<IList<bcstque4>>
    {
        public int? BatchNumber { get; set; }
        public string DatabaseName { get; set; }

        private readonly IMastersQueryable _db;

        public GetBroadcastRecordsByBatchNumberAndDatabaseQuery(IMastersQueryable db, int? batchNumber, string databaseName)
        {
            _db = db;
            BatchNumber = batchNumber;
            DatabaseName = databaseName;
        }

        public IList<bcstque4> ExecuteQuery()
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    using (_db)
                    {
                        return _db.BcstQue4.Where(x => x.batchnum == BatchNumber
                                                       && x.dbname.Trim().Equals(DatabaseName, StringComparison.OrdinalIgnoreCase)).ToList();
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
