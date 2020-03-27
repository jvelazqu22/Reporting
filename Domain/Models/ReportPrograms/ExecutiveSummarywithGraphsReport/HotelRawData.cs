using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.ExecutiveSummarywithGraphsReport
{
    public class HotelRawData : IRecKey
    {
        public HotelRawData()
        {
            RecKey = 0;
            Chaincod = string.Empty;
            Hplusmin = 0;
            Nights = 0;
            Rooms = 0;
            Bookrate = 0m;
            Reascodh = string.Empty;
            Hexcprat = 0m;
            Hotcity = string.Empty;
            Hotstate = string.Empty;
            UseDate = DateTime.MinValue;
        }
        public int RecKey { get; set; }
        public string Chaincod { get; set; }
        public int Hplusmin { get; set; }
        public int Nights { get; set; }
        public int Rooms { get; set; }
        [Currency(RecordType = RecordType.Hotel)]
        public decimal Bookrate { get; set; }
        public string Reascodh { get; set; }
        public decimal Hexcprat { get; set; }
        public string Hotcity { get; set; }
        public string Hotstate { get; set; }
        public DateTime? UseDate { get; set; }
        public DateTime? DateIn { get; set; }
        [ExchangeDate2]
        public DateTime? BookDate { get; set; }
        [ExchangeDate3]
        public DateTime? InvDate { get; set; }

        [HotelCurrency]
        public string HotCurrTyp { get; set; }
        [ExchangeDate1]
        public DateTime HotelExchangeDate { get { return DateIn.GetValueOrDefault().AddDays(Nights); } }


    }
}
