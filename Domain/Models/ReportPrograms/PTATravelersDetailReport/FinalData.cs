﻿using System;

namespace Domain.Models.ReportPrograms.PTATravelersDetailReport
{
    public class FinalData
    {
        public int Reckey { get; set; } = 0;
        public int Rownum { get; set; } = 0;
        public string Acct { get; set; } = string.Empty;
        public string Acctname { get; set; } = string.Empty;
        public string Break1 { get; set; } = string.Empty;
        public string Break2 { get; set; } = string.Empty;
        public string Break3 { get; set; } = string.Empty;
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;
        public string Recloc { get; set; } = string.Empty;
        public string Travreason { get; set; } = string.Empty;
        public string Cliauthnbr { get; set; } = string.Empty;
        public DateTime Statusdate { get; set; } = DateTime.MinValue;
        public string Statustime { get; set; } = string.Empty;
        public string Statusdesc { get; set; } = string.Empty;
        public int Travauthno { get; set; } = 0;
        public string Oopreason1 { get; set; } = string.Empty;
        public string Oopreason2 { get; set; } = string.Empty;
        public string Oopreason3 { get; set; } = string.Empty;
        public string Oopreason4 { get; set; } = string.Empty;
        public string Oopreason5 { get; set; } = string.Empty;
        public string Authemail1 { get; set; } = string.Empty;
        public string Authemail2 { get; set; } = string.Empty;
        public string Authemail3 { get; set; } = string.Empty;
        public string Authemail4 { get; set; } = string.Empty;
        public string Authemail5 { get; set; } = string.Empty;
        public string Authorizr1 { get; set; } = string.Empty;
        public string Authorizr2 { get; set; } = string.Empty;
        public string Authorizr3 { get; set; } = string.Empty;
        public string Authorizr4 { get; set; } = string.Empty;
        public string Authorizr5 { get; set; } = string.Empty;
        public string Authstat1 { get; set; } = string.Empty;
        public string Authstat2 { get; set; } = string.Empty;
        public string Authstat3 { get; set; } = string.Empty;
        public string Authstat4 { get; set; } = string.Empty;
        public string Authstat5 { get; set; } = string.Empty;
        public string Apvreason1 { get; set; } = string.Empty;
        public string Apvreason2 { get; set; } = string.Empty;
        public string Apvreason3 { get; set; } = string.Empty;
        public string Apvreason4 { get; set; } = string.Empty;
        public string Apvreason5 { get; set; } = string.Empty;
        public decimal Tottripchg { get; set; } = 0m;
        public decimal Airchg { get; set; } = 0m;
        public bool Exchange { get; set; } = false;
        public decimal Penaltyamt { get; set; } = 0m;
        public decimal Airfarebkd { get; set; } = 0m;
        public decimal Tktorgamt { get; set; } = 0m;
        public decimal Addcollamt { get; set; } = 0m;
        public decimal Airlowfare { get; set; } = 0m;
        public decimal Airlostsvg { get; set; } = 0m;
        public string Airexcreas { get; set; } = string.Empty;
        public string Airline { get; set; } = string.Empty;
        public DateTime Rdepdate { get; set; } = DateTime.MinValue;
        public string Origin { get; set; } = string.Empty;
        public string Orgdesc { get; set; } = string.Empty;
        public string Destinat { get; set; } = string.Empty;
        public string Destdesc { get; set; } = string.Empty;
        public string Carcompany { get; set; } = string.Empty;
        public DateTime Rentdate { get; set; } = DateTime.MinValue;
        public int Days { get; set; } = 0;
        public decimal Carcost { get; set; } = 0m;
        public decimal Carlowrate { get; set; } = 0m;
        public decimal Carlostsvg { get; set; } = 0m;
        public string Carexcreas { get; set; } = string.Empty;
        public string Hotelnam { get; set; } = string.Empty;
        public string Hotcity { get; set; } = string.Empty;
        public string Metro { get; set; } = string.Empty;
        public DateTime Datein { get; set; } = DateTime.MinValue;
        public int Rooms { get; set; } = 0;
        public int Nights { get; set; } = 0;
        public decimal Hotbookrat { get; set; } = 0m;
        public decimal Hotelcost { get; set; } = 0m;
        public decimal Hotlowrate { get; set; } = 0m;
        public decimal Hotlostsvg { get; set; } = 0m;
        public string Hotexcreas { get; set; } = string.Empty;
        public bool Legdata { get; set; } = false;
        public bool Cardata { get; set; } = false;
        public bool Hoteldata { get; set; } = false;
        public string Authcomm { get; set; } = string.Empty;
        public string Lostsvgmsg { get; set; } = string.Empty;
        public DateTime Bookedgmt { get; set; } = DateTime.MinValue;
        public string Class { get; set; } = string.Empty;
    }
}