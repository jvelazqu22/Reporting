using System;

using iBank.Server.Utilities;

namespace iBank.Services.Implementation.ReportPrograms.UpcomingPlans
{
    public class UpcomingCalculations
    {
        public DateTime GetSunday(DateTime beginDate)
        {
            var sunday = beginDate;
            while (sunday.DayOfWeekNumber() > 1)
            {
                sunday = sunday.AddDays(-1);
            }

            return sunday;
        }

        public string GetCrystalReportName()
        {
            return "ibUpcoming";
        }

        public decimal GetWeekNum(DateTime? rdepdate, DateTime? sundate)
        {
            return ((rdepdate.GetValueOrDefault() - sundate.GetValueOrDefault()).Days / 7) + 1;
        }
    }
}
