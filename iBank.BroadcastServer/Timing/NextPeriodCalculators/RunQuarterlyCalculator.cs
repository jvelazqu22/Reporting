using Domain.Interfaces.BroadcastServer;
using Domain.Models.BroadcastServer;

namespace iBank.BroadcastServer.Timing.NextPeriodCalculators
{
    public class RunQuarterlyCalculator : INextReportPeriodCalculator
    {
        private IRecordTimingDetails _timing;

        public RunQuarterlyCalculator(IRecordTimingDetails timing)
        {
            _timing = timing;
        }

        public ReportPeriodDateRange CalculateNextReportPeriod()
        {
            var retValue = new ReportPeriodDateRange();

            retValue.ReportPeriodStart = _timing.NextReportPeriodStart.AddMonths(3);
            retValue.ReportPeriodEnd = retValue.ReportPeriodStart.AddMonths(3).AddDays(-1);

            return retValue;
        }
    }
}