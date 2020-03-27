using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.ExecutiveSummaryYearToYearReport
{
    public class FeeRawData : IRecKey
    {
        [FeeCurrency]
        public string FeeCurrTyp { get; set; }
        public int RecKey { get; set; } = 0;
        public string Recloc { get; set; } = string.Empty;
        public string Invoice { get; set; } = string.Empty;
        public string Acct { get; set; } = string.Empty;
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;
        [Currency(RecordType = RecordType.SvcFee)]
        public decimal SvcAmt { get; set; } = 0m;
        public DateTime? UseDate { get; set; } = DateTime.MinValue;
        public string ValcarMode { get; set; } = string.Empty;
        [ExchangeDate2]
        public DateTime? Bookdate { get; set; }
        [ExchangeDate1]
        public DateTime? Invdate { get; set; }
    }
}
