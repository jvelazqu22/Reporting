using System;

namespace Domain.Models.ReportPrograms.AdvanceBookAir
{
    public class FinalData
    {
        public string Acct { get; set; } = string.Empty;
        public string Acctdesc { get; set; } = string.Empty;
        public string Break1 { get; set; } = string.Empty;
        public string Break2 { get; set; } = string.Empty;
        public string Break3 { get; set; } = string.Empty;
        public int RecKey { get; set; } = 0;
        public int Seqno { get; set; } = 0;
        public DateTime Bookdate { get; set; } = DateTime.MinValue;
        public DateTime Depdate { get; set; } = DateTime.MinValue;
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;
        public string Orgdesc { get; set; } = string.Empty;
        public string Destinat { get; set; } = string.Empty;
        public string Destdesc { get; set; } = string.Empty;
        public string Airline { get; set; } = string.Empty;
        public decimal Airchg { get; set; } = 0m;
        public decimal Actfare { get; set; } = 0m;
        public DateTime Rdepdate { get; set; } = DateTime.MinValue;
        public string ClassCode { get; set; } = string.Empty;
        public string Fltno { get; set; } = string.Empty;
        public decimal Bookdays { get; set; } = 0m;
        public string Bookcat { get; set; } = string.Empty;
        public string Catdesc { get; set; } = string.Empty;
        public string Alinedesc { get; set; } = string.Empty;
        public string Cryspgbrk { get; set; } = "1";
    }
}
