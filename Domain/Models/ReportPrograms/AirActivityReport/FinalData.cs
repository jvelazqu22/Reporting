using System;

namespace Domain.Models.ReportPrograms.AirActivityReport
{
    public class FinalData
    {
        public string Recloc { get; set; } = string.Empty;
        public int RecKey { get; set; }
        public string Invoice { get; set; } = string.Empty;
        public string Ticket { get; set; } = string.Empty;
        public string Acct { get; set; } = string.Empty;
        public string Break1 { get; set; } = string.Empty;
        public string Break2 { get; set; } = string.Empty;
        public string Break3 { get; set; } = string.Empty;
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;
        public decimal Airchg { get; set; }
        public decimal Offrdchg { get; set; }
        public decimal Svcfee { get; set; }
        public string SfTranType { get; set; } = string.Empty;
        public DateTime Depdate { get; set; } = DateTime.MinValue;
        public DateTime Invdate { get; set; }
        public DateTime Bookdate { get; set; }
        public string Cardnum { get; set; } = string.Empty;
        public string Pseudocity { get; set; } = string.Empty;
        public string Trantype { get; set; } = string.Empty;
        public string Origticket { get; set; } = string.Empty;
        public int Plusmin { get; set; }
        public string Origin { get; set; } = string.Empty;
        public string Destinat { get; set; } = string.Empty;
        public string Airline { get; set; } = string.Empty;
        public string Connect { get; set; } = string.Empty;
        public DateTime RDepDate { get; set; } = DateTime.MinValue;
        public string fltno { get; set; } = string.Empty;
        public string ClassCode { get; set; } = string.Empty;
        public int SeqNo { get; set; }
        public int Miles { get; set; }
        public DateTime Sortdate { get; set; } = DateTime.MinValue;
        public string AcctDesc { get; set; } = string.Empty;
        public string OrgDesc { get; set; } = string.Empty;
        public string DestDesc { get; set; } = string.Empty;
        public decimal AirCo2 { get; set; }
        public decimal AltCarCo2 { get; set; }
        public decimal AltRailCo2 { get; set; }
        public string HaulType { get; set; } = string.Empty;
        public string Tktdesig { get; set; } = string.Empty;
        public bool Exchange { get; set; }
        public int Udidnbr1 { get; set; }
        public string Udidtext1 { get; set; } = string.Empty;
        public int Udidnbr2 { get; set; }
        public string Udidtext2 { get; set; } = string.Empty;
        public int Udidnbr3 { get; set; }
        public string Udidtext3 { get; set; } = string.Empty;
        public int Udidnbr4 { get; set; }
        public string Udidtext4 { get; set; } = string.Empty;
        public int Udidnbr5 { get; set; }
        public string Udidtext5 { get; set; } = string.Empty;
        public int Udidnbr6 { get; set; }
        public string Udidtext6 { get; set; } = string.Empty;
        public int Udidnbr7 { get; set; }
        public string Udidtext7 { get; set; } = string.Empty;
        public int Udidnbr8 { get; set; }
        public string Udidtext8 { get; set; } = string.Empty;
        public int Udidnbr9 { get; set; }
        public string Udidtext9 { get; set; } = string.Empty;
        public int Udidnbr10 { get; set; }
        public string Udidtext10 { get; set; } = string.Empty;
    }
}
