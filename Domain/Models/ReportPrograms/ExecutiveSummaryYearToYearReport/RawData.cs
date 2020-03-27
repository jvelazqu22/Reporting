using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.ExecutiveSummaryYearToYearReport
{
    public class RawData : IRecKey
    {
        public int RecKey { get; set; }
        public int Plusmin { get; set; } = 0;
        public string Reascode { get; set; } = string.Empty;
        public bool Exchange { get; set; } = false;
        public string Savingcode { get; set; } = string.Empty;
        [ExchangeDate1]
        public DateTime? Invdate { get; set; } = DateTime.MinValue;
        public DateTime? Depdate { get; set; } = DateTime.MinValue;
        public DateTime? ArrDate { get; set; } = DateTime.MinValue;
        public string ValCarMode { get; set; } = string.Empty;
        public int Advance { get; set; } = 0;
        public DateTime UseDate { get; set; } = DateTime.MinValue;

        [AirCurrency]
        public string AirCurrTyp { get; set; }

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
        [Currency(RecordType = RecordType.Air)]
        public decimal Savings { get; set; } = 0m;
        [Currency(RecordType = RecordType.Air)]
        public decimal NegoSvngs { get; set; } = 0m;
        [Currency(RecordType = RecordType.Air)]
        public decimal LostAmt { get; set; } = 0m;
    }
}
