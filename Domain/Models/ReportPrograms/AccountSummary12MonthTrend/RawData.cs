using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.AccountSummary12MonthTrend
{
    public class RawData : IRecKey
    {
        public RawData()
        {
            Acct = string.Empty;
        }
        public int RecKey { get; set; }
        public string Acct { get; set; }
        [Currency(RecordType = RecordType.Air)]
        public decimal AirChg { get; set; }
        [ExchangeDate1]
        public DateTime UseDate { get; set; }
        public int PlusMin { get; set; }
        [AirCurrency]
        public string AirCurrTyp { get; set; }
    }
}
