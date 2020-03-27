using Domain.Helper;
using Domain.Interfaces;
using System;

namespace Domain.Models.ReportPrograms.TopBottomTravelersCar
{
    public class RawData : IRecKey
    {
        public int RecKey { get; set; } = 0;
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;
        [Currency(RecordType = RecordType.Car)]
        public decimal ABookRat { get; set; } = 0m;
        public int Rentals { get; set; } 
        public int Days { get; set; }
        [Currency(RecordType = RecordType.Car)]
        public decimal CarCost { get; set; }
        [Currency(RecordType = RecordType.Car)]
        public decimal sumbkrate { get; set; } 
        public short CPlusMin { get; set; }
        [ExchangeDate1]
        public DateTime? CarExchangeDate { get; set; }
        [CarCurrency]
        public string CarCurrTyp { get; set; }
    }
}
