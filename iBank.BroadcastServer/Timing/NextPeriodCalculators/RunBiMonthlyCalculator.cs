using Domain.Interfaces.BroadcastServer;
using Domain.Models.BroadcastServer;

namespace iBank.BroadcastServer.Timing.NextPeriodCalculators
{
    public class RunBiMonthlyCalculator : INextReportPeriodCalculator
    {
        private IRecordTimingDetails _timing;

        public RunBiMonthlyCalculator(IRecordTimingDetails timing)
        {
            _timing = timing;
        }

        public ReportPeriodDateRange CalculateNextReportPeriod()
        {
            var newRange = new ReportPeriodDateRange();

            newRange.ReportPeriodStart = _timing.NextReportPeriodStart.AddMonths(2);
            newRange.ReportPeriodEnd = newRange.ReportPeriodStart
                                               .AddMonths(2)
                                               .AddDays(-1);
            
            return newRange;
        }
    }
}