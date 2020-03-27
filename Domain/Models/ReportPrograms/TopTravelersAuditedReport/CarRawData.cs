using Domain.Helper;
using Domain.Interfaces;
using System;

namespace Domain.Models.ReportPrograms.TopTravelersAuditedReport
{
    public class CarRawData: IRecKey
    {
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;
        public int RecKey { get; set; } = 0;
        public int Days { get; set; } = 0;
        public int Numcars { get; set; } = 0;
        [Currency(RecordType = RecordType.Car)]
        public decimal Abookrat { get; set; } = 0m;
        public int TravAuthNo { get; set; } = 0;
        [ExchangeDate2]
        public DateTime? BookDate { get; set; } = DateTime.MinValue;
        [ExchangeDate3]
        public DateTime? InvDate { get; set; } = DateTime.MinValue;
        [CarCurrency]
        public string CarCurrTyp { get; set; } = string.Empty;
        [ExchangeDate1]
        public DateTime? CarExchangeDate { get; set; } = DateTime.MinValue;
    }
}
