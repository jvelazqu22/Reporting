using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.TravelManagementSummary
{
    public class HotelRawData : ICarbonCalculationsHotel, IRecKey
    {        
        public int RecKey { get; set; } = 0;
        [ExchangeDate3]
        public DateTime BookDate { get; set; } = DateTime.MinValue;
        [ExchangeDate2]
        public DateTime InvDate { get; set; } = DateTime.MinValue;
        public DateTime CcBegDate { get; set; }
        [HotelCurrency]
        public string HotCurrTyp { get; set; } = string.Empty;
        [ExchangeDate1]
        public DateTime UseDate { get; set; } = DateTime.MinValue;
        public int Nights { get; set; } = 0;
        public int Rooms { get; set; } = 0;
        public decimal HotelCo2 { get; set; }
        public int HPlusMin { get; set; } = 0;
        [Currency(RecordType = RecordType.Hotel)]
        public decimal Bookrate { get; set; } = 0m;
    }

}
