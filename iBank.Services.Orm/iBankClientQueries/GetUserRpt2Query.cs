using System.Collections.Generic;
using System.Linq;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankClientQueries
{
    public class GetUserRpt2Query : BaseiBankClientQueryable<IList<userrpt2>>
    {
        public int ReportKey { get; set; }
        public GetUserRpt2Query(IClientQueryable db, int reportKey)
        {
            _db = db;
            ReportKey = reportKey;
        }
        
        public override IList<userrpt2> ExecuteQuery()
        {
            using (_db)
            {
                return _db.UserRpt2.Where(s => s.reportkey.Equals(ReportKey)).ToList();
            }
        }
    }
}
