using System.Linq;

using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;


namespace Domain.Orm.iBankClientQueries
{
    public class GetTravelOptixReportQuery
    {
        public int ReportKey { get; set; }
        private readonly IClientQueryable _db;

        public GetTravelOptixReportQuery(IClientQueryable db, int reportKey)
        {
            _db = db;
            ReportKey = reportKey;
        }

        public TravelOptixReports ExecuteQuery()
        {
            using (_db)
            {
                return _db.TravelOptixReports.FirstOrDefault(s => s.ReportKey.Equals(ReportKey));
            }
        }

    }
}
