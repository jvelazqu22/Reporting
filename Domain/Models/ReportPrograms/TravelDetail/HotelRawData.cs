using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.TravelDetail
{
    public class HotelRawData : ICarbonCalculationsHotel, IRecKey
    {
        [HotelCurrency]
        public string HotCurrTyp { get; set; } = string.Empty;
        [ExchangeDate1]
        public DateTime? InvDate { get; set; }
        public string HotTranTyp { get; set; } = string.Empty;
        public int RecKey { get; set; } = 0;
        [Currency(RecordType = RecordType.Hotel)]
        public decimal BookRate { get; set; } = 0m;
        public string Metro { get; set; } = string.Empty;
        public string HotelNam { get; set; } = string.Empty;
        public string HotCity { get; set; } = string.Empty;
        public string HotState { get; set; } = string.Empty;
        [ExchangeDate2]
        public DateTime? DateIn { get; set; }
        public DateTime? DateOut { get; set; }
        public string RoomType { get; set; } = string.Empty;
        public string Guarante { get; set; } = string.Empty;
        public string ReasCodh { get; set; } = string.Empty;
        public string HotPhone { get; set; } = string.Empty;
        public string ConfirmNo { get; set; } = string.Empty;
        public int HPlusMin { get; set; } = 0;
        public int Nights { get; set; } = 0;
        public int Rooms { get; set; } = 0;
        public decimal HotelCo2 { get; set; }

        public string PassFrst { get; set; } = string.Empty;
        public string PassLast { get; set; } = string.Empty;
        public string RecLoc { get; set; } = string.Empty;
        public string Acct { get; set; } = string.Empty;
        public string Break1 { get; set; } = string.Empty;
        public string Break2 { get; set; } = string.Empty;
        public string Break3 { get; set; } = string.Empty;
        public DateTime? TripStart { get; set; }
        public DateTime? TripEnd { get; set; }
        public DateTime? DepDate { get; set; }
        public DateTime? ArrDate { get; set; }
        public string SourceAbbr { get; set; } = string.Empty;
        public string Udidtext { get; set; }

    }
}
