using Domain.Models.ReportPrograms.ServiceFeeSummaryByTransactionReport;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.SvcFeeSumTran
{
    public class SvcFeeSumTranDataProcessor
    {
        public IEnumerable<FinalData> MapRawToFinalData(IList<RawData> rawData)
        {
            return rawData.GroupBy(s => s.Descript, (k, g) => new FinalData
            {
                Descript = k,
                Svcfee = g.Sum(t => t.SvcAmt),
                Svcfeecnt = g.Sum(s => s.SvcAmt < 0 ? -1 : 1)
            });
        } 
    }
}