using Domain.Interfaces;
using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Orm.iBankMastersQueries.BroadcastQueries
{
    public class GetMasterCorpAcctsBroadcastDatabasesQuery : IQuery<IList<DatabaseInformation>>
    {
        private readonly IMastersQueryable _db;
        public GetMasterCorpAcctsBroadcastDatabasesQuery(IMastersQueryable db)
        {
            _db = db;
        }

        public IList<DatabaseInformation> ExecuteQuery()
        {
            using (_db)
            {
                return _db.MstrCorpAccts.Where(s => s.active && s.bcactive)
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
