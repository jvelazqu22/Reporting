using System.Collections.Generic;
using System.Linq;
using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetAllAirportsQuery : BaseiBankMastersQuery<IList<AirportInformation>>
    {
        public GetAllAirportsQuery(IMastersQueryable db)
        {
            _db = db;
        }

        public override IList<AirportInformation> ExecuteQuery()
        {
            using (_db)
            {
                return _db.Airports.OrderBy(x => x.airport).Select(s => new AirportInformation
                                                                            {
                                                                                RecordNumber = s.recordno,
                                                                                Airport = s.airport.Trim(),
                                                                                Mode = s.mode.Trim(),
                                                                                Country = s.country.Trim(),
                                                                                Region = s.region.Trim(),
                                                                                RRCarrier = s.rrcarrier.Trim(),
                                                                                City = s.city.Trim(),
                                                                                State = s.state.Trim(),
                                                                                Metro = s.metro.Trim(),
                                                                                CountryCode = s.CtryCode.Trim(),
                                                                                RegionCode = s.RegionCode.Trim(),
                                                                                Latitude = s.latitude,
                                                                                Longitude = s.longitude
                                                                            }).ToList();
            }
        }
    }
}
