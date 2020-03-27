using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankClientQueries
{
    public class GetSavedReport1ByKeyQuery : BaseiBankClientQueryable<savedrpt1>
    {
        public int SavedReportKey { get; set; }
        public GetSavedReport1ByKeyQuery(IClientQueryable db, int savedReportKey)
        {
            _db = db;
            SavedReportKey = savedReportKey;
        }

        public override savedrpt1 ExecuteQuery()
        {
            using (_db)
            {
                return _db.SavedRpt1.FirstOrDefault(x => x.recordnum == SavedReportKey);
            }
        }
    }
}
