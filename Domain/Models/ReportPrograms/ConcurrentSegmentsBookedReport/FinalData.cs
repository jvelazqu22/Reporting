using System;

namespace Domain.Models.ReportPrograms.ConcurrentSegmentsBookedReport
{
    public class FinalData
    {
        public string Acct { get; set; } = string.Empty;
        public string Acctdesc { get; set; } = string.Empty;
        public string Matchtype { get; set; } = string.Empty;
        public string Matchdesc { get; set; } = string.Empty;
        public int Matchno { get; set; }
        public string Airline { get; set; } = string.Empty;
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;
        public DateTime Departdate { get; set; } = DateTime.MinValue;
        public DateTime Middate { get; set; } = DateTime.MinValue;
        public string Orgdesc { get; set; } = string.Empty;
        public string Destdesc { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public string RecLoc { get; set; } = string.Empty;
        public string Gdsdesc { get; set; } = string.Empty;
        public string Bktooldesc { get; set; } = string.Empty;
        public string Ticket { get; set; } = string.Empty;
        public DateTime Bookdate { get; set; } = DateTime.MinValue;
        public decimal Airchg { get; set; }

        public int RecKey { get; set; }
        public string BkTool { get; set; } = string.Empty;
    }
}
