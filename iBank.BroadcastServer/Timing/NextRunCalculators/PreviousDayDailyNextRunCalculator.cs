using System;

namespace iBank.BroadcastServer.Timing.NextRunCalculators
{
    public class PreviousDayDailyNextRunCalculator : INextRunCalculator
    {
        private DateTime _nextReportPeriodStart;
        private bool _runNewData;
        private DateTime _today;

        public PreviousDayDailyNextRunCalculator(DateTime nextReportPeriodStart, bool runNewData, DateTime today)
        {
            _nextReportPeriodStart = nextReportPeriodStart;
            _runNewData = runNewData;
            _today = today;
        }

        public DateTime CalculateNextRun()
        {
            var nextRun = _nextReportPeriodStart.AddDays(1);

            if (_runNewData && nextRun <= _today)
            {
                nextRun = nextRun.AddDays(1);
            }

            return nextRun;
        }
    }
}