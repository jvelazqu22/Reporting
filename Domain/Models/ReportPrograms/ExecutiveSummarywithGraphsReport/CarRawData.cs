using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.ExecutiveSummarywithGraphsReport
{
    public class CarRawData : IRecKey
    {
        public int RecKey { get; set; } = 0;
        public string Company { get; set; } = string.Empty;
        public int Days { get; set; } = 0;
        public int Cplusmin { get; set; } = 0;
        [Currency(RecordType = RecordType.Car)]
        public decimal Abookrat { get; set; } = 0m;
        public string Reascoda { get; set; } = string.Empty;
        [Currency(RecordType = RecordType.Car)]
        public decimal Aexcprat { get; set; } = 0m;
        public string Autocity { get; set; } = string.Empty;
        public string Autostat { get; set; } = string.Empty;
        public DateTime? UseDate { get; set; } = DateTime.MinValue;

        [ExchangeDate2]
        public DateTime? BookDate { get; set; }
        [ExchangeDate3]
        public DateTime? InvDate { get; set; }
        [ExchangeDate1]
        public DateTime? CarExchangeDate { get; set; }
        [CarCurrency]
        public string CarCurrTyp { get; set; }
    }
}
