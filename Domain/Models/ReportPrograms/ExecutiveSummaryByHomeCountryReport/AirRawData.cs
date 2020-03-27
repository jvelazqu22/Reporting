using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.ExecutiveSummaryByHomeCountryReport
{
    public class AirRawData : IRecKey
    {
        public string HomeCtry { get; set; } = string.Empty;
        [ExchangeDate2]
        public DateTime? BookDate { get; set; }
        [ExchangeDate1]
        public DateTime? InvDate { get; set; }
        [AirCurrency]
        public string AirCurrTyp { get; set; }
        public int RecKey { get; set; } = 0;
        public string SourceAbbr { get; set; } = string.Empty;
        public int PlusMin { get; set; } = 0;
        [Currency(RecordType = RecordType.Air)]
        public decimal AirChg { get; set; } = 0;
        [Currency(RecordType = RecordType.Air)]
        public decimal StndChg { get; set; } = 0;
        [Currency(RecordType = RecordType.Air)]
        public decimal OffrdChg { get; set; } = 0;
        public string ValCarr { get; set; } = string.Empty;
        public DateTime? DepDate { get; set; } = DateTime.Now;
        public DateTime? ArrDate { get; set; } = DateTime.Now;
        public string Valcarmode { get; set; } = string.Empty;
    }
}
