using Domain.Helper;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Orm.iBankClientQueries.BroadcastQueries
{
    public class GetTravleOptixReportsByBatchNumberQuery : IQuery<IList<BroadcastReportInformation>>
    {
        public int? BatchNumber { get; set; }

        private readonly IClientQueryable _db;

        public GetTravleOptixReportsByBatchNumberQuery(IClientQueryable db, int? batchNumber)
        {
            _db = db;
            BatchNumber = batchNumber;
        }

        public IList<BroadcastReportInformation> ExecuteQuery()
        {
            using (_db)
            {
                return _db.iBBatch2.Where(s => s.udrkey > 0
                && s.savedrptnum == 0
                && s.batchnum == BatchNumber
                && s.processkey == (int)ReportTitles.TravelOptixReport)
                .Join(_db.TravelOptixReports, ibbatch2 => new {Key = (int)ibbatch2.udrkey, UserNumber = (int)ibbatch2.UserNumber},
                travelOptixReport => new {Key = travelOptixReport.ReportKey, travelOptixReport.UserNumber },
                (ibbtach2, travelOptixReport) => new BroadcastReportInformation
                {
                    BatchNumber = ibbtach2.batchnum,
                    BatchProgramNumber = ibbtach2.batprgnum,
                    SavedReportNumber = 0,
                    UdrKey = ibbtach2.udrkey ?? 0,
                    ProcessKey = ibbtach2.processkey ?? 0,
                    IsDotNetEnabled = true,
                    UserReportName = travelOptixReport.ReportName,
                    CrystalReportType = "TravelOptix",
                    DateType = ibbtach2.datetype ?? 0,
                    Usage = string.Empty
                }).ToList();
            }
        }
    }
}