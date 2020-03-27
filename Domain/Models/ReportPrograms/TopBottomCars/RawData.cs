using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.TopBottomCars
{
    public class RawData : IRecKey
    {
        public DateTime? BookDate { get; set; } = DateTime.MinValue;
        public DateTime? InvDate { get; set; } = DateTime.MinValue;
        [CarCurrency]
        public string CarCurrTyp { get; set; } = string.Empty;
        [ExchangeDate1]
        public DateTime? CarExchangeDate { get; set; } = DateTime.MinValue;
        [AirCurrency]
        public string AirCurrTyp { get; set; } = string.Empty;
        public string Autocity { get; set; } = string.Empty;
        public string Autostat { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public int Cplusmin { get; set; } = 0;
        public decimal Abookrat { get; set; } = 0m;
        public int Days { get; set; } = 0;
        public int RecKey { get; set; }
    }
}
