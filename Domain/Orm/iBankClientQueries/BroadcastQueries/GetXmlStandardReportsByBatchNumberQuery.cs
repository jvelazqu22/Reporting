using System.Collections.Generic;
using System.Linq;

using Domain.Helper;

using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientQueries.BroadcastQueries
{
    public class GetXmlStandardReportsByBatchNumberQuery : IQuery<IList<BroadcastReportInformation>>
    {
        public int? BatchNumber { get; set; }

        private readonly IClientQueryable _db;

        public GetXmlStandardReportsByBatchNumberQuery(IClientQueryable db, int? batchNumber)
        {
            _db = db;
            BatchNumber = batchNumber;
        }

        public IList<BroadcastReportInformation> ExecuteQuery()
        {
            using (_db)
            {
                return _db.iBBatch2.Where(s => s.udrkey < 0 && s.savedrptnum == 0 && s.batchnum == BatchNumber)
                .Select(s => new BroadcastReportInformation
                {
                    BatchNumber = s.batchnum,
                    BatchProgramNumber = s.batprgnum,
                    SavedReportNumber = 0,
                    UdrKey = s.udrkey ?? 0,
                    ProcessKey = s.processkey?? 0,
                    UserReportName = string.Empty,
                    CrystalReportType = "ixml",
                    DateType = s.datetype ?? 0,
                    Usage = "BOTH"
                }).ToList();
            }
        }
    }
}
