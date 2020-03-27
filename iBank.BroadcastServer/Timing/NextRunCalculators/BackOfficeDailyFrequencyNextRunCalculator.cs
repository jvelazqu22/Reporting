using System;

namespace iBank.BroadcastServer.Timing.NextRunCalculators
{
    public class BackOfficeDailyFrequencyNextRunCalculator : INextRunCalculator
    {
        private DateTime _nextReportPeriodStart;

        public BackOfficeDailyFrequencyNextRunCalculator(DateTime nextReportPeriodStart)
        {
            _nextReportPeriodStart = nextReportPeriodStart;
        }

        public DateTime CalculateNextRun()
        {
            return _nextReportPeriodStart.AddDays(1);
        }
    }
}