using System.Collections.Generic;
using System.Linq;

using Domain.Helper;

using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankClientQueries.BroadcastQueries
{
    public class GetUserDefinedReportsByBatchNumberQuery : BaseiBankClientQueryable<IList<BroadcastReportInformation>>
    {
        public int? BatchNumber { get; set; }

        public GetUserDefinedReportsByBatchNumberQuery(IClientQueryable db, int? batchNumber)
        {
            _db = db;
            BatchNumber = batchNumber;
        }
        
        public override IList<BroadcastReportInformation> ExecuteQuery()
        {
            using (_db)
            {
                return _db.iBBatch2.Where(s => s.savedrptnum == 0 && s.batchnum == BatchNumber && s.processkey != 581)
                .Join(_db.UserRpt, ibbatch2 => new { Key = (int)ibbatch2.udrkey, UserNumber = (int)ibbatch2.UserNumber },
                    userrpt => new { Key = userrpt.reportkey, userrpt.UserNumber },
                    (ibbatch2, userrpt) => new BroadcastReportInformation
                    {
                        BatchNumber = ibbatch2.batchnum,
                        BatchProgramNumber = ibbatch2.batprgnum,
                        SavedReportNumber = 0,
                        UdrKey = ibbatch2.udrkey ?? 0,
                        ProcessKey = 0,
                        UserReportName = userrpt.crname.Trim(),
                        CrystalReportType = userrpt.crtype.Trim(),
                        DateType = ibbatch2.datetype ?? 0,
                        Usage = "BOTH"
                    }).ToList();
            }
        }
    }
}
