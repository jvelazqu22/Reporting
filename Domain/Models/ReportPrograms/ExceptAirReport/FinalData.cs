using System;

namespace Domain.Models.ReportPrograms.ExceptAirReport
{
    public class FinalData
    {
        public FinalData()
        {
            Reckey = 0;
            Recloc = string.Empty;
            Invoice = string.Empty;
            Ticket = string.Empty;
            Acct = string.Empty;
            Acctdesc = string.Empty;
            Break1 = string.Empty;
            Break2 = string.Empty;
            Break3 = string.Empty;
            Passlast = string.Empty;
            Passfrst = string.Empty;
            Reascode = string.Empty;
            Reasdesc = string.Empty;
            Airchg = 0m;
            Offrdchg = 0m;
            Depdate = DateTime.MinValue;
            Origin = string.Empty;
            Orgdesc = string.Empty;
            Destinat = string.Empty;
            Destdesc = string.Empty;
            Connect = string.Empty;
            Rdepdate = DateTime.MinValue;
            Airline = string.Empty;
            Fltno = string.Empty;
            Carrdesc = string.Empty;
            Lostamt = 0m;
            Class = string.Empty;
            Seqno = 0;
            Cryspgbrk = string.Empty;
        }

        public int Reckey { get; set; }
        public string Recloc { get; set; }
        public string Invoice { get; set; }
        public string Ticket { get; set; }
        public string Acct { get; set; }
        public string Acctdesc { get; set; }
        public string Break1 { get; set; }
        public string Break2 { get; set; }
        public string Break3 { get; set; }
        public string Passlast { get; set; }
        public string Passfrst { get; set; }
        public string Reascode { get; set; }
        public string Reasdesc { get; set; }
        public decimal Airchg { get; set; }
        public decimal Offrdchg { get; set; }
        public DateTime Depdate { get; set; }
        public string Origin { get; set; }
        public string Orgdesc { get; set; }
        public string Destinat { get; set; }
        public string Destdesc { get; set; }
        public string Connect { get; set; }
        public DateTime Rdepdate { get; set; }
        public string Airline { get; set; }
        public string Fltno { get; set; }
        public string Carrdesc { get; set; }
        public decimal Lostamt { get; set; }
        public string Class { get; set; }
        public int Seqno { get; set; }
        public string Cryspgbrk { get; set; }
    }
}
