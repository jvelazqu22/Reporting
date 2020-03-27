using Domain.Helper;
using Domain.Interfaces;
using System;

namespace Domain.Models.ReportPrograms.HotelFareSavings
{
    public class RawData : IRecKey
    {
        [ExchangeDate3]
        public DateTime? BookDate { get; set; } = DateTime.Now;
        [HotelCurrency]
        public string HotCurrTyp { get; set; }
        [ExchangeDate1]
        public DateTime? HotelExchangeDate { get; set; }
        public int RecKey { get; set; } = 0;
        public string Acct { get; set; } = string.Empty;
        public string Break1 { get; set; } = string.Empty;
        public string Break2 { get; set; } = string.Empty;
        public string Break3 { get; set; } = string.Empty;
        public string SourceAbbr { get; set; } = string.Empty;
        public string ConfirmNo { get; set; } = string.Empty;
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;
        [ExchangeDate2]
        public DateTime? Invdate { get; set; } = DateTime.Now;
        public DateTime? DateIn { get; set; } = DateTime.Now;
        public string Hotelnam { get; set; } = string.Empty;
        public string Hotcity { get; set; } = string.Empty;
        public string Hotstate { get; set; } = string.Empty;
        public int Nights { get; set; } = 0;
        public int Rooms { get; set; } = 0;
        public string Roomtype { get; set; } = string.Empty;
        [Currency(RecordType = RecordType.Hotel)]
        public decimal HExcpRat { get; set; } = 0m;
        [Currency(RecordType = RecordType.Hotel)]
        public decimal BookRate { get; set; } = 0m;
        public string Reascodh { get; set; } = string.Empty;
        public int Hplusmin { get; set; } = 0;
        [Currency(RecordType = RecordType.Hotel)]
        public decimal HotStdRate { get; set; } = 0m;
        public string HotSvgCode { get; set; } = string.Empty;
    }
}
