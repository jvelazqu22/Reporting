using Domain.Helper;
using Domain.Interfaces;
using System;

namespace Domain.Models.ReportPrograms.TravelManagementSummary
{
    public class CarRawData : ICarbonCalculationsCar, IRecKey
    {
        public int RecKey { get; set; } = 0;
        [ExchangeDate3]
        public DateTime BookDate { get; set; } = DateTime.MinValue;
        [ExchangeDate2]
        public DateTime InvDate { get; set; } = DateTime.MinValue;
        public DateTime RentDate { get; set; } = DateTime.MinValue;

        public DateTime ReturnDate { get { return RentDate.AddDays(Days); } }

        [CarCurrency]
        public string CarCurrTyp { get; set; } = string.Empty;

        [ExchangeDate1]
        public DateTime UseDate { get; set; } = DateTime.MinValue;

        public int Days { get; set; } = 0;
        public decimal CarCo2 { get; set; } = 0m;
        public int CPlusMin { get; set; } = 0;
        [Currency(RecordType = RecordType.Car)]
        public decimal Abookrat { get; set; } = 0m;
        public int Numcars { get; set; } = 0;
        public string CarType { get; set; } = string.Empty;
    }
}
