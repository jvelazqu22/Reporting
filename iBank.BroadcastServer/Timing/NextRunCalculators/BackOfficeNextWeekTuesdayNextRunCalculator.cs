using System;

namespace iBank.BroadcastServer.Timing.NextRunCalculators
{
    public class BackOfficeNextWeekTuesdayNextRunCalculator : INextRunCalculator
    {
        private int _dayOfWeekToRun;
        private DateTime _nextReportPeriodStart;

        public BackOfficeNextWeekTuesdayNextRunCalculator(int dayOfWeekToRun, DateTime nextReportPeriodStart)
        {
            _dayOfWeekToRun = dayOfWeekToRun;
            _nextReportPeriodStart = nextReportPeriodStart;
        }

        public DateTime CalculateNextRun()
        {
            return ((_dayOfWeekToRun - 2) < 1)
                              ? _nextReportPeriodStart.AddDays(_dayOfWeekToRun - 3)
                              : _nextReportPeriodStart.AddDays(_dayOfWeekToRun - 3 - 7);
        }
    }
}