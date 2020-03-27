using Domain.Interfaces.BroadcastServer;
using Domain.Models.BroadcastServer;

namespace iBank.BroadcastServer.Timing.NextPeriodCalculators
{
    public class RunDailyNextBusinessDayCalculator : INextReportPeriodCalculator
    {
        private IRecordTimingDetails _timing;

        public RunDailyNextBusinessDayCalculator(IRecordTimingDetails timing)
        {
            _timing = timing;
        }

        public ReportPeriodDateRange CalculateNextReportPeriod()
        {
            var returnValue = new ReportPeriodDateRange();

            returnValue.ReportPeriodEnd = _timing.Today.AddDays(2);
            returnValue.ReportPeriodStart = returnValue.ReportPeriodEnd;

            return returnValue;
        }
    }
}