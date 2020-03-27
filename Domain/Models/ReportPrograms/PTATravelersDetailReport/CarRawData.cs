using System;
using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.PTATravelersDetailReport
{
    public class CarRawData : IRecKey
    {
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
        public int RecKey { get; set; } = 0;
        public string Company { get; set; } = string.Empty;
        public DateTime? RentDate { get; set; } = DateTime.MinValue;
        [Currency(RecordType = RecordType.Car)]
        public decimal ABookRat { get; set; } = 0m;
        public int Days { get; set; } = 0;
        public decimal AExcpRat { get; set; } = 0m;
        public string ReasCodA { get; set; } = string.Empty;
        public int TravAuthNo { get; set; } = 0;
    }
}
