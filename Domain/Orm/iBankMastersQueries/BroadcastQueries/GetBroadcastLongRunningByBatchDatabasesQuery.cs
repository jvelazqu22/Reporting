using System.Collections.Generic;
using System.Linq;
using Domain.Extensions;
using Domain.Orm.Classes;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.BroadcastQueries
{
    public class GetBroadcastLongRunningByBatchDatabasesQuery : IQuery<List<DatabaseInformation>>
    {
        private readonly IMastersQueryable _db;
        private readonly int _batchNumber;

        public GetBroadcastLongRunningByBatchDatabasesQuery(IMastersQueryable db, int batchNumber)
        {
            _db = db;
            _batchNumber = batchNumber;
        }
        public List<DatabaseInformation> ExecuteQuery()
        {
            List<DatabaseInformation> result = new List<DatabaseInformation>();
            using (_db)
            {

                IQueryable<bcstque4> bcstque = _db.BcstQue4.Where(x => x.batchnum == _batchNumber);

                var agencyDatabases = _db.MstrAgcy.ToDatabaseInformation(bcstque);
                result.AddRange(agencyDatabases);

                var shareCorpAcctDatabases = _db.MstrAgcy.ToDatabaseInformation(bcstque, _db.JunctionAgcyCorp);
                result.AddRange(shareCorpAcctDatabases);

            }

            return result.GroupBy(p => p.DatabaseName)
                  .Select(g => g.First())
                  .OrderBy(x => x.DatabaseName)
                  .ToList();
        }

    }
}
