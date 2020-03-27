using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetReportHandoffRecordsByParmNameQuery : BaseiBankMastersQuery<reporthandoff>
    {
        public string ParmName { get; set; }

        public GetReportHandoffRecordsByParmNameQuery(IMastersQueryable db, string parmName)
        {
            _db = db;
            ParmName = parmName;
        }

        public override reporthandoff ExecuteQuery()
        {
            using(_db)
            {
                return _db.ReportHandoff.FirstOrDefault(x => x.parmname.ToUpper().Trim() == ParmName);
            }
        }
    }
}
