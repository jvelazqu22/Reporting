using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.ValidatingCarrierReport
{
    public class RawData : IRecKey
    {
        public RawData()
        {
            BookDate = DateTime.MinValue;
            InvDate = DateTime.MinValue;

            AirCurrTyp = string.Empty;
            Valcarr = string.Empty;
            ValcarMode = string.Empty;
            Plusmin = 0;
            Acommisn = 0m;
            Airchg = 0m;
        }
        public int RecKey { get; set; }
        [ExchangeDate2]
        public DateTime? BookDate { get; set; }
        [ExchangeDate1]
        public DateTime? InvDate { get; set; }

        [AirCurrency]
        public string AirCurrTyp { get; set; }
        public string Valcarr { get; set; }
        public string ValcarMode { get; set; }
        public int Plusmin { get; set; }
        [Currency(RecordType = RecordType.Air)]
        public decimal Acommisn { get; set; }
        [Currency(RecordType = RecordType.Air)]
        public decimal Airchg { get; set; }
    }
}
