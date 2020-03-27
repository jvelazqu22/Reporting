using System;

namespace iBank.BroadcastServer.Timing.NextRunCalculators
{
    public class ReservationDailyFrequencyNextRunCalculator : INextRunCalculator
    {
        private DateTime _nextReportPeriodStart;

        public ReservationDailyFrequencyNextRunCalculator(DateTime nextReportPeriodStart)
        {
            _nextReportPeriodStart = nextReportPeriodStart;
        }

        public DateTime CalculateNextRun()
        {
            return _nextReportPeriodStart.AddDays(-1);
        }
    }
}