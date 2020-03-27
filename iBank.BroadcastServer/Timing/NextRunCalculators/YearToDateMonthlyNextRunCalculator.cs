using System;

namespace iBank.BroadcastServer.Timing.NextRunCalculators
{
    public class YearToDateMonthlyNextRunCalculator : INextRunCalculator
    {
        private int _dayOfMonthToRunMonthlyReport;
        private DateTime _nextReportPeriodEnd;

        public YearToDateMonthlyNextRunCalculator(DateTime nextReportPeriodEnd, int dayOfMonthToRunMonthlyReport)
        {
            _nextReportPeriodEnd = nextReportPeriodEnd;
            _dayOfMonthToRunMonthlyReport = dayOfMonthToRunMonthlyReport;
        }

        public DateTime CalculateNextRun()
        {
            var timingCalculator = new TimingCalculator();

            var temp = _nextReportPeriodEnd.AddDays(1);
            return timingCalculator.GetValidDate(temp.Year, temp.Month, _dayOfMonthToRunMonthlyReport);
        }
    }
}