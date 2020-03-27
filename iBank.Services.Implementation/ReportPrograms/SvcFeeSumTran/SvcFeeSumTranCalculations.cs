using System.Collections.Generic;

namespace iBank.Services.Implementation.ReportPrograms.SvcFeeSumTran
{
    public class SvcFeeSumTranCalculations
    {
        public IList<string> GetExportFields()
        {
            return new List<string> { "descript", "svcfeecnt", "svcfee" };
        }

        public string GetCrystalReportName()
        {
            return "ibSvcFeeSumTran";
        }
    }
}