using System.Collections.Generic;
using System.Linq;

using Domain.Helper;

using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientQueries.BroadcastQueries
{
    public class GetXmlUserDefinedReportsByBatchNumberQuery : IQuery<IList<BroadcastReportInformation>>
    {
        public int? BatchNumber { get; set; }

        private readonly IClientQueryable _db;

        public GetXmlUserDefinedReportsByBatchNumberQuery(IClientQueryable db, int? batchNumber)
        {
            _db = db;
            BatchNumber = batchNumber;
        }

        public IList<BroadcastReportInformation> ExecuteQuery()
        {
            using (_db)
            {
                if (Features.TravelOptixImplimentation.IsEnabled())
                {
                    return _db.iBBatch2.Where(
                    s => s.savedrptnum == 0 
                    && s.processkey == (int)ReportTitles.iXMLUserDefinedExport 
                    && s.batchnum == BatchNumber)
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
                else
                {
                    return _db.iBBatch2.Where(
                        s => s.savedrptnum == 0 
                        && s.processkey == 581 
                        && s.batchnum == BatchNumber)
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
}
