using System;
using iBank.Server.Utilities;

namespace iBank.BroadcastServer.Timing.NextRunCalculators
{
    public class ReservationPreviousWeekWeeklyNextRunCalculator : INextRunCalculator
    {
        private int _dayOfWeekToRun;
        private DateTime _nextReportPeriodEnd;
        private bool _runNewData;
        private DateTime _today;

        public ReservationPreviousWeekWeeklyNextRunCalculator(DateTime nextReportPeriodEnd, int dayOfWeekToRun, bool runNewData, 
            DateTime today)
        {
            _nextReportPeriodEnd = nextReportPeriodEnd;
            _dayOfWeekToRun = dayOfWeekToRun;
            _runNewData = runNewData;
            _today = today;
        }

        public DateTime CalculateNextRun()
        {
            DateTime nextRun;
            if (_nextReportPeriodEnd.DayOfWeekNumber() < _dayOfWeekToRun)
            {
                nextRun = _nextReportPeriodEnd
                    .AddDays(_dayOfWeekToRun - _nextReportPeriodEnd.DayOfWeekNumber());
            }
            else
            {
                nextRun = _nextReportPeriodEnd
                    .AddDays(_dayOfWeekToRun - _nextReportPeriodEnd.DayOfWeekNumber() + 7);
            }

            if (_runNewData && nextRun <= _today)
            {
                nextRun = nextRun.AddDays(7);
            }

            return nextRun;
        }
    }
}