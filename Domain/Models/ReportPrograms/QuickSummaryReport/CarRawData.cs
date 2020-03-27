using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.QuickSummaryReport
{
    public class CarRawData : IRecKey
    {
        public CarRawData()
        {
            BookDate = DateTime.MinValue;
            InvDate = DateTime.MinValue;
            CarCurrTyp = string.Empty;
            CarExchangeDate = DateTime.MinValue;
            RecKey = 0;
            Cplusmin = 0;
            Domintl = string.Empty;
            Days = 0;
            Abookrat = 0m;
            Reascoda = string.Empty;
            Aexcprat = 0m;
        }
        [ExchangeDate2]
        public DateTime? BookDate { get; set; }
        [ExchangeDate3]
        public DateTime? InvDate { get; set; }
        [CarCurrency]
        public string CarCurrTyp { get; set; }
        [ExchangeDate1]
        public DateTime? CarExchangeDate { get; set; }
        public int RecKey { get; set; }
        public int Cplusmin { get; set; }
        public string Domintl { get; set; }
        public int Days { get; set; }
        [Currency(RecordType = RecordType.Car)]
        public decimal Abookrat { get; set; }
        public string Reascoda { get; set; }
        [Currency(RecordType = RecordType.Car)]
        public decimal Aexcprat { get; set; }
    }
}
