using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.ExceptCarReport
{
    public class RawData : IRecKey
    {
        public string Recloc { get; set; }
        public int RecKey { get; set; }
        public string Acct { get; set; }
        public string Break1 { get; set; }
        public string Break2 { get; set; }
        public string Break3 { get; set; }
        public string Passlast { get; set; }
        public string Passfrst { get; set; }
        public string Reascoda { get; set; }
        public DateTime? Rentdate { get; set; }
        [ExchangeDate1]
        public DateTime ReturnDate { get { return Rentdate.GetValueOrDefault().AddDays(Days); } }
        public string Company { get; set; }
        public string Autocity { get; set; }
        public string Autostat { get; set; }
        public int Days { get; set; }
        public string Cartype { get; set; }
        public string Ctypedesc { get; set; }
        public int Cplusmin { get; set; }
        [Currency(RecordType = RecordType.Car)]
        public decimal Abookrat { get; set; }
        [Currency(RecordType = RecordType.Car)]
        public decimal Aexcprat { get; set; }
        public string Cryspgbrk { get; set; }
        [ExchangeDate3]
        public DateTime? BookDate { get; set; }
        [ExchangeDate2]
        public DateTime? InvDate { get; set; }
        [CarCurrency]
        public string CarCurrTyp { get; set; }
    }
}
