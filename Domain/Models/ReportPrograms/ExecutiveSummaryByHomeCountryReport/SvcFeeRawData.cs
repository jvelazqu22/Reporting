using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.ExecutiveSummaryByHomeCountryReport
{
    public class SvcFeeRawData : IRecKey
    {
        public string SourceAbbr { get; set; } = string.Empty;
        public string HomeCtry { get; set; } = string.Empty;
        [FeeCurrency]
        public string FeeCurrTyp { get; set; }
        [ExchangeDate2]
        public DateTime? BookDate { get; set; }
        [ExchangeDate1]
        public DateTime? InvDate { get; set; }
        [Currency(RecordType = RecordType.SvcFee)]
        public decimal SvcAmt { get; set; } = 0;

        public int RecKey { get; set; }
    }
}
