using System;

using Domain.Helper;

namespace Domain.Models.ReportPrograms.ServiceFeeSummaryByTransactionReport
{
    public class FinalData
    {
        public FinalData()
        {
            Descript = string.Empty;
            Svcfeecnt = 0;
            Svcfee = 0m;
            FeeCurrTyp = "";
        }
        [FeeCurrency]
        public string FeeCurrTyp { get; set; }
        public string Descript { get; set; }
        public int Svcfeecnt { get; set; }
        [Currency(RecordType = RecordType.SvcFee)]
        public decimal Svcfee { get; set; }
        [ExchangeDate3]
        public DateTime TranDate { get; set; }
        [ExchangeDate2]
        public DateTime BookDate { get; set; }
        [ExchangeDate1]
        public DateTime InvDate { get; set; }
    }
}
