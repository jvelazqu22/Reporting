using System;

namespace Domain.Models.ReportPrograms.HotelActivity
{
    public class FinalData
    {
        public string Recloc { get; set; } = string.Empty;
        public int Hplusmin { get; set; } = 0;
        public string Acct { get; set; } = string.Empty;
        public string Acctdesc { get; set; } = string.Empty;
        public string Break1 { get; set; } = string.Empty;
        public string Break2 { get; set; } = string.Empty;
        public string Break3 { get; set; } = string.Empty;
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;
        public string Hotcity { get; set; } = string.Empty;
        public string Hotstate { get; set; } = string.Empty;
        public string Hotcityst { get; set; } = string.Empty;
        public DateTime Datein { get; set; } = DateTime.MinValue;
        public string Hotelnam { get; set; } = string.Empty;
        public string Chaincod { get; set; } = string.Empty;
        public string Roomtype { get; set; } = string.Empty;
        public int Nights { get; set; } = 0;
        public int Rooms { get; set; } = 0;
        public decimal Bookrate { get; set; } = 0m;
        public decimal Weeknum { get; set; } = 0m;
        public string WeekNumTxt { get; set; } = string.Empty;
    }
}
