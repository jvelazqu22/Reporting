using System;
using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.ExecutiveSummarywithGraphsReport
{
    public class RawData : IRecKey
    {
        public int RecKey { get; set; } = 0;
        public int Plusmin { get; set; } = 0;
        public string Valcarr { get; set; } = string.Empty;
        public string Reascode { get; set; } = string.Empty;
        public string Savingcode { get; set; } = string.Empty;
        [Currency(RecordType = RecordType.Air)]
        public decimal Airchg { get; set; } = 0m;
        [Currency(RecordType = RecordType.Air)]
        public decimal Stndchg { get; set; } = 0m;
        [Currency(RecordType = RecordType.Air)]
        public decimal Mktfare { get; set; } = 0m;
        [Currency(RecordType = RecordType.Air)]
        public decimal Offrdchg { get; set; } = 0m;
        [Currency(RecordType = RecordType.Air)]
        public decimal Basefare { get; set; } = 0m;
        [Currency(RecordType = RecordType.Air)]
        public decimal Svcfee { get; set; } = 0m;
        public string ValcarMode { get; set; } = string.Empty;
        public string Mode { get; set; } = string.Empty;
        public DateTime? UseDate { get; set; } = DateTime.MinValue;

        public string RecLoc { get; set; } = string.Empty;
        public string Invoice { get; set; } = string.Empty;
        public string Acct { get; set; } = string.Empty;
        public string PassLast { get; set; } = string.Empty;
        public string PassFrst { get; set; } = string.Empty;

        [Currency(RecordType = RecordType.Air)]
        public decimal Savings { get; set; } = 0m;
        [Currency(RecordType = RecordType.Air)]
        public decimal NegoSvngs { get; set; } = 0m;
        [Currency(RecordType = RecordType.Air)]
        public decimal LostAmt { get; set; } = 0m;

        [ExchangeDate2]
        public DateTime? BookDate { get; set; }
        [ExchangeDate1]
        public DateTime? InvDate { get; set; }
        [AirCurrency]
        public string AirCurrTyp { get; set; } = string.Empty;
    }
}
