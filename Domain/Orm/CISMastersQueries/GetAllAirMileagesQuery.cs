using System;
using System.Collections.Generic;
using System.Linq;

using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.CISMastersQueries
{
    public class GetAllAirMileagesQuery : IQuery<Dictionary<Tuple<string, string>, int>>
    {
        private readonly ICisMastersQueryable _db;
        public GetAllAirMileagesQuery(ICisMastersQueryable db)
        {
            _db = db;
        }

        public Dictionary<Tuple<string, string>, int> ExecuteQuery()
        {
            using (_db)
            {
                var results = _db.AirMileage.OrderBy(x => x.Origin)
                                            .ThenBy(x => x.Destinat)
                                            .ToList();
                
                return results.ToDictionary(x => Tuple.Create(x.Origin, x.Destinat), x => (int)x.Mileage);
            }
        } 
    }
}
