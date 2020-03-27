using System.Collections.Generic;
using System.Linq;

using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientQueries
{
    public class GetTravelOptixReport2Query
    {
        public int ReportKey { get; set; }
        private readonly IClientQueryable _db;

        public GetTravelOptixReport2Query(IClientQueryable db, int reportKey)
        {
            _db = db;
            ReportKey = reportKey;
        }

        public IList<TravelOptixReport2> ExecuteQuery()
        {
            using (_db)
            {
                return _db.TravelOptixReport2.Where(s => s.ReportKey.Equals(ReportKey)).ToList();
            }
        }
    }
}
