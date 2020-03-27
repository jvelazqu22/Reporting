using Domain.Interfaces.BroadcastServer;
using Domain.Models.BroadcastServer;

namespace iBank.BroadcastServer.Timing.NextPeriodCalculators
{
    public class RunAnnualCalculator : INextReportPeriodCalculator
    {
        private IRecordTimingDetails _timing;

        public RunAnnualCalculator(IRecordTimingDetails timing)
        {
            _timing = timing;
        }

        public ReportPeriodDateRange CalculateNextReportPeriod()
        {
            var retValue = new ReportPeriodDateRange();

            retValue.ReportPeriodStart = _timing.NextReportPeriodStart.AddMonths(12);
            retValue.ReportPeriodEnd = retValue.ReportPeriodStart.AddMonths(12).AddDays(-1);

            return retValue;
        }
    }
}