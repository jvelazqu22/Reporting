using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.ServiceFeeSummaryByTransactionReport
{
    public class RawData : IRecKey
    {
        public RawData()
        {
            BookDate = DateTime.MinValue;
            InvDate = DateTime.MinValue;
            TranDate = DateTime.MinValue;
            FeeCurrTyp = string.Empty;
            AirCurrTyp = string.Empty;
            Descript = string.Empty;
            SvcAmt = 0m;
        }
        [ExchangeDate3]
        public DateTime? TranDate { get; set; }
        [ExchangeDate2]
        public DateTime? BookDate { get; set; }
        [ExchangeDate1]
        public DateTime? InvDate { get; set; }
        [FeeCurrency]
        public string FeeCurrTyp { get; set; }
        public string AirCurrTyp { get; set; }
        public string Descript { get; set; }
        [Currency(RecordType = RecordType.SvcFee)]
        public decimal SvcAmt { get; set; }

        public int RecKey { get; set; }
    }
}
