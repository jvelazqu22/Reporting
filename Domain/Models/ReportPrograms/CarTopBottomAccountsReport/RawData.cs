using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.CarTopBottomAccountsReport
{
    public class RawData : IRecKey
    {
        public int RecKey { get; set; } = 0;
        public string SourceAbbr { get; set; } = string.Empty;
        public string Acct { get; set; } = string.Empty;
        public string GroupAccount { get; set; } = string.Empty;
        [Currency(RecordType = RecordType.Car)]
        public decimal ABookRat { get; set; } = 0m;
        public int Rentals { get; set; } = 0;
        public int Days { get; set; } = 0;
        [Currency(RecordType = RecordType.Car)]
        public decimal CarCost { get; set; } = 0m;
        [Currency(RecordType = RecordType.Car)]
        public decimal sumbkrate { get; set; } = 0m;
        public short CPlusMin { get; set; }
        public decimal BookCnt { get; set; } = 0m;
        [ExchangeDate1]
        public DateTime? CarExchangeDate { get; set; }
        [CarCurrency]
        public string CarCurrTyp { get; set; }
    }
}
