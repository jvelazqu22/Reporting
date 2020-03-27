using System;

namespace Domain.Models.ReportPrograms.AccountSummary12MonthTrend
{
    public class GroupedRecord
    {
        public GroupedRecord()
        {
            Acct = string.Empty;
            Name = string.Empty;
            UsedDate = DateTime.Now;
        }
        public string Acct { get; set; }
        public string Name { get; set; }
        public DateTime? UsedDate { get; set; }
        public int Trips { get; set; }
        public decimal Amount { get; set; }
        public int RecordCount { get; set; }
    }
}
