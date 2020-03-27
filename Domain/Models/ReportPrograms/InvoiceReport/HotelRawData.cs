using System;

using Domain.Helper;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.InvoiceReport
{
    public class HotelRawData : IRecKey
    {
        [ExchangeDate2]
        public DateTime? Invdate { get; set; } = DateTime.MinValue;
        public int RecKey { get; set; } = 0;
        public string Hotelnam { get; set; } = string.Empty;
        public string Hotcity { get; set; } = string.Empty;
        public string Hotstate { get; set; } = string.Empty;
        public int Nights { get; set; } = 0;
        public int Rooms { get; set; } = 0;
        [ExchangeDate1]
        public DateTime? Datein { get; set; } = DateTime.MinValue;
        [Currency(RecordType = RecordType.Hotel)]
        public decimal Bookrate { get; set; } = 0m;
        public decimal Compamt { get; set; } = 0m;
        [HotelCurrency]
        public string MoneyType { get; set; } = string.Empty;
        public string Guarante { get; set; } = string.Empty;
        public int Hplusmin { get; set; } = 0;
        public string Hotphone { get; set; } = string.Empty;
        public string Confirmno { get; set; } = string.Empty;
        public bool Invbyagcy { get; set; } = false;
        public string HotTranTyp { get; set; } = string.Empty;
        public string Roomtype { get; set; } = string.Empty;
    }
}
