using System.Collections.Generic;
using System.Linq;

using Domain.Helper;

using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankClientQueries.BroadcastQueries
{
    public class GetStandardReportsByBatchNumberQuery : BaseiBankClientQueryable<IList<BroadcastReportInformation>>
    {
        public int? BatchNumber { get; set; }
        public IEnumerable<ProcessCaptionInformation> ProcessCaptions { get; set; }

        public GetStandardReportsByBatchNumberQuery(IClientQueryable db, int? batchNumber, IEnumerable<ProcessCaptionInformation> processCaptions)
        {
            _db = db;
            BatchNumber = batchNumber;
            ProcessCaptions = processCaptions;
        }

        public override IList<BroadcastReportInformation> ExecuteQuery()
        {
            using (_db)
            {
                return _db.iBBatch2.Where(s => s.processkey > 0 && s.savedrptnum == 0
                && (s.processkey < 500 || s.processkey > 599) && s.batchnum == BatchNumber).ToList()
                .Join(ProcessCaptions.Select(s => new { s.ProcessKey, s.Caption, s.Usage }), b => b.processkey, c => c.ProcessKey, (b, c) => new BroadcastReportInformation
                {
                    BatchNumber = b.batchnum,
                    BatchProgramNumber = b.batprgnum,
                    SavedReportNumber = b.savedrptnum ?? 0,
                    UdrKey = b.udrkey ?? 0,
                    ProcessKey = b.processkey ?? 0,
                    UserReportName = c.Caption.Trim(),
                    DateType = b.datetype ?? 0,
                    Usage = c.Usage
                }).ToList();
            }
        }
    }
}
