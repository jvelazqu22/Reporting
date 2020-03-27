using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.ExecutiveSummarywithGraphsReport
{
    public class FeeRawData : IRecKey
    {
        public FeeRawData()
        {
            RecKey = 0;
            RecLoc = string.Empty;
            Invoice = string.Empty;
            Acct = string.Empty;
            PassLast = string.Empty;
            PassFrst = string.Empty;
            SvcAmt = 0;
        }
        public int RecKey { get; set; }
        public string RecLoc { get; set; }
        public string Invoice { get; set; }
        public string Acct { get; set; }
        public string PassLast { get; set; }
        public string PassFrst { get; set; }

        [Currency(RecordType = RecordType.Air)]
        public decimal SvcAmt { get; set; }
        [ExchangeDate1]
        public DateTime? BookDate { get; set; }
        [ExchangeDate2]
        public DateTime? InvDate { get; set; }
        [AirCurrency]
        public string AirCurrTyp { get; set; }
    }
}
