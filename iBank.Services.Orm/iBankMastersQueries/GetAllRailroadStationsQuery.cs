using System.Collections.Generic;
using System.Linq;

using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetAllRailroadStationsQuery : BaseiBankMastersQuery<IList<RRStationInformation>>
    {
        public GetAllRailroadStationsQuery(IMastersQueryable db)
        {
            _db = db;
        }
        public override IList<RRStationInformation> ExecuteQuery()
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
