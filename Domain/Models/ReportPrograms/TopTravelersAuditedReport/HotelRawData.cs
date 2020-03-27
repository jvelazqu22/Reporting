using System;
using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.TopTravelersAuditedReport
{
    public class HotelRawData : IRecKey
    {
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;
        public int RecKey { get; set; } = 0;
        public int Nights { get; set; } = 0;
        public int Rooms { get; set; } = 0;
        [Currency(RecordType = RecordType.Hotel)]
        public decimal Bookrate { get; set; } = 0m;
        public int TravAuthNo { get; set; } = 0;

        [ExchangeDate1]
        public DateTime CheckOut { get { return Datein.GetValueOrDefault().AddDays(Nights); } }
        public DateTime? Datein { get; set; }
        [ExchangeDate3]
        public DateTime? BookDate { get; set; }
        [ExchangeDate2]
        public DateTime? InvDate { get; set; }

        [HotelCurrency]
        public string HotCurrTyp { get; set; }

    }
}
