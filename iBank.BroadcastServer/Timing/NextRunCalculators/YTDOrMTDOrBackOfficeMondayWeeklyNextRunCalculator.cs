using System;
using iBank.Server.Utilities;

namespace iBank.BroadcastServer.Timing.NextRunCalculators
{
    public class YTDOrMTDOrBackOfficeMondayWeeklyNextRunCalculator : INextRunCalculator
    {
        private int _dayOfWeekToRun;
        private DateTime _nextReportPeriodEnd;
        private DateTime _today;

        public YTDOrMTDOrBackOfficeMondayWeeklyNextRunCalculator(DateTime nextReportPeriodEnd, int dayOfWeekToRun, DateTime today)
        {
            _nextReportPeriodEnd = nextReportPeriodEnd;
            _dayOfWeekToRun = dayOfWeekToRun;
            _today = today;
        }

        public DateTime CalculateNextRun()
        {
            DateTime nextRun;
            if ((_dayOfWeekToRun - 1) < 1)
            {
                nextRun = _nextReportPeriodEnd.AddDays(_dayOfWeekToRun - 1 + 7);
            }
            else
            {
                nextRun = _nextReportPeriodEnd.AddDays(_dayOfWeekToRun - 1);
            }

            var dayDifference = _dayOfWeekToRun - nextRun.DayOfWeekNumber();
            nextRun = nextRun.AddDays(dayDifference);

            if (nextRun > _today.AddDays(8))
            {
                nextRun = _today.AddDays(7 + _dayOfWeekToRun - _today.DayOfWeekNumber());
            }

            return nextRun;
        }
    }
}