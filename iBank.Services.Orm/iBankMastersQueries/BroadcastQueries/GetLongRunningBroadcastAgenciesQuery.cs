using Domain.Interfaces;
using iBank.Services.Orm.Databases.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Orm.iBankMastersQueries.BroadcastQueries
{
    public class GetLongRunningBroadcastAgenciesQuery : IQuery<IList<broadcast_long_running_agencies>>
    {
        private readonly IMastersQueryable _db;

        public GetLongRunningBroadcastAgenciesQuery(IMastersQueryable db)
        {
            _db = db;
        }

        public IList<broadcast_long_running_agencies> ExecuteQuery()
        {
            using (_db)
            {
                return _db.BroadcastLongRunningAgencies.Where(x => x.currently_processing_long_running).ToList();
            }
        }
    }
}
