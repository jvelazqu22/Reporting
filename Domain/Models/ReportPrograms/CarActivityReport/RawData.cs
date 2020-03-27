using Domain.Helper;
using Domain.Interfaces;
using System;

namespace Domain.Models.ReportPrograms.CarActivityReport
{
    public class RawData : IRecKey
    {
        public int RecKey { get; set; } = 0;
        [ExchangeDate2]
        public DateTime? BookDate { get; set; } = DateTime.MinValue;
        [ExchangeDate3]
        public DateTime? InvDate { get; set; } = DateTime.MinValue;
        [CarCurrency]
        public string CarCurrTyp { get; set; } = string.Empty;
        [ExchangeDate1]
        public DateTime? CarExchangeDate { get; set; } = DateTime.MinValue;

        public string Acct { get; set; } = string.Empty;
        public string Break1 { get; set; } = string.Empty;
        public string Break2 { get; set; } = string.Empty;
        public string Break3 { get; set; } = string.Empty;
        public string Recloc { get; set; } = string.Empty;
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;
        public string Autocity { get; set; } = string.Empty;
        public string Autostat { get; set; } = string.Empty;
        public DateTime? Rentdate { get; set; } = DateTime.Now;
        public string Company { get; set; } = string.Empty;
        public string Cartype { get; set; } = string.Empty;
        public int Days { get; set; } = 0;
        [Currency(RecordType = RecordType.Car)]
        public decimal Abookrat { get; set; } = 0m;
        public string Milecost { get; set; } = string.Empty;
        public string Ratetype { get; set; } = string.Empty;
        public int Cplusmin { get; set; } = 0;

    }
}
