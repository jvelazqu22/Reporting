using Domain.Interfaces;
using System;

namespace Domain.Models.ReportPrograms.HotelFareSavings
{
    public class FinalData : IUdidData
    {
        public int Reckey { get; set; } = 0;
        public string Homectry { get; set; } = string.Empty;
        public string Acct { get; set; } = string.Empty;
        public string Acctdesc { get; set; } = string.Empty;
        public string Break1 { get; set; } = string.Empty;
        public string Break2 { get; set; } = string.Empty;
        public string Break3 { get; set; } = string.Empty;
        public string Confirmno { get; set; } = string.Empty;
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;
        public DateTime Invdate { get; set; } = DateTime.MinValue;
        public DateTime Datein { get; set; } = DateTime.MinValue;
        public string Hotelnam { get; set; } = string.Empty;
        public string Hotcity { get; set; } = string.Empty;
        public string Hotstate { get; set; } = string.Empty;
        public decimal Nights { get; set; } = 0m;
        public decimal Rooms { get; set; } = 0m;
        public int Hplusmin { get; set; } = 0;
        public string Roomtype { get; set; } = string.Empty;
        public decimal Hotstdrate { get; set; } = 0m;
        public decimal Hexcprat { get; set; } = 0m;
        public decimal Bookrate { get; set; } = 0m;
        public string Hotsvgcode { get; set; } = string.Empty;
        public decimal Savings { get; set; } = 0m;
        public string Reascodh { get; set; } = string.Empty;
        public decimal Lostamt { get; set; } = 0m;
        public decimal Lostpct { get; set; } = 0m;
        public string Origacct { get; set; } = string.Empty;
        public string Sourceabbr { get; set; } = string.Empty;
        public string UdidLbl1 { get; set; } = string.Empty;
        public string UdidText1 { get; set; } = string.Empty;
        public string UdidLbl2 { get; set; } = string.Empty;
        public string UdidText2 { get; set; } = string.Empty;
        public string UdidLbl3 { get; set; } = string.Empty;
        public string UdidText3 { get; set; } = string.Empty;
        public string UdidLbl4 { get; set; } = string.Empty;
        public string UdidText4 { get; set; } = string.Empty;
        public string UdidLbl5 { get; set; } = string.Empty;
        public string UdidText5 { get; set; } = string.Empty;
        public string UdidLbl6 { get; set; } = string.Empty;
        public string UdidText6 { get; set; } = string.Empty;
        public string UdidLbl7 { get; set; } = string.Empty;
        public string UdidText7 { get; set; } = string.Empty;
        public string UdidLbl8 { get; set; } = string.Empty;
        public string UdidText8 { get; set; } = string.Empty;
        public string UdidLbl9 { get; set; } = string.Empty;
        public string UdidText9 { get; set; } = string.Empty;
        public string UdidLbl10 { get; set; } = string.Empty;
        public string UdidText10 { get; set; } = string.Empty;

    }
}
