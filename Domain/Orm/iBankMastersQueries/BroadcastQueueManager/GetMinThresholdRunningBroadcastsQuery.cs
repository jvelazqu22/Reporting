using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Extensions;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.BroadcastQueueManager
{
    public class GetMinThresholdRunningBroadcastsQuery : IQuery<IEnumerable<bcstque4>>
    {
        private readonly IMastersQueryable _db;
        private readonly DateTime _threshold;

        public GetMinThresholdRunningBroadcastsQuery(IMastersQueryable db, DateTime threshold)
        {
            _db = db;
            _threshold = threshold;
        }

        public IEnumerable<bcstque4> ExecuteQuery()
        {
            using (_db)
            {
                return _db.BcstQue4.AreRunningBroadcasts()
                                   .HaveRunningTimeOverThreshold(_threshold)
                                   .ToList();
            }
        }
    }
}
