using System.Collections.Generic;
using System.Linq;

using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases;

namespace iBank.Services.Orm.CISMastersQueries
{
    public class GetAllAirMileagesQuery : BaseCISMastersQuery<List<AirMileageInformation>>
    {
        public GetAllAirMileagesQuery(CisMastersQueryable db)
        {
            _db = db;
        }

        public override List<AirMileageInformation> ExecuteQuery()
        {
            using(_db)
            {
                return _db.AirMileage.OrderBy(x => x.Origin).ThenBy(x => x.Destinat)
                    .Select(x => new AirMileageInformation
                                     {
                                         RecordNumber = x.RecordNo,
                                         Origin = x.Origin.Trim(),
                                         Destination = x.Destinat.Trim(),
                                         Mileage = x.Mileage
                                     }).ToList();
            }
        }
    }
}
