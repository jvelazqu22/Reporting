using System.Collections.Generic;
using System.Linq;

using Domain.Helper;

using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankClientQueries.BroadcastQueries
{
    public class GetSavedReportsByBatchNumberQuery : BaseiBankClientQueryable<IList<BroadcastReportInformation>>
    {
        public int? BatchNumber { get; set; }

        public GetSavedReportsByBatchNumberQuery(IClientQueryable db, int? batchNumber)
        {
            _db = db;
            BatchNumber = batchNumber;
        }

        public override IList<BroadcastReportInformation> ExecuteQuery()
        {
            using (_db)
            {
                return _db.iBBatch2.Where(s => s.batchnum == BatchNumber)
                .Join(_db.SavedRpt1, ibbatch2 => ibbatch2.savedrptnum, savedrpt1 => savedrpt1.recordnum,
                    (ibbatch2, savedrpt1) => new BroadcastReportInformation
                    {
                        BatchNumber = ibbatch2.batchnum,
                        BatchProgramNumber = ibbatch2.batprgnum,
                        SavedReportNumber = ibbatch2.savedrptnum ?? 0,
                        UdrKey = ibbatch2.udrkey ?? 0,
                        ProcessKey = savedrpt1.processkey ?? 0,
                        UserReportName = savedrpt1.userrptnam,
                        DateType = 0
                    }).ToList();
            }
        }
    }
}
