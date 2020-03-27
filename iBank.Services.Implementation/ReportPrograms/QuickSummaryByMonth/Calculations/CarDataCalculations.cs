using System.Collections.Generic;

namespace iBank.Services.Implementation.ReportPrograms.QuickSummaryByMonth.Calculations
{
    public class CarDataCalculations
    {
        public int CalculateCarExcepts(string reasCoda, IList<string> reasExclude, int cplusMin)
        {
            reasCoda = reasCoda.Trim();
            if (string.IsNullOrEmpty(reasCoda))
            {
                return 000000;
            }

            if (reasExclude.Contains(reasCoda))
            {
                return 000000;
            }

            return cplusMin;
        }

        public decimal CalculateCarLost(string reasCode, IList<string> reasExclude, decimal abookRat, decimal aExcprat, int days)
        {
            reasCode = reasCode.Trim();
            if (string.IsNullOrEmpty(reasCode))
            {
                return 00000000.00M;
            }

            if (reasExclude.Contains(reasCode))
            {
                return 000000;
            }

            return (abookRat - aExcprat) * days;
        }
    }
}
