using System;

namespace Domain.Models.ReportPrograms.TripDurationReport
{
    public class FinalData
    {
        public int Reckey { get; set; } = 0;
        public string Acct { get; set; } = string.Empty;
        public string Acctdesc { get; set; } = string.Empty;
        public string Break1 { get; set; } = string.Empty;
        public string Break2 { get; set; } = string.Empty;
        public string Break3 { get; set; } = string.Empty;
        public string Invoice { get; set; } = string.Empty;
        public string Recloc { get; set; } = string.Empty;
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;
        public string Itinerary { get; set; } = string.Empty;
        public DateTime Depdate { get; set; } = DateTime.MinValue;
        public DateTime Arrdate { get; set; } = DateTime.MinValue;
        public int Days { get; set; } = 0;
        public decimal Airchg { get; set; } = 0m;
        public string Cryspgbrk { get; set; } = string.Empty;
    }
}
