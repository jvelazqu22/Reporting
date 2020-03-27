using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.RailActivityReport
{
    public class ServiceFeeInformation : IRecKey
    {
        [FeeCurrency]
        public string AirCurrTyp { get; set; }

        [ExchangeDate2]
        public DateTime BookDate { get; set; }

        [ExchangeDate1]
        public DateTime InvDate { get; set; }

        public int RecKey { get; set; }

        [Currency(RecordType = RecordType.SvcFee)]
        public decimal SvcFee { get; set; }
    }
}
