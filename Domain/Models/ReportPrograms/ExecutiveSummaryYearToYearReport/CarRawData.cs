using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.ExecutiveSummaryYearToYearReport
{
    public class CarRawData : ICarbonCalculationsCar, IRecKey
    {
        [CarCurrency]
        public string CarCurrTyp { get; set; }
        public DateTime UseDate { get; set; } = DateTime.MinValue;
        public int Days { get; set; } = 0;
        [Currency(RecordType = RecordType.Car)]
        public decimal CarCo2 { get; set; }
        public int CPlusMin { get; set; } = 0;
        [Currency(RecordType = RecordType.Car)]
        public decimal Abookrat { get; set; } = 0m;
        public int Numcars { get; set; } = 0;
        public string CarType { get; set; } = string.Empty;
        public int RecKey { get; set; }
        public DateTime? Bookdate { get; set; }
        [ExchangeDate1]
        public DateTime? CarExchangeDate { get; set; }
        [ExchangeDate2]
        public DateTime? Invdate { get; set; }
    }
}
