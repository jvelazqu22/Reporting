using Domain.Helper;
using Domain.Interfaces;
using System;

namespace Domain.Models.ReportPrograms.TopBottomCostCenter
{
    public class HotelRawData : IRecKey
    {
        public int RecKey { get; set; } = 0;
        public string GrpCol { get; set; } = string.Empty;
        [Currency(RecordType = RecordType.Hotel)]
        public decimal BookRate { get; set; } = 0;
        public int Rooms { get; set; } = 0;
        public int Nights { get; set; } = 0;
        public int HPlusMin { get; set; } = 0;
        [ExchangeDate1]
        public DateTime? HotelExchangeDate { get; set; } = DateTime.MinValue;
        [ExchangeDate2]
        public DateTime? BookDate { get; set; } = DateTime.MinValue;
        [ExchangeDate3]
        public DateTime? InvDate { get; set; } = DateTime.MinValue;
        [HotelCurrency]
        public string HotCurrTyp { get; set; }
    }
}
