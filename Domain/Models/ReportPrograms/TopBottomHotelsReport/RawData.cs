using Domain.Helper;
using Domain.Interfaces;
using System;

namespace Domain.Models.ReportPrograms.TopBottomHotelsReport
{
    public class RawData : IRecKey
    {
        public int RecKey { get; set; }
        public DateTime? BookDate { get; set; } = DateTime.MinValue;
        public DateTime? InvDate { get; set; } = DateTime.MinValue;
        [HotelCurrency]
        public string HotCurrTyp { get; set; } = string.Empty;
        [ExchangeDate1]
        public DateTime? HotelExchangeDate { get; set; } = DateTime.MinValue;
        [Currency(RecordType = RecordType.Hotel)]
        public decimal Bookrate { get; set; } = 0m;
        public int Nights { get; set; } = 0;
        public int Rooms { get; set; } = 0;
        public int Hplusmin { get; set; } = 0;
        public string Chaincod { get; set; } = string.Empty;
        public string Hotcity { get; set; } = string.Empty;
        public string Hotstate { get; set; } = string.Empty;
        public string Hotcountry { get; set; } = string.Empty;
        public string Metro { get; set; } = string.Empty;
        public string Hotelnam { get; set; } = string.Empty;
    }
}
