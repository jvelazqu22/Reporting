using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetReportByProcessIdQuery : BaseiBankMastersQuery<ibproces>
    {
        public int ProcessId { get; set; }

        public GetReportByProcessIdQuery(IMastersQueryable db, int processId)
        {
            _db = db;
            ProcessId = processId;
        }

        public override ibproces ExecuteQuery()
        {
            using (_db)
            { 
                var processRecord = _db.iBProcess.FirstOrDefault(x => x.processkey == ProcessId);

                return processRecord;
            }
        }
    }
}
