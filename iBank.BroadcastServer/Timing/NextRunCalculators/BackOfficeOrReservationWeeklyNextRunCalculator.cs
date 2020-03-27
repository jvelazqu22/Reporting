using System;
using Domain.Constants;
using iBank.Server.Utilities;

namespace iBank.BroadcastServer.Timing.NextRunCalculators
{
    public class BackOfficeOrReservationWeeklyNextRunCalculator : INextRunCalculator
    {
        private DateTime _nextReportPeriodStart;

        private int _broadcastSchedule;

        private int _dayOfWeekToRun;

        private DateTime _nextReportPeriodEnd;

        public BackOfficeOrReservationWeeklyNextRunCalculator(DateTime nextReportPeriodStart, DateTime nextReportPeriodEnd,
            int broadcastSchedule, int dayOfWeekToRun)
        {
            _nextReportPeriodStart = nextReportPeriodStart;
            _nextReportPeriodEnd = nextReportPeriodEnd;
            _broadcastSchedule = broadcastSchedule;
            _dayOfWeekToRun = dayOfWeekToRun;
        }

        public DateTime CalculateNextRun()
        {
            var dayOfWeekOfNextReportPeriodStart = _nextReportPeriodStart.DayOfWeekNumber();
            if (_broadcastSchedule == BroadcastSchedule.RESERVATION)
            {
                return dayOfWeekOfNextReportPeriodStart < _dayOfWeekToRun 
                    ? _nextReportPeriodStart.AddDays(_dayOfWeekToRun - dayOfWeekOfNextReportPeriodStart - 7)
                    : _nextReportPeriodStart.AddDays(_dayOfWeekToRun - dayOfWeekOfNextReportPeriodStart);
            }
            else
            {
                var nextRun = dayOfWeekOfNextReportPeriodStart < _dayOfWeekToRun 
                                       ? _nextReportPeriodStart.AddDays(_dayOfWeekToRun - dayOfWeekOfNextReportPeriodStart) 
                                       : _nextReportPeriodStart.AddDays(_dayOfWeekToRun - dayOfWeekOfNextReportPeriodStart + 7);

                if (nextRun <= _nextReportPeriodEnd) nextRun = nextRun.AddDays(7);

                return nextRun;
            }
        }
    }
}
