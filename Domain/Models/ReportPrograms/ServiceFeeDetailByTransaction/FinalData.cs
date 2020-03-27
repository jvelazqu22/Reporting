using Domain.Helper;
using System;

namespace Domain.Models.ReportPrograms.ServiceFeeDetailByTransaction
{
    public class FinalData
    {
        public string Acct { get; set; } = string.Empty;
        public string Acctdesc { get; set; } = string.Empty;
        public string Descript { get; set; } = string.Empty;
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;
        [ExchangeDate1]
        public DateTime Trandate { get; set; } = DateTime.Now;
        public string Recloc { get; set; } = string.Empty;
        [Currency(RecordType = RecordType.SvcFee)]
        public decimal Svcfee { get; set; } = 0m;
        [FeeCurrency]
        public string SvcFeeCurrType { get; set; } = string.Empty;
        [ExchangeDate2]
        public DateTime Depdate { get; set; } = DateTime.Now;
        public string Invoice { get; set; } = string.Empty;
        public string Ticket { get; set; } = string.Empty;
        public string Itinerary { get; set; } = string.Empty;
        public string Brkfield { get; set; } = string.Empty;
    }
}
