using System;

namespace Domain.Models.ReportPrograms.ConcurrentSegmentsBookedReport
{
    public class TempData
    {
        public int RecKey { get; set; } = 0;
        public string RecLoc { get; set; } = string.Empty;
        public string Ticket { get; set; } = string.Empty;
        public string Acct { get; set; } = string.Empty;
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;
        public string BkTool { get; set; } = string.Empty;
        public DateTime? Bookdate { get; set; } = DateTime.MinValue;
        public string Gds { get; set; } = string.Empty;
        public decimal Airchg { get; set; } = 0m;
        public string Origin { get; set; } = string.Empty;
        public string Destinat { get; set; } = string.Empty;
        public string RetOrigin { get; set; } = string.Empty;
        public DateTime? DepartDate { get; set; } = DateTime.MinValue;
        public DateTime? MidDate { get; set; } = DateTime.MinValue;
        public DateTime? ReturnDate { get; set; } = DateTime.MinValue;
        public string Airline { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public int MatchNo { get; set; }
        public string MatchType { get; set; } = string.Empty;
        public string TripType { get; set; } = string.Empty;
    }
}
