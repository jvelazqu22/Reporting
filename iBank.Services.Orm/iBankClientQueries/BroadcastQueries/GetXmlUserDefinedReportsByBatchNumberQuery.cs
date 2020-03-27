using System.Collections.Generic;
using System.Linq;

using Domain.Helper;

using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankClientQueries.BroadcastQueries
{
    public class GetXmlUserDefinedReportsByBatchNumberQuery : BaseiBankClientQueryable<IList<BroadcastReportInformation>>
    {
        public int? BatchNumber { get; set; }

        public GetXmlUserDefinedReportsByBatchNumberQuery(IClientQueryable db, int? batchNumber)
        {
            _db = db;
            BatchNumber = batchNumber;
        }

        public override IList<BroadcastReportInformation> ExecuteQuery()
        {
            using (_db)
            {
                return _db.iBBatch2.Where(
                s => s.savedrptnum == 0 && s.processkey == 581 && s.batchnum == BatchNumber)
                .Join(_db.XmlUserRpt, ibbatch2 => new { Key = (int)ibbatch2.udrkey, UserNumber = (int)ibbatch2.UserNumber },
                    xmluserrpts => new { Key = xmluserrpts.reportkey, xmluserrpts.UserNumber },
                    (ibbatch2, xmluserrpt) => new BroadcastReportInformation
                    {
                        BatchNumber = ibbatch2.batchnum,
                        BatchProgramNumber = ibbatch2.batprgnum,
                        SavedReportNumber = 0,
                        UdrKey = ibbatch2.udrkey ?? 0,
                        ProcessKey = ibbatch2.processkey ?? 0,
                        UserReportName = xmluserrpt.crname.Trim(),
                        CrystalReportType = xmluserrpt.crtype.Trim(),
                        DateType = ibbatch2.datetype ?? 0,
                        Usage = "BOTH"
                    }).ToList();
            }
        }
    }
}
