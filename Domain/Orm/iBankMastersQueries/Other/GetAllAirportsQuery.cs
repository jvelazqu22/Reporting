using System.Collections.Generic;
using System.Linq;
using Domain.Orm.Classes;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetAllAirportsQuery : IQuery<IList<AirportInformation>>
    {
        private readonly IMastersQueryable _db;

        public GetAllAirportsQuery(IMastersQueryable db)
        {
            _db = db;
        }

        public IList<AirportInformation> ExecuteQuery()
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
                                                                                Longitude = s.longitude,
                                                                                AirportName = s.airportnam
                                                                            }).ToList();
            }
        }
    }
}
