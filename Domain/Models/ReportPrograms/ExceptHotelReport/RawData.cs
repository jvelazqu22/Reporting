using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.ExceptHotelReport
{
    public class RawData : IRecKey
    {
        public string Recloc { get; set; }
        public int RecKey { get; set; }
        public string Acct { get; set; }
        public string Break1 { get; set; }
        public string Break2 { get; set; }
        public string Break3 { get; set; }
        public string Passlast { get; set; }
        public string Passfrst { get; set; }
        public string Reascodh { get; set; }
        [ExchangeDate1]
        public DateTime CheckOut { get { return Datein.GetValueOrDefault().AddDays(Nights); } }
        public DateTime? Datein { get; set; }
        public string Hotelnam { get; set; }
        public string Hotcity { get; set; }
        public string Hotstate { get; set; }
        public int Nights { get; set; }
        public int Rooms { get; set; }
        public string Roomtype { get; set; }
        [Currency(RecordType = RecordType.Hotel)]
        public decimal Bookrate { get; set; }
        [Currency(RecordType = RecordType.Hotel)]
        public decimal Hexcprat { get; set; }
        public string Confirmno { get; set; }
        public int Hplusmin { get; set; }
        [ExchangeDate3]
        public DateTime? BookDate { get; set; }
        [ExchangeDate2]
        public DateTime? InvDate { get; set; }
        [HotelCurrency]
        public string HotCurrTyp { get; set; }
    }
}
