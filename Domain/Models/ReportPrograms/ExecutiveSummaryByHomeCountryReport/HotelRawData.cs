using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.ExecutiveSummaryByHomeCountryReport
{
    public class HotelRawData : IRecKey
    {
        public string HomeCtry { get; set; } = string.Empty;
        [HotelCurrency]
        public string HotCurrTyp { get; set; }
        [ExchangeDate3]
        public DateTime? BookDate { get; set; }
        [ExchangeDate2]
        public DateTime? InvDate { get; set; }
        public string SourceAbbr { get; set; } = string.Empty;
        public int HPlusMin { get; set; } = 0;
        public int Nights { get; set; } = 0;
        public int Rooms { get; set; } = 0;
        [Currency(RecordType = RecordType.Hotel)]
        public decimal BookRate { get; set; } = 0m;
        public DateTime? DateIn { get; set; } = DateTime.Now;
        [ExchangeDate1]
        public DateTime CheckoutDate { get { return DateIn.GetValueOrDefault().AddDays(Nights); } }

        public int RecKey { get; set; }
    }
}
