using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.TravelDetail
{
    public class HibSvcFeesRawData: IRecKey
    {
        [ExchangeDate1]
        public DateTime? TranDate { get; set; }
        [FeeCurrency]
        public string AirCurrTyp { get; set; } = string.Empty;
        public string RecLoc { get; set; } = string.Empty;
        public string PassFrst { get; set; } = string.Empty;
        public string PassLast { get; set; } = string.Empty;
        public string Acct { get; set; } = string.Empty;
        public string Invoice { get; set; } = string.Empty;
        [Currency(RecordType = RecordType.SvcFee)]
        public decimal SvcFee { get; set; } = 0m;
        public int RecKey { get; set; } = 0;
    }
}