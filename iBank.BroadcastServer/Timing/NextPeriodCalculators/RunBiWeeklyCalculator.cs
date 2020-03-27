using Domain.Interfaces.BroadcastServer;
using Domain.Models.BroadcastServer;

namespace iBank.BroadcastServer.Timing.NextPeriodCalculators
{
    public class RunBiWeeklyCalculator : INextReportPeriodCalculator
    {
        private IRecordTimingDetails _timing;

        public RunBiWeeklyCalculator(IRecordTimingDetails timing)
        {
            _timing = timing;
        }

        public ReportPeriodDateRange CalculateNextReportPeriod()
        {
            return new ReportPeriodDateRange
                       {
                           ReportPeriodStart = _timing.NextReportPeriodStart.AddDays(14),
                           ReportPeriodEnd = _timing.NextReportPeriodEnd.AddDays(14)
                       };

        }
    }
}