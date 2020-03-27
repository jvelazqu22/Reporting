using Domain.Helper;
using System;

namespace Domain.Models.ReportPrograms.ServiceFeeDetailByTripReport
{
    public class FinalData
    {
        public int Reckey { get; set; }
        public int Svcfeerec { get; set; }
        public string Acct { get; set; }
        public string Acctdesc { get; set; }
        public string Passlast { get; set; }
        public string Passfrst { get; set; }
        public string Recloc { get; set; }
        public string Invoice { get; set; }
        public string Ticket { get; set; }
        public decimal Airchg { get; set; }
        public DateTime Depdate { get; set; }
        public DateTime Invdate { get; set; }
        public string Itinerary { get; set; }
        [Currency(RecordType = RecordType.SvcFee)]
        public decimal Svcfee { get; set; }
        [FeeCurrency]
        public string FeeCurrTyp { get; set; }
        [ExchangeDate1]
        public DateTime Trandate { get; set; }
        public string Descript { get; set; }
    }
}
