using System.Collections.Generic;
using System.Linq;
using Domain.Helper;
using Domain.Models.ReportPrograms.ExecutiveSummarywithGraphsReport;

namespace iBank.Services.Implementation.ReportPrograms.ExecutiveSummarywithGraphs
{
    public class DataRetrieval
    {
        public static decimal GetServiceFeeTotal(List<FeeRawData> feeRawData, List<RawData> rawData, BackofficeOrReservation reportDesignation, bool orphanServiceFees)
        {
            if (!feeRawData.Any()) return 0m;
            if (reportDesignation == BackofficeOrReservation.Reservation && !orphanServiceFees)
            {
                return (from f in feeRawData
                        join r in rawData.Where(x => !string.IsNullOrEmpty(x.RecLoc) && !x.RecLoc.Equals(" "))
                        on new { f.RecLoc, f.Invoice, f.Acct, f.PassLast } equals new { r.RecLoc, r.Invoice, r.Acct, r.PassLast }
                        select new FeeRawData { SvcAmt = f.SvcAmt }).ToList()
                                                                    .Sum(s => s.SvcAmt);
            }
            else
            {
                return feeRawData.ToList().Sum(s => s.SvcAmt);
            }
        }
    }
}
