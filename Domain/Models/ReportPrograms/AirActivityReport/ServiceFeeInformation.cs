using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.AirActivityReport
{
    public class ServiceFeeInformation :IRecKey
    {
        public int RecKey { get; set; }
        [Currency(RecordType = RecordType.SvcFee)]
        public decimal SvcFee { get; set; }
        [FeeCurrency]
        public string AirCurrTyp { get; set; }
        [ExchangeDate1]
        public DateTime? InvDate { get; set; }
        [ExchangeDate2]
        public DateTime? BookDate { get; set; }
    }
}
