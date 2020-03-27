using System;

namespace Domain.Models.ReportPrograms.TravelManagementSummary
{
    public class MonthInfo
    {
        public DateTime BeginMonth { get; set; } = DateTime.MinValue;
        public DateTime EndMonth { get; set; } = DateTime.MinValue;
        public string MonthAbbreviation { get; set; } = string.Empty;
        public int MonthNumber { get; set; } = 0;
    }
}
