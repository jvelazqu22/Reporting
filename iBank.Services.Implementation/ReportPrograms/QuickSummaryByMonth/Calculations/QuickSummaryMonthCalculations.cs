using System.Collections.Generic;
using System.Linq;

using Domain.Constants;

namespace iBank.Services.Implementation.ReportPrograms.QuickSummaryByMonth.Calculations
{
    public class QuickSummaryMonthCalculations
    {
        public string GetCrystalReportName(bool excludeExceptions, bool isGraphReportOutput)
        {
            if (isGraphReportOutput)
            {
                return ReportNames.GRAPH_2;
            }

            return excludeExceptions
                       ? ReportNames.QUICK_SUMMARY_BY_MONTH_RPT_1A
                       : ReportNames.QUICK_SUMMARY_BY_MONTH_RPT_1;
        }

        public bool IsGraphReportOutput(string outputType)
        {
            var graphOutputTypes = new List<string> { "4", "6", "8", "RG", "XG" };

            return graphOutputTypes.Any(type => type == outputType);
        }
    }
}
