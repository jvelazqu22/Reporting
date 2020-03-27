using System;

namespace iBank.BroadcastServer.Timing.NextRunCalculators
{
    public class ReservationDailyNextRunCalculator : INextRunCalculator
    {
        private DateTime _nextReportPeriodStart;
        private bool _runNewData;
        private DateTime _today;

        public ReservationDailyNextRunCalculator(DateTime nextReportPeriodStart, bool runNewData, DateTime today)
        {
            _nextReportPeriodStart = nextReportPeriodStart;
            _runNewData = runNewData;
            _today = today;
        }

        public DateTime CalculateNextRun()
        {
            var nextRun = _nextReportPeriodStart;
            if (_runNewData && nextRun <= _today)
            {
                nextRun = nextRun.AddDays(1);
            }

            return nextRun;
        }
    }
}