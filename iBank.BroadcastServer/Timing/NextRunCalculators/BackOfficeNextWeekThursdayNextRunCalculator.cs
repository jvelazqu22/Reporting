using System;

namespace iBank.BroadcastServer.Timing.NextRunCalculators
{
    public class BackOfficeNextWeekThursdayNextRunCalculator : INextRunCalculator
    {
        private int _dayOfWeekToRun;
        private DateTime _nextReportPeriodStart;

        public BackOfficeNextWeekThursdayNextRunCalculator(int dayOfWeekToRun, DateTime nextReportPeriodStart)
        {
            _dayOfWeekToRun = dayOfWeekToRun;
            _nextReportPeriodStart = nextReportPeriodStart;
        }

        public DateTime CalculateNextRun()
        {
            return ((_dayOfWeekToRun - 2) < 1)
                              ? _nextReportPeriodStart.AddDays(_dayOfWeekToRun - 5)
                              : _nextReportPeriodStart.AddDays(_dayOfWeekToRun - 5 - 7);
        }
    }
}