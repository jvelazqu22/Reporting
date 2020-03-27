using Domain.Interfaces.BroadcastServer;
using System;

using Domain.Models.BroadcastServer;

namespace iBank.BroadcastServer.Timing.NextPeriodCalculators
{
    public class ReservationMonWedFriCalculator : INextReportPeriodCalculator
    {
        private IRecordTimingDetails _timing;

        public ReservationMonWedFriCalculator(IRecordTimingDetails timing)
        {
            _timing = timing;
        }

        public ReportPeriodDateRange CalculateNextReportPeriod()
        {
            var reportPeriod = new ReportPeriodDateRange(_timing.NextReportPeriodStart, _timing.NextReportPeriodEnd);

            reportPeriod.ReportPeriodStart = _timing.NextReportPeriodStart.AddDays(2);

            if (reportPeriod.ReportPeriodStart.DayOfWeek == DayOfWeek.Sunday)
            {
                //move to Monday
                reportPeriod.ReportPeriodStart = reportPeriod.ReportPeriodStart.AddDays(1);
            }
            if (reportPeriod.ReportPeriodStart.DayOfWeek == DayOfWeek.Saturday)
            {
                //move to Monday
                reportPeriod.ReportPeriodStart = reportPeriod.ReportPeriodStart.AddDays(2);
            }

            reportPeriod.ReportPeriodEnd = reportPeriod.ReportPeriodStart.AddDays(_timing.ReportDays - 1);

            return reportPeriod;
        }
    }
}