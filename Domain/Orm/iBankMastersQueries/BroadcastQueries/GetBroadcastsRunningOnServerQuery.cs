using System.Collections.Generic;
using System.Linq;
using Domain.Extensions;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.BroadcastQueries
{
    public class GetBroadcastsRunningOnServerQuery : IQuery<IList<bcstque4>>
    {
        private readonly IMastersQueryable _db;
        private readonly int _serverNumber;

        public GetBroadcastsRunningOnServerQuery(IMastersQueryable db, int serverNumber)
        {
            _db = db;
            _serverNumber = serverNumber;
        }
        public IList<bcstque4> ExecuteQuery()
        {
            using (_db)
            {
                return _db.BcstQue4.AreRunningBroadcasts()
                                   .AreOnServer(_serverNumber)
                                   .ToList();
            }
        }
    }
}
