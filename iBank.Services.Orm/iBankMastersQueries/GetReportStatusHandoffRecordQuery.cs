using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetReportStatusHandoffRecordQuery : BaseiBankMastersQuery<reporthandoff>
    {
        public string ReportId { get; set; }
        public string Agency { get; set; }
        public int UserNumber { get; set; }
        public string ColdFusionBox { get; set; }

        public GetReportStatusHandoffRecordQuery(IMastersQueryable db, string reportId, string agency, int userNumber, 
            string coldFusionBox)
        {
            _db = db;
            ReportId = reportId;
            Agency = agency;
            UserNumber = userNumber;
            ColdFusionBox = coldFusionBox;
        }

        public override reporthandoff ExecuteQuery()
        {
            using(_db)
            {
                return _db.ReportHandoff.FirstOrDefault(x => x.reportid == ReportId && x.parmname.ToUpper().Trim() == "REPORTSTATUS"
                                                             && x.agency == Agency && x.usernumber == UserNumber && x.cfbox == ColdFusionBox);
            }
        }
    }
}
