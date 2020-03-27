using System.Linq;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetReportByProcessIdQuery : IQuery<ibproces>
    {
        public int ProcessId { get; set; }
        private readonly IMastersQueryable _db;

        public GetReportByProcessIdQuery(IMastersQueryable db, int processId)
        {
            _db = db;
            ProcessId = processId;
        }

        public ibproces ExecuteQuery()
        {
            using (_db)
            { 
                var processRecord = _db.iBProcess.FirstOrDefault(x => x.processkey == ProcessId);

                return processRecord;
            }
        }
    }
}
