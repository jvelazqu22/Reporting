using System;

namespace Domain.Models.ReportPrograms.PublishedSavings
{
    public class FinalData
    {
        public int Reckey { get; set; } = 0;
        public string Recloc { get; set; } = string.Empty;
        public string Invoice { get; set; } = string.Empty;
        public string Ticket { get; set; } = string.Empty;
        public string Acct { get; set; } = string.Empty;
        public string Acctdesc { get; set; } = string.Empty;
        public string Break1 { get; set; } = string.Empty;
        public string Break2 { get; set; } = string.Empty;
        public string Break3 { get; set; } = string.Empty;
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;
        public string Savingcode { get; set; } = string.Empty;
        public string Svngdesc { get; set; } = string.Empty;
        public decimal Airchg { get; set; } = 0m;
        public decimal Stndchg { get; set; } = 0m;
        public DateTime Depdate { get; set; } = DateTime.MinValue;
        public string Origin { get; set; } = string.Empty;
        public string Orgdesc { get; set; } = string.Empty;
        public string Destinat { get; set; } = string.Empty;
        public string Destdesc { get; set; } = string.Empty;
        public string Connect { get; set; } = string.Empty;
        public DateTime Rdepdate { get; set; } = DateTime.MinValue;
        public string Airline { get; set; } = string.Empty;
        public string Fltno { get; set; } = string.Empty;
        public string Carrdesc { get; set; } = string.Empty;
        public decimal Savings { get; set; } = 0m;
        public string Class { get; set; } = string.Empty;
        public int Seqno { get; set; } = 0;
        public string Cryspgbrk { get; set; } = string.Empty;
        public string ReasonCode { get; set; } = string.Empty;
        public string OrigAcct { get; set; } = string.Empty;
    }
}
