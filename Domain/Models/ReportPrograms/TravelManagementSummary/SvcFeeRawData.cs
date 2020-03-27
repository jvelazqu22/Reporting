using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.TravelManagementSummary
{
    public class SvcFeeRawData : IRecKey
    {
        public SvcFeeRawData()
        {
            RecKey = 0;
            FeeCurrTyp = string.Empty;
            SvcAmt = 0;
            UseDate = DateTime.MinValue;
            SvcFeeCount = 0;
        }
        public int RecKey { get; set; }
        [FeeCurrency]
        public string FeeCurrTyp { get; set; }
        [Currency(RecordType = RecordType.SvcFee)]
        public decimal SvcAmt { get; set; }
        public int SvcFeeCount { get; set; }
        [ExchangeDate1]
        public DateTime? UseDate { get; set; }
    }
}
