using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.TravelDetail
{
    public class ServiceFeesRawData: IRecKey
    {
        public ServiceFeesRawData() { }

        public ServiceFeesRawData(int recKey, decimal svcFee)
        {
            RecKey = recKey;
            SvcFee = svcFee;
        }

        [FeeCurrency]
        public string AirCurrTyp { get; set; } = string.Empty;

        [ExchangeDate2]
        public DateTime? BookDate { get; set; }

        [ExchangeDate1]
        public DateTime? InvDate { get; set; }

        public int RecKey { get; set; }

        [Currency(RecordType = RecordType.SvcFee)]
        public decimal SvcFee { get; set; }
    }
}