using System.Linq;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetReportStatusHandoffRecordQuery : IQuery<reporthandoff>
    {
        public string ReportId { get; set; }
        public string Agency { get; set; }
        public int UserNumber { get; set; }
        public string ColdFusionBox { get; set; }
        private readonly IMastersQueryable _db;

        public GetReportStatusHandoffRecordQuery(IMastersQueryable db, string reportId, string agency, int userNumber, 
            string coldFusionBox)
        {
            _db = db;
            ReportId = reportId;
            Agency = agency;
            UserNumber = userNumber;
            ColdFusionBox = coldFusionBox;
        }

        public reporthandoff ExecuteQuery()
        {
            using(_db)
            {
                return _db.ReportHandoff.FirstOrDefault(x => x.reportid == ReportId && x.parmname.ToUpper().Trim() == "REPORTSTATUS"
                                                             && x.agency == Agency && x.usernumber == UserNumber && x.cfbox == ColdFusionBox);
            }
        }
    }
}
