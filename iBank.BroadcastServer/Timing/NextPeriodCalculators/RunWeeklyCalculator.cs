using Domain.Interfaces.BroadcastServer;
using Domain.Models.BroadcastServer;

namespace iBank.BroadcastServer.Timing.NextPeriodCalculators
{
    public class RunWeeklyCalculator : INextReportPeriodCalculator
    {
        private IRecordTimingDetails _timing;

        public RunWeeklyCalculator(IRecordTimingDetails timing)
        {
            _timing = timing;
        }

        public ReportPeriodDateRange CalculateNextReportPeriod()
        {
            return new ReportPeriodDateRange
            {
                ReportPeriodStart = _timing.NextReportPeriodStart.AddDays(7),
                ReportPeriodEnd = _timing.NextReportPeriodEnd.AddDays(7)
            };
        }
    }
}