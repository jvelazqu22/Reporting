using System;

namespace iBank.BroadcastServer.Timing.NextRunCalculators
{
    public class MultipleDayOfWeekNextRunCalculator : INextRunCalculator
    {
        private DateTime _nextReportPeriodStart;

        public MultipleDayOfWeekNextRunCalculator(DateTime nextReportPeriodStart)
        {
            _nextReportPeriodStart = nextReportPeriodStart;
        }

        public DateTime CalculateNextRun()
        {
            return _nextReportPeriodStart;
        }
    }
}