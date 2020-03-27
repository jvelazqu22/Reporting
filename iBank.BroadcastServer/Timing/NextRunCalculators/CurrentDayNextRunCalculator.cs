using System;

namespace iBank.BroadcastServer.Timing.NextRunCalculators
{
    public class CurrentDayNextRunCalculator : INextRunCalculator
    {
        private DateTime _nextReportPeriodEnd;

        public CurrentDayNextRunCalculator(DateTime nextReportPeriodEnd)
        {
            _nextReportPeriodEnd = nextReportPeriodEnd;
        }

        public DateTime CalculateNextRun()
        {
            return _nextReportPeriodEnd;
        }
    }
}