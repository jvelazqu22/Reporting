using Domain.Interfaces;
using iBank.Services.Orm.Databases.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Orm.iBankAdminQueries
{
    public class GetLoadLogs : IQuery<IList<ibuser>>
    {
        IClientQueryable _db;
        public GetLoadLogs(IClientQueryable db)
        {
            _db = db;
        }

        public IList<ibuser> ExecuteQuery()
        {
            /*
             select t2.recordno as llrecno, t2.loaddate, 
		     t2.loadtype, t2.sourceabbr, t2.sourcever, t2.gds_bo, t2.loadmsg, t2.triprecs, t3.startdate, t3.enddate
		     from MstrAgcySources T1 inner join LoadLog T2 on t1.sourceabbr = t2.sourceabbr and T1.agency = T2.agency 
		     inner join LoadLogExtras T3 on t2.recordno = t3.llrecno
		     where T1.agency = 'demo' and T1.SourceAbbr is not null 
		     and T2.LoadType is not null and len(ltrim(T2.LoadType)) > 1 
              */
            using (_db)
            {
                return _db.iBUser.ToList();
            }
        }
    }
}
