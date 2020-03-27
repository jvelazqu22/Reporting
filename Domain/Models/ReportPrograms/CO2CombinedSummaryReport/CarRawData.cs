using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.CO2CombinedSummaryReport
{

    public class CarRawData : IRecKey, ICarbonCalculationsCar
    {
        [ExchangeDate2]
        public DateTime? BookDate { get; set; } = DateTime.MinValue;
        [ExchangeDate3]
        public DateTime? InvDate { get; set; } = DateTime.MinValue;
        public DateTime? UseDate { get; set; } = DateTime.MinValue;
        [CarCurrency]
        public string CarCurrTyp { get; set; } = string.Empty;
        [ExchangeDate1]
        public DateTime? CarExchangeDate { get; set; } = DateTime.MinValue;
        public int Days { get; set; } = 0;
        public decimal CarCo2 { get; set; } = 0m;
        public int CPlusMin { get; set; } = 0;

        [Currency(RecordType = RecordType.Car)]
        public decimal Abookrat { get; set; } = 0m;
        public string Autocity { get; set; } = string.Empty;
        public string Autostat { get; set; } = string.Empty;
        public string CarType { get; set; } = string.Empty;
        public int RecKey { get; set; } = 0;
    }
}
