using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.InvoiceReport
{
    public class CarRawData : IRecKey
    {
        [ExchangeDate1]
        public DateTime? Invdate { get; set; } = DateTime.MinValue;
        public int RecKey { get; set; } = 0;
        public string Company { get; set; } = string.Empty;
        public string Autocity { get; set; } = string.Empty;
        public string Autostat { get; set; } = string.Empty;
        public int Days { get; set; } = 0;
        [ExchangeDate2]
        public DateTime? Rentdate { get; set; } = DateTime.MinValue;
        public string Cartype { get; set; } = string.Empty;
        [Currency(RecordType = RecordType.Car)]
        public decimal Abookrat { get; set; } = 0m;
        public int Cplusmin { get; set; } = 0;
        public int Numcars { get; set; } = 0;
        public string Confirmno { get; set; } = string.Empty;
        public bool Invbyagcy { get; set; } = false;
        public string CarTranTyp { get; set; } = string.Empty;
        [CarCurrency]
        public string MoneyType { get; set; } = string.Empty;
        public DateTime? DateBack { get; set; } = DateTime.MinValue;
    }
}
