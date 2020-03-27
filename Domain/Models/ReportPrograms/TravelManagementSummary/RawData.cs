using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.TravelManagementSummary
{
    public class RawData : IRecKey
    {
        public int RecKey { get; set; } = 0;

        [ExchangeDate3]
        public DateTime? BookDate { get; set; } = DateTime.MinValue;

        [AirCurrency]
        public string AirCurrTyp { get; set; } = string.Empty;

        public int Plusmin { get; set; } = 0;

        public bool Exchange { get; set; } = false;

        [Currency(RecordType = RecordType.Air)]
        public decimal Airchg { get; set; } = 0m;

        [Currency(RecordType = RecordType.Air)]
        public decimal Stndchg { get; set; } = 0m;

        [Currency(RecordType = RecordType.Air)]
        public decimal Mktfare { get; set; } = 0m;

        [Currency(RecordType = RecordType.Air)]
        public decimal Basefare { get; set; } = 0m;
        
        public string Trantype { get; set; } = string.Empty;

        [Currency(RecordType = RecordType.Air)]
        public decimal Offrdchg { get; set; } = 0m;

        [ExchangeDate2]
        public DateTime? Invdate { get; set; } = DateTime.MinValue;

        public DateTime? Depdate { get; set; } = DateTime.MinValue;

        public string Domintl { get; set; } = string.Empty;

        public string Bktool { get; set; } = string.Empty;

        public string Valcarr { get; set; } = string.Empty;

        public string ValcarMode { get; set; } = string.Empty;

        [ExchangeDate1]
        public DateTime? UseDate { get; set; } = DateTime.MinValue;

        //Calculated fields
        [Currency(RecordType = RecordType.Air)]
        public decimal LostAmt { get; set; } = 0m;

        [Currency(RecordType = RecordType.Air)]
        public decimal Savings { get; set; } = 0m;

        [Currency(RecordType = RecordType.Air)]
        public decimal NegotiatedSavings { get; set; } = 0m;
    }
}
