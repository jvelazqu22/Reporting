using System.Linq;

using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientQueries
{
    public class GetSavedReport1ByKeyQuery : IQuery<savedrpt1>
    {
        public int SavedReportKey { get; set; }

        private readonly IClientQueryable _db;

        public GetSavedReport1ByKeyQuery(IClientQueryable db, int savedReportKey)
        {
            _db = db;
            SavedReportKey = savedReportKey;
        }

        public savedrpt1 ExecuteQuery()
        {
            using (_db)
            {
                return _db.SavedRpt1.FirstOrDefault(x => x.recordnum == SavedReportKey);
            }
        }
    }
}
