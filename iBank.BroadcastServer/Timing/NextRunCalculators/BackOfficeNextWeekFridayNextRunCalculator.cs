using System;

namespace iBank.BroadcastServer.Timing.NextRunCalculators
{
    public class BackOfficeNextWeekFridayNextRunCalculator : INextRunCalculator
    {
        private int _dayOfWeekToRun;
        private DateTime _nextReportPeriodStart;

        public BackOfficeNextWeekFridayNextRunCalculator(int dayOfWeekToRun, DateTime nextReportPeriodStart)
        {
            _dayOfWeekToRun = dayOfWeekToRun;
            _nextReportPeriodStart = nextReportPeriodStart;
        }

        public DateTime CalculateNextRun()
        {
            return ((_dayOfWeekToRun - 2) < 1)
                              ? _nextReportPeriodStart.AddDays(_dayOfWeekToRun - 6)
                              : _nextReportPeriodStart.AddDays(_dayOfWeekToRun - 6 - 7);
        }
    }
}