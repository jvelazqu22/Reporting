using Domain.Interfaces;
using System;

namespace Domain.Models.ReportPrograms.CCReconReport
{
    public class ServiceFee : IRecKey
    {
        public string FeeCurrTyp { get; set; } = string.Empty;
        public int RecKey { get; set; }
        public string Acct { get; set; } = string.Empty;
        public string Break1 { get; set; } = string.Empty;
        public string Break2 { get; set; } = string.Empty;
        public string Break3 { get; set; } = string.Empty;
        public string RecLoc { get; set; } = string.Empty;
        public string Cardnum { get; set; } = string.Empty;
        public string Ticket { get; set; } = string.Empty;
        public DateTime Trandate { get; set; }
        public string Invoice { get; set; } = string.Empty;
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;
        public decimal SvcFee { get; set; }
        public string Descript { get; set; }
        public string Trantype { get; set; } = string.Empty;
        public string SfCardNum { get; set; } = string.Empty;
        public string Mco { get; set; } = string.Empty;
        public decimal Airchg { get; set; }
        public DateTime? Depdate { get; set; } = DateTime.Now;
        public DateTime? Arrdate { get; set; } = DateTime.Now;

    }
}
