using System.Collections.Generic;
using System.Linq;
using Domain.Orm.Classes;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetAllRailroadStationsQuery : IQuery<IList<RRStationInformation>>
    {
        private readonly IMastersQueryable _db;

        public GetAllRailroadStationsQuery(IMastersQueryable db)
        {
            _db = db;
        }
        public IList<RRStationInformation> ExecuteQuery()
        {
            using(_db)
            {
                return _db.RRStations.OrderBy(x => x.RStationNo).Select(s => new RRStationInformation
                                                                                 {
                                                                                     StationNumber = s.RStationNo,
                                                                                     StationName = s.stationName,
                                                                                     City = s.city,
                                                                                     State = s.state,
                                                                                     Metro = s.metro,
                                                                                     CountryCode = s.ctryCode,
                                                                                     RegionCode = s.regionCode,
                                                                                     GroupCode = s.groupCode
                                                                                 }).ToList();
            }
        }
    }
}
