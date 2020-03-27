using System;

namespace Domain.Models.ReportPrograms.TripChangesAir
{
    public class FinalData
    {
        public FinalData()
        {
            Rectype = string.Empty;
            Reckey = 0;
            Acct = string.Empty;
            Acctdesc = string.Empty;
            Passlast = string.Empty;
            Passfrst = string.Empty;
            Mtggrpnbr = string.Empty;
            Ticket = string.Empty;
            Recloc = string.Empty;
            Bookdate = DateTime.MinValue;
            Depdate = DateTime.MinValue;
            Airchg = 0m;
            Changstamp = DateTime.MinValue;
            Changedesc = string.Empty;
            Origin = string.Empty;
            Orgdesc = string.Empty;
            Destinat = string.Empty;
            Destdesc = string.Empty;
            Connect = string.Empty;
            Seqno = 0m;
            Airline = string.Empty;
            Rdepdate = DateTime.MinValue;
            Fltno = string.Empty;
            Deptime = string.Empty;
            Arrtime = string.Empty;
            Segnum = 0;
        }

        public string Rectype { get; set; }
        public int Reckey { get; set; }
        public string Acct { get; set; }
        public string Acctdesc { get; set; }
        public string Passlast { get; set; }
        public string Passfrst { get; set; }
        public string Mtggrpnbr { get; set; }
        public string Ticket { get; set; }
        public string Recloc { get; set; }
        public DateTime Bookdate { get; set; }
        public DateTime Depdate { get; set; }
        public decimal Airchg { get; set; }
        public DateTime Changstamp { get; set; }
        public string Changedesc { get; set; }
        public string Origin { get; set; }
        public string Orgdesc { get; set; }
        public string Destinat { get; set; }
        public string Destdesc { get; set; }
        public string Connect { get; set; }
        public decimal Seqno { get; set; }
        public string Airline { get; set; }
        public DateTime Rdepdate { get; set; }
        public string Fltno { get; set; }
        public string Deptime { get; set; }
        public string Arrtime { get; set; }
        public int Segnum { get; set; }
    }
}
