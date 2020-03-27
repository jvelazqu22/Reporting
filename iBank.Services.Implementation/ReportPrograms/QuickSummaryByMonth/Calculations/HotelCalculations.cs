using System.Collections.Generic;

namespace iBank.Services.Implementation.ReportPrograms.QuickSummaryByMonth.Calculations
{
    public class HotelCalculations
    {
        public int CalculateHotelExcepts(string reasCodh, IList<string> reasExclude, int hplusmin)
        {
            reasCodh = reasCodh.Trim();
            if (string.IsNullOrEmpty(reasCodh))
            {
                return 000000;
            }

            if (reasExclude.Contains(reasCodh))
            {
                return 000000;
            }

            return hplusmin;
        }

        public decimal CalculateHotelLost(string reasCodh, IList<string> reasExclude, decimal bookRate, decimal hexcprat, int nights, int rooms)
        {
            reasCodh = reasCodh.Trim();
            if (string.IsNullOrEmpty(reasCodh))
            {
                return 00000000.00M;
            }

            if (reasExclude.Contains(reasCodh))
            {
                return 000000;
            }

            return (bookRate - hexcprat) * nights * rooms;
        }
    }
}
