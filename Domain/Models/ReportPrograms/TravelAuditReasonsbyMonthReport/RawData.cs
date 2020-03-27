using System;

using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.TravelAuditReasonsbyMonthReport
{
    public class RawData : IRecKey
    {
        public RawData()
        {
            RecKey = 0;
            Acct = string.Empty;
            Depdate = DateTime.MinValue;
            StatusTime = DateTime.MinValue;
            Bookdate = DateTime.MinValue;
            OutPolCods = string.Empty;
            TravAuthNo = 0;
            AuthStatus = string.Empty;
            SGroupNbr = 0;
        }
        public int RecKey { get; set; }
        public string Acct { get; set; }
        public DateTime? Depdate { get; set; }
        public DateTime? StatusTime { get; set; }
        public DateTime? Bookdate { get; set; }
        public string OutPolCods { get; set; }
        public int TravAuthNo { get; set; }
        public string AuthStatus { get; set; }
        public int SGroupNbr { get; set; }
    }
}
