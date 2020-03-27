using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.QuickSummaryReport
{
    public class RawData : IRecKey
    {
        public RawData()
        {
            BookDate = DateTime.MinValue;
            InvDate = DateTime.MinValue;
            AirCurrTyp = string.Empty;
            Plusmin = 0;
            Domintl = string.Empty;
            Reascode = string.Empty;
            Savingcode = string.Empty;
            Mktfare = 0m;
            Airchg = 0m;
            Offrdchg = 0m;
            Stndchg = 0m;
            Savings = 0m;
            ValcarMode = string.Empty;
            Negosvngs = 0m;
            Lostamt = 0m;
        }
        public int RecKey { get; set; }
        [ExchangeDate1]
        public DateTime? BookDate { get; set; }
        [ExchangeDate2]
        public DateTime? InvDate { get; set; }
        [AirCurrency]
        public string AirCurrTyp { get; set; }
        public int Plusmin { get; set; }
        public string Domintl { get; set; }
        public string Reascode { get; set; }
        public string Savingcode { get; set; }
        [Currency(RecordType = RecordType.Air)]
        public decimal Mktfare { get; set; }
        [Currency(RecordType = RecordType.Air)]
        public decimal Airchg { get; set; }
        [Currency(RecordType = RecordType.Air)]
        public decimal Offrdchg { get; set; }
        [Currency(RecordType = RecordType.Air)]
        public decimal Stndchg { get; set; }
        [Currency(RecordType = RecordType.Air)]
        public decimal Savings { get; set; }
        public string ValcarMode { get; set; }
        [Currency(RecordType = RecordType.Air)]
        public decimal Negosvngs { get; set; }
        [Currency(RecordType = RecordType.Air)]
        public decimal Lostamt { get; set; }
        
    }
}
