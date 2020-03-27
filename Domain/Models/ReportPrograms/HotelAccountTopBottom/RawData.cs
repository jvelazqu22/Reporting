using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.HotelAccountTopBottom
{
    public class RawData : IRecKey
    {
        public int RecKey { get; set; }
        public string SourceAbbr { get; set; }
        public string Acct { get; set; }
        public string GroupAccount { get; set; } = string.Empty;
        [Currency(RecordType = RecordType.Hotel)]
        public decimal BookRate { get; set; }
        public int Stays { get; set; } //TODO: VERIFY TYPE: sum(hplusmin) as Stays
        public int nights { get; set; } //TODO: VERIFY TYPE: sum(nights*rooms*hplusmin) as nights
        public int rooms { get; set; }
        [Currency(RecordType = RecordType.Hotel)]
        public decimal hotelcost { get; set; } //TODO: VERIFY TYPE: sum(bookrate*nights*rooms) as hotelcost
        [Currency(RecordType = RecordType.Hotel)]
        public decimal sumbkrate { get; set; } //TODO: VERIFY TYPE: sum(bookrate) as sumbkrate
        public short Hplusmin { get; set; }
        [ExchangeDate3]
        public DateTime? BookDate { get; set; }
        [HotelCurrency]
        public string HotCurrTyp { get; set; }

    }
}
