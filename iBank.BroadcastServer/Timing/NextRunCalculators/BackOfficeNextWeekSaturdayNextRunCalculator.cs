using System;

namespace iBank.BroadcastServer.Timing.NextRunCalculators
{
    public class BackOfficeNextWeekSaturdayNextRunCalculator : INextRunCalculator
    {
        private int _dayOfWeekToRun;
        private DateTime _nextReportPeriodStart;

        public BackOfficeNextWeekSaturdayNextRunCalculator(int dayOfWeekToRun, DateTime nextReportPeriodStart)
        {
            _dayOfWeekToRun = dayOfWeekToRun;
            _nextReportPeriodStart = nextReportPeriodStart;
        }

        public DateTime CalculateNextRun()
        {
            return ((_dayOfWeekToRun - 2) < 1)
                              ? _nextReportPeriodStart.AddDays(_dayOfWeekToRun - 7)
                              : _nextReportPeriodStart.AddDays(_dayOfWeekToRun - 7 - 7);
        }
    }
}