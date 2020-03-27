using Domain.Interfaces.BroadcastServer;
using System;

using Domain.Models.BroadcastServer;

namespace iBank.BroadcastServer.Timing.NextPeriodCalculators
{
    public class GeneralReportPeriodCalculator : INextReportPeriodCalculator
    {
        private IRecordTimingDetails _timing;

        public GeneralReportPeriodCalculator(IRecordTimingDetails timing)
        {
            this._timing = timing;
        }

        public ReportPeriodDateRange CalculateNextReportPeriod()
        {
            var reportPeriod = new ReportPeriodDateRange(_timing.NextReportPeriodStart, _timing.NextReportPeriodEnd);

            var nextPeriodStartMonth = _timing.NextReportPeriodStart.Month;
            var nextPeriodStartYear = _timing.NextReportPeriodStart.Year;
            nextPeriodStartMonth++;

            if (nextPeriodStartMonth > 12)
            {
                nextPeriodStartMonth = 1;
                nextPeriodStartYear++;
            }

            var nextPeriodStartDay = GetCorrectDayBasedOnMonth(_timing.DateOfStartingRangeForMonthlyData, nextPeriodStartMonth);

            reportPeriod.ReportPeriodStart = new DateTime(nextPeriodStartYear, nextPeriodStartMonth, nextPeriodStartDay);

            nextPeriodStartMonth++;
            if (nextPeriodStartMonth > 12)
            {
                nextPeriodStartMonth = 1;
                nextPeriodStartYear++;
            }

            var nextPeriodEndDay = GetCorrectDayBasedOnMonth(nextPeriodStartDay, nextPeriodStartMonth);
            reportPeriod.ReportPeriodEnd = new DateTime(nextPeriodStartYear, nextPeriodStartMonth, nextPeriodEndDay).AddDays(-1);

            return reportPeriod;
        }

        public int GetCorrectDayBasedOnMonth(int day, int month)
        {
            switch (month)
            {
                case 1:
                case 3:
                case 5:
                case 7:
                case 8:
                case 10:
                case 12:
                    return day > 31 ? 31 : day;
                case 2:
                    return day > 28 ? 28 : day;
                case 4:
                case 6:
                case 9:
                case 11:
                    return day > 30 ? 30 : day;
            }

            return day;
        }

    }
}