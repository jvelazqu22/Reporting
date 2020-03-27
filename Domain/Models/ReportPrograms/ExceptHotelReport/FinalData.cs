using System;

namespace Domain.Models.ReportPrograms.ExceptHotelReport
{
    public class FinalData
    {
        public int Reckey { get; set; }
        public string Recloc { get; set; }
        public string Acct { get; set; }
        public string Acctdesc { get; set; }
        public string Break1 { get; set; }
        public string Break2 { get; set; }
        public string Break3 { get; set; }
        public string Passlast { get; set; }
        public string Passfrst { get; set; }
        public string Reascodh { get; set; }
        public string Reasdesc { get; set; }
        public DateTime Datein { get; set; }
        public string Hotelnam { get; set; }
        public string Hotcity { get; set; }
        public string Hotstate { get; set; }
        public int Nights { get; set; }
        public int Rooms { get; set; }
        public string Roomtype { get; set; }
        public string Typedesc { get; set; }
        public int Hplusmin { get; set; }
        public decimal Bookrate { get; set; }
        public string Confirmno { get; set; }
        public decimal Hexcprat { get; set; }
    }
}
