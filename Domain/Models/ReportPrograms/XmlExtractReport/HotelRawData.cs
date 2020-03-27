using Domain.Interfaces;
using System;

namespace Domain.Models.ReportPrograms.XmlExtractReport
{

    public class HotelRawData : IRecKey
    {
        public HotelRawData()
        {
            RecKey = 0;
            Pseudocity = string.Empty;
            Agentid = string.Empty;
            Agency = string.Empty;
            Recloc = string.Empty;
            Chaincod = string.Empty;
            Metro = string.Empty;
            Hotelnam = string.Empty;
            Hotcity = string.Empty;
            Hotstate = string.Empty;
            Nights = 0;
            Rooms = 0;
            Datein = DateTime.MinValue;
            Bookrate = 0m;
            Moneytype = string.Empty;
            Roomtype = string.Empty;
            Hexcprat = 0m;
            Guarante = string.Empty;
            Reascodh = string.Empty;
            Hotphone = string.Empty;
            Confirmno = string.Empty;
            Hotpropid = string.Empty;
            Numguests = 0;
            Compamt = 0m;
            Hottrantyp = string.Empty;
            Hplusmin = 0;
            Hcommissn = 0m;
            Invbyagcy = false;
            Emailaddr = string.Empty;
            Gds = string.Empty;
        }
        public int RecKey { get; set; }
        public string Pseudocity { get; set; }
        public string Agentid { get; set; }
        public string Agency { get; set; }
        public string Recloc { get; set; }
        public string Chaincod { get; set; }
        public string Metro { get; set; }
        public string Hotelnam { get; set; }
        public string Hotcity { get; set; }
        public string Hotstate { get; set; }
        public string HotelAddr1 { get; set; } = string.Empty;
        public string HotelAddr2 { get; set; } = string.Empty;
        public int Nights { get; set; }
        public int Rooms { get; set; }
        public DateTime? Datein { get; set; }
        public decimal Bookrate { get; set; }
        public string Moneytype { get; set; }
        public string Roomtype { get; set; }
        public decimal Hexcprat { get; set; }
        public string Guarante { get; set; }
        public string Reascodh { get; set; }
        public string Hotphone { get; set; }
        public string Confirmno { get; set; }
        public string Hotpropid { get; set; }
        public int Numguests { get; set; }
        public decimal Compamt { get; set; }
        public string Hottrantyp { get; set; }
        public int Hplusmin { get; set; }
        public decimal Hcommissn { get; set; }
        public bool Invbyagcy { get; set; }
        public string Emailaddr { get; set; }
        public string Gds { get; set; }
    }
}
