using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.QuickSummaryByMonthReport
{
    public class CarRawData : IRecKey
    {
        public int RecKey { get; set; }
        [CarCurrency]
        public string CarCurrTyp { get; set; }
        public DateTime? Datecomp { get; set; }
        [ExchangeDate1]
        public DateTime? Invdate { get; set; }
        public DateTime? Depdate { get; set; }
        public DateTime? BookDate { get; set; }
        public int Cplusmin { get; set; }
        public int Days { get; set; }
        [Currency(RecordType = RecordType.Car)]
        public decimal Abookrat { get; set; }
        public string Reascoda { get; set; }
        [Currency(RecordType = RecordType.Car)]
        public decimal Aexcprat { get; set; }
    }
}
