using Domain.Interfaces;
using System;

namespace Domain.Models.ReportPrograms.CarFareSavings
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
        public DateTime Rentdate { get; set; } = DateTime.MinValue;
        public string Company { get; set; } = string.Empty;
        public string Autocity { get; set; } = string.Empty;
        public string Autostat { get; set; } = string.Empty;
        public decimal Days { get; set; } = 0m;
        public int Cplusmin { get; set; } = 0;
        public string Cartype { get; set; } = string.Empty;
        public decimal Carstdrate { get; set; } = 0m;
        public decimal Aexcprat { get; set; } = 0m;
        public decimal Abookrat { get; set; } = 0m;
        public string Carsvgcode { get; set; } = string.Empty;
        public decimal Savings { get; set; } = 0m;
        public string Reascode { get; set; } = string.Empty;
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
