using Domain.Interfaces.BroadcastServer;
using System;

using Domain.Models.BroadcastServer;

namespace iBank.BroadcastServer.Timing.NextPeriodCalculators
{
    public class VariableTimePeriodCalculator : INextReportPeriodCalculator
    {
        private IRecordTimingDetails _timing;

        public VariableTimePeriodCalculator(IRecordTimingDetails timing)
        {
            _timing = timing;
        }

        public ReportPeriodDateRange CalculateNextReportPeriod()
        {
            var reportPeriod = new ReportPeriodDateRange(_timing.NextReportPeriodStart, _timing.NextReportPeriodEnd);

            var seconds = _timing.Conditionals.IsDailyEveryXHoursSchedule
                ? 60 * 60 * Math.Abs(_timing.FrequencyOfRun)
                : 60 * Math.Abs(_timing.FrequencyOfRun);

            reportPeriod.ReportPeriodStart = _timing.NextReportPeriodStart.AddSeconds(seconds);
            reportPeriod.ReportPeriodEnd = _timing.NextReportPeriodEnd.AddSeconds(seconds);

            return reportPeriod;
        }
    }
}