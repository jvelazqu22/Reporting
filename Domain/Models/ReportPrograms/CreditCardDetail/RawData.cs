using System;
using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.CreditCardDetail
{
    public class RawData : IRecKey
    {
        public int RecKey { get; set; }
        public string CardType { get; set; } = string.Empty;
        public string CardNum { get; set; } = string.Empty;
        public string RefNbr { get; set; } = string.Empty;
        [ExchangeDate1]
        public DateTime? PostDate { get; set; } = DateTime.MinValue;
        [ExchangeDate2]
        public DateTime? TranDate { get; set; } = DateTime.MinValue;
        public string RecType { get; set; } = string.Empty;
        public string MerchName { get; set; } = string.Empty;
        public string MerchAddr1 { get; set; } = string.Empty;
        public string MerchCity { get; set; } = string.Empty;
        public string MerchState { get; set; } = string.Empty;
        public string MerchSIC { get; set; } = string.Empty;
        public decimal TransAmt { get; set; } = 0m;
        public decimal TaxAmt { get; set; } = 0m;
    }
}
