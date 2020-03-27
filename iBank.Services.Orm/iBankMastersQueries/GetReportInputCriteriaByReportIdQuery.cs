using System.Collections.Generic;
using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetReportInputCriteriaByReportIdQuery : BaseiBankMastersQuery<IList<reporthandoff>>
    {
        public string ReportId { get; set; }

        public GetReportInputCriteriaByReportIdQuery(IMastersQueryable db, string reportId)
        {
            _db = db;
            ReportId = reportId;
        }

        public override IList<reporthandoff> ExecuteQuery()
        {
            using(_db)
            {
                return _db.ReportHandoff.Where(x => x.reportid == ReportId && x.parminout.ToUpper() == "IN").ToList();
            }
        }
    }
}
