using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.TopBottomValidatingCarriers
{
    public class RawData:IRecKey
    {
        public RawData()
        {
            RecKey = 0;
            Rdepdate = DateTime.MinValue;
            BookDate = DateTime.MinValue;
            InvDate = DateTime.MinValue;
            ValCarr = string.Empty;
            SourceAbbr = string.Empty;
            ValCarMode = string.Empty;
            AirChg = 0m;
            Mktfare = 0m;
            Stndchg = 0m;
            Offrdchg = 0m;
            Acommisn = 0m;
            Basefare = 0m;
            Plusmin = 0;
        }
        public int RecKey { get; set; }
        [ExchangeDate2]
        public DateTime? Rdepdate { get; set; }
        [ExchangeDate3]
        public DateTime? BookDate { get; set; }
        [ExchangeDate1]
        public DateTime? InvDate { get; set; }
        [AirCurrency]
        public string AirCurrTyp { get; set; } 
        public string ValCarr { get; set; }
        public string SourceAbbr { get; set; }
        public string ValCarMode { get; set; }
        [Currency(RecordType = RecordType.Air)]
        public decimal AirChg { get; set; }
        public decimal Mktfare { get; set; }
        public decimal Stndchg { get; set; }
        public decimal Offrdchg { get; set; }
        public decimal Acommisn { get; set; }
        public decimal Basefare { get; set; }
        public int Plusmin { get; set; }

    }
}