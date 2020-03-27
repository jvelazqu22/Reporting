using System;
using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.HotelActivity
{
    public class RawData : IRecKey
    {
        public int RecKey { get; set; } = 0;
        public string Acct { get; set; } = string.Empty;
        public string Break1 { get; set; } = string.Empty;
        public string Break2 { get; set; } = string.Empty;
        public string Break3 { get; set; } = string.Empty;
        public string Recloc { get; set; } = string.Empty;
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;
        public string Hotcity { get; set; } = string.Empty;
        public string Hotstate { get; set; } = string.Empty;
        public string Hotcityst { get; set; } = string.Empty;
        [ExchangeDate1]
        public DateTime CheckOut { get { return Datein.GetValueOrDefault().AddDays(Nights); } }
        public DateTime? Datein { get; set; } = DateTime.Now;
        public string Hotelnam { get; set; } = string.Empty;
        public string Chaincod { get; set; } = string.Empty;
        public string Roomtype { get; set; } = string.Empty;
        public int Nights { get; set; } = 0;
        public int Rooms { get; set; } = 0;

        [Currency(RecordType = RecordType.Hotel)]
        public decimal Bookrate { get; set; } = 0m;
        public int Hplusmin { get; set; } = 0;
        public string Origin { get; set; } = string.Empty;
        public string Destinat { get; set; } = string.Empty;
        public string Airline { get; set; } = string.Empty;
        public string Mode { get; set; } = string.Empty;
        public string Connect { get; set; } = string.Empty;

        public DateTime? Rdepdate { get; set; } = DateTime.Now;
        public string Fltno { get; set; } = string.Empty;
        public string Deptime { get; set; } = string.Empty;

        public DateTime? Rarrdate { get; set; } = DateTime.Now;
        public string Arrtime { get; set; } = string.Empty;
        public string ClassCode { get; set; } = string.Empty;
        public string Classcat { get; set; } = string.Empty;
        public int Seqno { get; set; } = 0;
        public int Miles { get; set; }
        public decimal Actfare { get; set; } = 0m;
        public decimal Miscamt { get; set; } = 0m;
        public string Farebase { get; set; } = string.Empty;
        public string DitCode { get; set; } = string.Empty;
        public string Seat { get; set; } = string.Empty;
        public string Stops { get; set; } = string.Empty;
        public string Segstatus { get; set; } = string.Empty;
        public string Tktdesig { get; set; } = string.Empty;
        public int Rplusmin { get; set; }
        public string OrigOrigin { get; set; } = string.Empty;
        public string OrigDest { get; set; } = string.Empty;
        public string OrigCarr { get; set; } = string.Empty;
        [ExchangeDate3]
        public DateTime? BookDate { get; set; }
        [ExchangeDate2]
        public DateTime? InvDate { get; set; }

        [HotelCurrency]
        public string HotCurrTyp { get; set; }
    }
}
