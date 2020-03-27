using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.ExecutiveSummaryYearToYearReport
{
    public class HotelRawData : ICarbonCalculationsHotel, IRecKey
    {
        [HotelCurrency]
        public string HotCurrTyp { get; set; }
        public int RecKey { get; set; }
        public DateTime? Bookdate { get; set; }
        [ExchangeDate1]
        public DateTime? HotelExchangeDate { get; set; }
        [ExchangeDate2]
        public DateTime? Invdate { get; set; }
        public DateTime? UseDate { get; set; } = DateTime.MinValue;
        public int Nights { get; set; } = 0;
        public int Rooms { get; set; } = 0;
        [Currency(RecordType = RecordType.Hotel)]
        public decimal HotelCo2 { get; set; }
        public int HPlusMin { get; set; } = 0;
        [Currency(RecordType = RecordType.Hotel)]
        public decimal Bookrate { get; set; } = 0m;

    }
}
