using System;

namespace iBank.BroadcastServer.Timing.NextRunCalculators
{
    public class DailyBusinessDayNextRunCalculator : INextRunCalculator
    {
        private DateTime _nextRun;
        private DateTime _today;

        public DailyBusinessDayNextRunCalculator(DateTime today, DateTime nextRun)
        {
            _today = today;
            _nextRun = nextRun;
        }

        public DateTime CalculateNextRun()
        {
            var nextDay = _today.AddDays(1);
            var hour = _nextRun.Hour;
            var minute = _nextRun.Minute;
            var second = _nextRun.Second;

            return hour > 5 
                ? new DateTime(nextDay.Year, nextDay.Month, nextDay.Day, hour, minute, second) 
                : new DateTime(nextDay.Year, nextDay.Month, nextDay.Day, 5, 0, 0);
        }
    }
}