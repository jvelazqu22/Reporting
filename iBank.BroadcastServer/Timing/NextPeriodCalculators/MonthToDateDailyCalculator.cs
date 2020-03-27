using Domain.Interfaces.BroadcastServer;
using System;

using Domain.Models.BroadcastServer;

namespace iBank.BroadcastServer.Timing.NextPeriodCalculators
{
    public class MonthToDateDailyCalculator : INextReportPeriodCalculator
    {
        private IRecordTimingDetails _timing;

        public MonthToDateDailyCalculator(IRecordTimingDetails timing)
        {
            _timing = timing;
        }

        public ReportPeriodDateRange CalculateNextReportPeriod()
        {
            var reportPeriod = new ReportPeriodDateRange(_timing.NextReportPeriodStart, _timing.NextReportPeriodEnd);

            var oldYear = _timing.NextReportPeriodEnd.Year;
            var oldMonth = _timing.NextReportPeriodEnd.Month;

            reportPeriod.ReportPeriodEnd = reportPeriod.ReportPeriodEnd.AddDays(1);
            var newYear = reportPeriod.ReportPeriodEnd.Year;
            var newMonth = reportPeriod.ReportPeriodEnd.Month;
            if (newYear > oldYear || (newYear == oldYear && newMonth > oldMonth))
            {
                reportPeriod.ReportPeriodStart = new DateTime(newYear, newMonth, 1);
            }

            return reportPeriod;
        }
    }
}