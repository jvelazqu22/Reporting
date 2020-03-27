using System.Collections.Generic;
using System.Linq;

using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientQueries
{
    public class GetUserRpt2Query : IQuery<IList<userrpt2>>
    {
        public int ReportKey { get; set; }
        private readonly IClientQueryable _db;

        public GetUserRpt2Query(IClientQueryable db, int reportKey)
        {
            _db = db;
            ReportKey = reportKey;
        }
        
        public IList<userrpt2> ExecuteQuery()
        {
            using (_db)
            {
                return _db.UserRpt2.Where(s => s.reportkey.Equals(ReportKey)).ToList();
            }
        }
    }
}
