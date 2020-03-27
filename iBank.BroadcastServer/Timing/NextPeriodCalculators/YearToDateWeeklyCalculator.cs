using Domain.Interfaces.BroadcastServer;
using System;

using Domain.Models.BroadcastServer;

namespace iBank.BroadcastServer.Timing.NextPeriodCalculators
{
    internal class YearToDateWeeklyCalculator : INextReportPeriodCalculator
    {
        private IRecordTimingDetails _timing;

        public YearToDateWeeklyCalculator(IRecordTimingDetails timing)
        {
            _timing = timing;
        }

        public ReportPeriodDateRange CalculateNextReportPeriod()
        {
            var reportPeriod = new ReportPeriodDateRange(_timing.NextReportPeriodStart, _timing.NextReportPeriodEnd);

            var oldYear = _timing.NextReportPeriodEnd.Year;

            reportPeriod.ReportPeriodEnd = reportPeriod.ReportPeriodEnd.AddDays(7);

            var newYear = reportPeriod.ReportPeriodEnd.Year;

            if (newYear > oldYear)
            {
                reportPeriod.ReportPeriodStart = new DateTime(newYear, 1, 1);
            }

            return reportPeriod;
        }
    }
}
