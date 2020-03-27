using System;

namespace Domain.Models.ReportPrograms.AirActivityByUdidReport
{
    public class FinalData
    {
        public FinalData()
        {
            Acct = "";
            AcctDesc = "";
            Break1 = "";
            Break2 = "";
            Break3 = "";
            Reckey = 0;
            SeqNo = 0;
            Udidtext = "";
            Passlast = "";
            Passfrst = "";
            Origin = "";
            Destinat = "";
            Rdepdate = DateTime.MinValue;
            Destdesc = "";
            Connect = "";
            Depdate = DateTime.MinValue;
            Ticket = "";
            Airline = "";
            Fltno = "";
            Class = "";
            Airchg = 0;
            Trantype = "";
            Cryspgbrk = "";
        }

        public string Acct { get; set; }
        public string AcctDesc { get; set; }
        public string Break1 { get; set; }
        public string Break2 { get; set; }
        public string Break3 { get; set; }
        public int Reckey { get; set; }
        public int SeqNo { get; set; }
        public string Udidtext { get; set; }
        public string Passlast { get; set; }
        public string Passfrst { get; set; }
        public string Origin { get; set; }
        public string Orgdesc { get; set; }
        public string Destinat { get; set; }
        public DateTime Rdepdate { get; set; }
        public string Destdesc { get; set; }
        public string Connect { get; set; }
        public DateTime Depdate { get; set; }
        public string Ticket { get; set; }
        public string Airline { get; set; }
        public string Fltno { get; set; }
        public string Class { get; set; }
        public decimal Airchg { get; set; }
        public string Trantype { get; set; }
        public string Cryspgbrk { get; set; }
    }
}
