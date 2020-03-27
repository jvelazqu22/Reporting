using Domain.Helper;
using Domain.Interfaces;
using System;

namespace Domain.Models.ReportPrograms.TopBottomCostCenter
{
    public class CarRawData : IRecKey
    {
        public int RecKey { get; set; } = 0;
        public string GrpCol { get; set; } = string.Empty;
        [Currency(RecordType = RecordType.Car)]
        public decimal ABookRat { get; set; } = 0;
        public int Days { get; set; } = 0;
        public int CPlusMin { get; set; } = 0;
        [ExchangeDate1]
        public DateTime? CarExchangeDate { get; set; }
        [ExchangeDate2]
        public DateTime? BookDate { get; set; }
        [ExchangeDate3]
        public DateTime? InvDate { get; set; }
        [CarCurrency]
        public string CarCurrTyp { get; set; }
    }
}
