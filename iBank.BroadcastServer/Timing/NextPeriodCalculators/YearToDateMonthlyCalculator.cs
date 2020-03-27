using Domain.Interfaces.BroadcastServer;
using System;

using Domain.Models.BroadcastServer;

namespace iBank.BroadcastServer.Timing.NextPeriodCalculators
{
    public class YearToDateMonthlyCalculator : INextReportPeriodCalculator
    {
        private IRecordTimingDetails _timing;

        public YearToDateMonthlyCalculator(IRecordTimingDetails timing)
        {
            _timing = timing;
        }

        public ReportPeriodDateRange CalculateNextReportPeriod()
        {
            var reportPeriod = new ReportPeriodDateRange(_timing.NextReportPeriodStart, _timing.NextReportPeriodEnd);
            var year = _timing.NextReportPeriodEnd.Year;
            var month = _timing.NextReportPeriodEnd.Month + 1;

            if (month > 12)
            {
                year++;
                month = 1;
                reportPeriod.ReportPeriodStart = new DateTime(year, month, 1);
            }

            //get last day of next month
            reportPeriod.ReportPeriodEnd = new DateTime(year, month, 1).AddMonths(1).AddDays(-1);

            return reportPeriod;
        }
    }
}