using Domain.Interfaces.BroadcastServer;
using Domain.Models.BroadcastServer;

namespace iBank.BroadcastServer.Timing.NextPeriodCalculators
{
    public class RunSemiAnnualCalculator : INextReportPeriodCalculator
    {
        private IRecordTimingDetails _timing;

        public RunSemiAnnualCalculator(IRecordTimingDetails timing)
        {
            _timing = timing;
        }

        public ReportPeriodDateRange CalculateNextReportPeriod()
        {
            var returnVal = new ReportPeriodDateRange();

            returnVal.ReportPeriodStart = _timing.NextReportPeriodStart.AddMonths(6);
            returnVal.ReportPeriodEnd = returnVal.ReportPeriodStart.AddMonths(6).AddDays(-1);

            return returnVal;
        }
    }
}