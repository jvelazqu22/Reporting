using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.QuickSummaryByMonthReport
{
    public class AirRawData : IRecKey
    {
        public AirRawData()
        {
            Datecomp = DateTime.MinValue;
            Depdate = DateTime.MinValue;
            BookDate = DateTime.MinValue;
            Plusmin = 0;
            Reascode = string.Empty;
            Savingcode = string.Empty;
            Airchg = 0m;
            Stndchg = 0m;
            Offrdchg = 0m;
        }
        public int RecKey { get; set; }
        [AirCurrency]
        public string AirCurrTyp { get; set; }
        public DateTime? Datecomp { get; set; }
        [ExchangeDate1]
        public DateTime? InvDate { get; set; }
        public DateTime? Depdate { get; set; }
        public DateTime? BookDate { get; set; }
        public int Plusmin { get; set; }
        public string Reascode { get; set; }
        public string Savingcode { get; set; }
        [Currency(RecordType = RecordType.Air)]
        public decimal Airchg { get; set; }
        [Currency(RecordType = RecordType.Air)]
        public decimal Stndchg { get; set; }
        [Currency(RecordType = RecordType.Air)]
        public decimal Offrdchg { get; set; }
    }
    
}
