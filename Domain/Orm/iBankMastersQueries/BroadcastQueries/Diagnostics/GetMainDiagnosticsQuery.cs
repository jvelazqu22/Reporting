using System.Linq;
using Domain.Models.WebModels.BroadcastDiagnostics;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.BroadcastQueries.Diagnostics
{
    public class GetMainDiagnosticsQuery : IQuery<MainDiagnostics>
    {
        private readonly IMastersQueryable _db;

        public GetMainDiagnosticsQuery(IMastersQueryable db)
        {
            _db = db;
        }
        public MainDiagnostics ExecuteQuery()
        {
            using (_db)
            {
                //_db.Database.CommandTimeout = 5000;
                var mainDiags = _db.Database.SqlQuery<MainDiagnostics>("EXEC Dbo.GetMainBroadcastDiagnostics").FirstOrDefault();
                return mainDiags;
            }
        }
    }
}
