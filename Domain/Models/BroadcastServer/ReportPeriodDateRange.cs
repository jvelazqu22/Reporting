using System;

namespace Domain.Models.BroadcastServer
{
    public class ReportPeriodDateRange
    {
        public DateTime ReportPeriodStart { get; set; }
        public DateTime ReportPeriodEnd { get; set; }

        public ReportPeriodDateRange() {}

        public ReportPeriodDateRange(DateTime startDate, DateTime endDate)
        {
            ReportPeriodStart = startDate;
            ReportPeriodEnd = endDate;
        }
    }
}
