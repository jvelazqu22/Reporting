using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.CO2CombinedSummaryReport
{
    public class HotelRawData : IRecKey, ICarbonCalculationsHotel
    {
        [ExchangeDate2]
        public DateTime? BookDate { get; set; } = DateTime.MinValue;
        [ExchangeDate3]
        public DateTime? InvDate { get; set; } = DateTime.MinValue;
        public DateTime? UseDate { get; set; } = DateTime.MinValue;
        [HotelCurrency]
        public string HotCurrTyp { get; set; } = string.Empty;
        [ExchangeDate1]
        public DateTime? HotelExchangeDate { get; set; } = DateTime.MinValue;
        public string Chaincod { get; set; } = string.Empty;
        public int Nights { get; set; } = 0;
        public int Rooms { get; set; } = 0;
        public decimal HotelCo2 { get; set; }
        public int HPlusMin { get; set; } = 0;

        [Currency(RecordType = RecordType.Hotel)]
        public decimal Bookrate { get; set; } = 0m;
        public string Hotcity { get; set; } = string.Empty;
        public string Hotstate { get; set; } = string.Empty;
        public int RecKey { get; set; }
    }
}
