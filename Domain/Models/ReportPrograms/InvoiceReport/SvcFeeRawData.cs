using System;

using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.InvoiceReport
{
    public class SvcFeeRawData : IRecKey
    {
        public int RecKey { get; set; } = 0;
        public string Recloc { get; set; } = string.Empty;
        public string Invoice { get; set; } = string.Empty;
        public string Acct { get; set; } = string.Empty;
        public string SfTrantype { get; set; } = string.Empty;
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;
        public DateTime? Trandate { get; set; } = DateTime.MinValue;
        public decimal SvcAmt { get; set; } = 0m;
        public string SvcDesc { get; set; } = string.Empty;
        public string Mco { get; set; } = string.Empty;
        public string SfCardnum { get; set; } = string.Empty;
        public decimal Tax1 { get; set; } = 0m;
        public decimal Tax2 { get; set; } = 0m;
        public decimal Tax3 { get; set; } = 0m;
        public decimal Tax4 { get; set; } = 0m;
    }
}
