using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.QuickSummaryReport
{
    public class HotelRawData : IRecKey
    {
        public HotelRawData()
        {
            BookDate = DateTime.MinValue;
            InvDate = DateTime.MinValue;
            HotCurrTyp = string.Empty;
            RecKey = 0;
            Hplusmin = 0;
            Domintl = string.Empty;
            Nights = 0;
            Rooms = 0;
            Bookrate = 0m;
            Reascodh = string.Empty;
            Hexcprat = 0m;
            HotelExchangeDate = DateTime.MinValue;
        }
        [ExchangeDate2]
        public DateTime? BookDate { get; set; }
        [ExchangeDate3]
        public DateTime? InvDate { get; set; }
        [HotelCurrency]
        public string HotCurrTyp { get; set; }
        [ExchangeDate1]
        public DateTime? HotelExchangeDate { get; set; } 
        public int RecKey { get; set; }
        public int Hplusmin { get; set; }
        public string Domintl { get; set; }
        public int Nights { get; set; }
        public int Rooms { get; set; }
        [Currency(RecordType = RecordType.Hotel)]
        public decimal Bookrate { get; set; }
        public string Reascodh { get; set; }
        [Currency(RecordType = RecordType.Hotel)]
        public decimal Hexcprat { get; set; }
    }
}
