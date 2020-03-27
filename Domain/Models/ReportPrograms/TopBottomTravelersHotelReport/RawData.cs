using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.TopBottomTravelersHotelReport
{
    public class RawData: IRecKey
    {
        public int RecKey { get; set; } = 0;
        public string PassLast { get; set; } = string.Empty;
        public string PassFrst { get; set; } = string.Empty;
        public int Rooms { get; set; } = 0;
        public int Stays { get; set; } = 0;
        public int Nights { get; set; } = 0;
        public int HPlusMin { get; set; } = 0;
        public DateTime dateIn { get; set; } = DateTime.MinValue;
        [Currency(RecordType = RecordType.Hotel)]
        public decimal BookRate { get; set; } = 0;
        [ExchangeDate1]
        public DateTime HotelExchangeDate { get; set; } = DateTime.MinValue;
        [ExchangeDate2]
        public DateTime BookDate { get; set; } = DateTime.MinValue;
        [ExchangeDate3]
        public DateTime InvDate { get; set; } = DateTime.MinValue;
        [HotelCurrency]
        public string HotCurrTyp { get; set; } = string.Empty;
    }
}
