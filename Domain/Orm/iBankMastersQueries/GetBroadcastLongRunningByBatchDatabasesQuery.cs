using System.Collections.Generic;
using System.Linq;
using Domain.Extensions;
using Domain.Orm.Classes;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;
using Domain.Orm.iBankMastersQueries.BroadcastQueries;

namespace Domain.Orm.iBankMastersQueries
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
                List<string> agencies = new List<string>();

                var bcstque = _db.BcstQue4.FirstOrDefault(x => x.batchnum == _batchNumber); 

                //if the agency is regular agency
                result.AddRange(_db.MstrAgcy.Where(s => s.agency == bcstque.agency)
                        .Select(s => new DatabaseInformation
                        {
                            DatabaseName = s.databasename.Trim().ToLower(),
                            TimeZoneOffset = s.tzoffset ?? 0
                        })
                        .ToList());

                //if the agency is corpacct
                var agies = _db.JunctionAgcyCorp.Where(x => x.CorpAcct == bcstque.agency)
                        .Select(x => x.agency)
                        .ToList();

                if (agies.Any())
                {
                    result.AddRange(_db.MstrAgcy.Where(s => agies.Contains(s.agency))
                        .Select(s => new DatabaseInformation
                        {
                            DatabaseName = s.databasename.Trim().ToLower(),
                            TimeZoneOffset = s.tzoffset ?? 0
                        })
                        .ToList());
                }
            }

            return result.GroupBy(p => p.DatabaseName)
                  .Select(g => g.First())
                  .OrderBy(x => x.DatabaseName)
                  .ToList();
        }
    }
}
