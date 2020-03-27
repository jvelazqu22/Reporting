using Domain.Helper;
using Domain.Interfaces;
using System;

namespace Domain.Models.ReportPrograms.CarFareSavings
{
    public class RawData : IRecKey
    {
        [ExchangeDate1]
        public DateTime? CarExchangeDate { get; set; }
        [ExchangeDate2]
        public DateTime? Invdate { get; set; } = DateTime.MinValue;
        [ExchangeDate3]
        public DateTime? RentDate { get; set; } = DateTime.MinValue;
        public DateTime? BookDate { get; set; } = DateTime.MinValue;
        [CarCurrency]
        public string CarCurrTyp { get; set; }
        public int RecKey { get; set; } = 0;
        public string Acct { get; set; } = string.Empty;
        public string Break1 { get; set; } = string.Empty;
        public string Break2 { get; set; } = string.Empty;
        public string Break3 { get; set; } = string.Empty;
        public string SourceAbbr { get; set; } = string.Empty;
        public string ConfirmNo { get; set; } = string.Empty;
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;

        public string Company { get; set; } = string.Empty;
        public string Autocity { get; set; } = string.Empty;
        public string Autostat { get; set; } = string.Empty;
        public int Days { get; set; } = 0;
        public string Cartype { get; set; } = string.Empty;
        [Currency(RecordType = RecordType.Car)]
        public decimal AExcpRat { get; set; } = 0m;
        [Currency(RecordType = RecordType.Car)]
        public decimal ABookRat { get; set; } = 0m;
        public string Reascoda { get; set; } = string.Empty;
        public int Cplusmin { get; set; } = 0;
        [Currency(RecordType = RecordType.Car)]
        public decimal CarStdRate { get; set; } = 0m;
        public string CarSvgCode { get; set; } = string.Empty;
    }
}
