using Domain.Interfaces.BroadcastServer;
using Domain.Models.BroadcastServer;

namespace iBank.BroadcastServer.Timing.NextPeriodCalculators
{
    public class ReservationDailyCalculator : INextReportPeriodCalculator
    {
        private IRecordTimingDetails _timing;

        public ReservationDailyCalculator(IRecordTimingDetails timing)
        {
            _timing = timing;
        }

        public ReportPeriodDateRange CalculateNextReportPeriod()
        {
            var range = new ReportPeriodDateRange();
            range.ReportPeriodStart = _timing.NextReportPeriodStart.AddDays(1);
            range.ReportPeriodEnd = range.ReportPeriodStart.AddDays(_timing.ReportDays);

            return range;
        }
    }
}