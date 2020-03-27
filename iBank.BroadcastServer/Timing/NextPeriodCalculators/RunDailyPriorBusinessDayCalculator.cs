using Domain.Interfaces.BroadcastServer;
using Domain.Models.BroadcastServer;

namespace iBank.BroadcastServer.Timing.NextPeriodCalculators
{
    public class RunDailyPriorBusinessDayCalculator : INextReportPeriodCalculator
    {
        private IRecordTimingDetails _timing;

        public RunDailyPriorBusinessDayCalculator(IRecordTimingDetails timing)
        {
            _timing = timing;
        }

        public ReportPeriodDateRange CalculateNextReportPeriod()
        {
            var returnVal = new ReportPeriodDateRange();
                //ReportPeriodStart = _timing.NextReportPeriodEnd,
                //ReportPeriodEnd = DateTime.Today
            returnVal.ReportPeriodEnd = _timing.Today;
            returnVal.ReportPeriodStart = returnVal.ReportPeriodEnd;

            return returnVal;
        }
    }
}