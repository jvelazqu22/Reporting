using Domain.Interfaces;
using System;
using Domain.Helper;

namespace Domain.Models.ReportPrograms.ServiceFeeDetailByTransaction
{
    public class RawData : IRecKey
    {
        public DateTime? BookDate { get; set; } = DateTime.Now;
        [FeeCurrency]
        public string FeeCurrTyp { get; set; } = string.Empty;
        public string AirCurrTyp { get; set; } = string.Empty;
        public int RecKey { get; set; } = 0;
        public string Acct { get; set; } = string.Empty;
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;
        [ExchangeDate2]
        public DateTime? Depdate { get; set; } = DateTime.Now;
        [ExchangeDate1]
        public DateTime? Trandate { get; set; } = DateTime.Now;
        public string Ticket { get; set; } = string.Empty;
        public string Invoice { get; set; } = string.Empty;
        public DateTime? Invdate { get; set; } = DateTime.Now;
        public string Recloc { get; set; } = string.Empty;
        public string SvcDesc { get; set; } = string.Empty;
        [Currency(RecordType = RecordType.SvcFee)]
        public decimal? SvcAmt { get; set; } = 0;
    }
}
