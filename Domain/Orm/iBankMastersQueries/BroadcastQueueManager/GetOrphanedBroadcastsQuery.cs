using System.Collections.Generic;
using System.Linq;
using Domain.Extensions;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.BroadcastQueueManager
{
    public class GetOrphanedBroadcastsQuery : IQuery<IList<bcstque4>>
    {
        private readonly IMastersQueryable _db;

        public GetOrphanedBroadcastsQuery(IMastersQueryable db)
        {
            _db = db;
        }

        public IList<bcstque4> ExecuteQuery()
        {
            using (_db)
            {
                var errorRecs = _db.BcstQue4.AreErroredBroadcasts().ToList();

                var doneRecs = _db.BcstQue4.AreFinishedBroadcasts().ToList();

                errorRecs.AddRange(doneRecs);

                return errorRecs;
            }
        }
    }
}
