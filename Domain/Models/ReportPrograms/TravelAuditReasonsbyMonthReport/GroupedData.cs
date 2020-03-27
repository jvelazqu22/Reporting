using System;

namespace Domain.Models.TravelAuditReasonsbyMonthReport
{
    public class GroupedData
    {
        public GroupedData()
        {
            RecKey = 0;
            Acct = string.Empty;
            OutPolCods = string.Empty;
            UseDate = DateTime.MinValue;
            RecCntr = 1;
            AuthStatus = string.Empty;
        }

        public int RecKey { get; set; }
        public string Acct { get; set; }
        public string OutPolCods { get; set; }
        public DateTime UseDate { get; set; }
        public int RecCntr { get; set; }
        public string AuthStatus { get; set; }
    }
}
