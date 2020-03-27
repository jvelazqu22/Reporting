using System.Collections.Generic;
using System.Linq;
using Domain.Extensions;
using Domain.Orm.Classes;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.BroadcastQueries
{
    public class GetBroadcastLongRunningOtherDatabasesQuery : IQuery<List<DatabaseInformation>>
    {

        private readonly IMastersQueryable _db;
        private readonly List<int> _otherLongRunningServerNumbers;

        public GetBroadcastLongRunningOtherDatabasesQuery(IMastersQueryable db, List<int> otherLongRunningServerNumbers)
        {
            _db = db;
            _otherLongRunningServerNumbers = otherLongRunningServerNumbers;
        }

        public List<DatabaseInformation> ExecuteQuery()
        {
            List<DatabaseInformation> result = new List<DatabaseInformation>();
            using (_db)
            {
                foreach (var serverNumber in _otherLongRunningServerNumbers)
                {
                    //must be running
                    var bcstque = _db.BcstQue4.AreRunningBroadcasts()
                                              .AreOnServer(serverNumber);

                    var agencyDatabases = _db.MstrAgcy.ToDatabaseInformation(bcstque);
                    result.AddRange(agencyDatabases);

                    var shareCorpAcctDatabases = _db.MstrAgcy.ToDatabaseInformation(bcstque, _db.JunctionAgcyCorp);
                    result.AddRange(shareCorpAcctDatabases);
                    
                }
            }

            return result.GroupBy(p => p.DatabaseName)
                  .Select(g => g.First())
                  .OrderBy(x => x.DatabaseName)
                  .ToList();
        }
    }
}