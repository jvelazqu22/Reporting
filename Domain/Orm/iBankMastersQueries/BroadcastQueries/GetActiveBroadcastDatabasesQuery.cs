using System.Collections.Generic;
using System.Linq;

using Domain.Orm.Classes;

using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.BroadcastQueries
{
    public class GetActiveBroadcastDatabasesQuery : IQuery<IList<DatabaseInformation>>
    {
        private readonly IMastersQueryable _db;
        public GetActiveBroadcastDatabasesQuery(IMastersQueryable db)
        {
            _db = db;
        }

        public IList<DatabaseInformation> ExecuteQuery()
        {
            using (_db)
            {
                return _db.MstrAgcy.Where(s => s.active 
                                                && s.bcactive.HasValue 
                                                && s.bcactive.Value 
                                                && !string.IsNullOrEmpty(s.databasename))
                    .Select(s => new DatabaseInformation
                                        {
                                            DatabaseName = s.databasename.Trim().ToLower(),
                                            TimeZoneOffset = s.tzoffset ?? 0
                                        })
                    .Distinct().ToList();
            }
        }
    }
}
