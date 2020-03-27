using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.QuickSummaryByMonthReport
{
    public class HotelRawData : IRecKey
    {
        public int RecKey { get; set; }
        [HotelCurrency]
        public string HotCurrTyp { get; set; }
        public DateTime? Datecomp { get; set; }
        [ExchangeDate1]
        public DateTime? Invdate { get; set; }
        public DateTime? Depdate { get; set; }
        public DateTime? BookDate { get; set; }
        public int Hplusmin { get; set; }
        public int Nights { get; set; }
        public int Rooms { get; set; }
        [Currency(RecordType = RecordType.Hotel)]
        public decimal Bookrate { get; set; }
        public string Reascodh { get; set; }
        [Currency(RecordType = RecordType.Hotel)]
        public decimal Hexcprat { get; set; }
    }
}
