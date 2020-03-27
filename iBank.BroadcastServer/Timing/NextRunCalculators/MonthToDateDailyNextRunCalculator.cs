using System;

namespace iBank.BroadcastServer.Timing.NextRunCalculators
{
    public class MonthToDateDailyNextRunCalculator : INextRunCalculator
    {
        private DateTime _nextReportPeriodEnd;

        public MonthToDateDailyNextRunCalculator(DateTime nextReportPeriodEnd)
        {
            _nextReportPeriodEnd = nextReportPeriodEnd;
        }

        public DateTime CalculateNextRun()
        {
            return _nextReportPeriodEnd.AddDays(1);
        }
    }
}