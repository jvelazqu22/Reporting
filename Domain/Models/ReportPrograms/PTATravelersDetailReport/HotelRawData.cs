using System;
using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.PTATravelersDetailReport
{
    public class HotelRawData : IRecKey
    {
        [ExchangeDate2]
        public DateTime? BookDate { get; set; } = DateTime.MinValue;
        [ExchangeDate3]
        public DateTime? InvDate { get; set; } = DateTime.MinValue;
        [HotelCurrency]
        public string HotCurrTyp { get; set; } = string.Empty;
        [ExchangeDate1]
        public DateTime? HotelExchangeDate { get; set; } = DateTime.MinValue;
        public string Acct { get; set; } = string.Empty;
        public string Break1 { get; set; } = string.Empty;
        public string Break2 { get; set; } = string.Empty;
        public string Break3 { get; set; } = string.Empty;
        public string Recloc { get; set; } = string.Empty;
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;
        public int RecKey { get; set; } = 0;
        public string HotelNam { get; set; } = string.Empty;
        public string HotCity { get; set; } = string.Empty;
        public string Metro { get; set; } = string.Empty;
        public DateTime? DateIn { get; set; } = DateTime.MinValue;
        public int Rooms { get; set; } = 0;
        public int Nights { get; set; } = 0;
        [Currency(RecordType = RecordType.Hotel)]
        public decimal BookRate { get; set; } = 0m;
        [Currency(RecordType = RecordType.Hotel)]
        public decimal HExcpRat { get; set; } = 0m;
        public string ReasCodH { get; set; } = string.Empty;
        public int TravAuthNo { get; set; } = 0;
    }
}
