using System;
using iBank.Server.Utilities;

namespace iBank.BroadcastServer.Timing.NextRunCalculators
{
    public class BackOfficeTuesdayWeeklyNextRunCalculator : INextRunCalculator
    {
        private int _dayOfWeekToRun;
        private DateTime _nextReportPeriodEnd;
        private DateTime _today;

        public BackOfficeTuesdayWeeklyNextRunCalculator(DateTime nextReportPeriodEnd, int dayOfWeekToRun, DateTime today)
        {
            _nextReportPeriodEnd = nextReportPeriodEnd;
            _dayOfWeekToRun = dayOfWeekToRun;
            _today = today;
        }

        public DateTime CalculateNextRun()
        {
            var nextRun = _nextReportPeriodEnd.AddDays(_dayOfWeekToRun);

            if ((nextRun - _today).TotalDays > 8)
            {
                nextRun = _today.AddDays(7 + _dayOfWeekToRun - _today.DayOfWeekNumber());
            }

            return nextRun;
        }
    }
}