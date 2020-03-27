using System;
using iBank.Server.Utilities;

namespace iBank.BroadcastServer.Timing.NextRunCalculators
{
    public class ReservationWeeklyNextRunCalculator : INextRunCalculator
    {
        private int _dayOfWeekToRun;
        private DateTime _today;

        public ReservationWeeklyNextRunCalculator(int dayOfWeekToRun, DateTime today)
        {
            _dayOfWeekToRun = dayOfWeekToRun;
            _today = today;
        }

        public DateTime CalculateNextRun()
        {
            var todayDayOfWeekNumber = _today.DayOfWeekNumber();

            return _dayOfWeekToRun <= todayDayOfWeekNumber
                ? _today.AddDays(7 + _dayOfWeekToRun - todayDayOfWeekNumber)
                : _today.AddDays(_dayOfWeekToRun - todayDayOfWeekNumber);
        }
    }
}