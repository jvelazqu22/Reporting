using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetReportLogKeyByUserNumberAndProcessKeyQuery : BaseiBankMastersQuery<int>
    {
        private int UserNumber { get; set; }
        private int ProcessKey { get; set; }

        public GetReportLogKeyByUserNumberAndProcessKeyQuery(IMastersQueryable db, int userNumber, int processKey)
        {
            UserNumber = userNumber;
            ProcessKey = processKey;
            _db = db;
        }
        public override int ExecuteQuery()
        {
            using (_db)
            {
                return _db.iBRptLog.Where(x => x.usernumber == UserNumber && x.processkey == ProcessKey)
                                   .OrderByDescending(x => x.submittime)
                                   .Select(x => x.rptlogno)
                                   .FirstOrDefault();

            }
        }
    }
}
