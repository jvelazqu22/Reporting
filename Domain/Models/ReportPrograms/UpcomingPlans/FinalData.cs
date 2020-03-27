using System;

namespace Domain.Models.ReportPrograms.UpcomingPlans
{
    public class FinalData
    {
        public FinalData()
        {
            Recloc = string.Empty;
            Acct = string.Empty;
            Acctdesc = string.Empty;
            Break1 = string.Empty;
            Break2 = string.Empty;
            Break3 = string.Empty;
            Origin = string.Empty;
            Orgdesc = string.Empty;
            Destinat = string.Empty;
            Destdesc = string.Empty;
            Airline = string.Empty;
            Alinedesc = string.Empty;
            Passlast = string.Empty;
            Passfrst = string.Empty;
            ClassCode = string.Empty;
            Deptime = string.Empty;
            Fltno = string.Empty;
            Rdepdate = DateTime.MinValue;
        }
        public string Recloc { get; set; }
        public int Seqno { get; set; }
        public string Acct { get; set; }
        public string Acctdesc { get; set; }
        public string Break1 { get; set; }
        public string Break2 { get; set; }
        public string Break3 { get; set; }
        public decimal Weeknum { get; set; }
        public string Origin { get; set; }
        public string Orgdesc { get; set; }
        public string Destinat { get; set; }
        public string Destdesc { get; set; }
        public string Airline { get; set; }
        public string Alinedesc { get; set; }
        public decimal Actfare { get; set; }
        public DateTime Rdepdate { get; set; }
        public string Passlast { get; set; }
        public string Passfrst { get; set; }
        public string ClassCode { get; set; }
        public string Class
        {
            get { return ClassCode; }
        }
        public string Deptime { get; set; }
        public string Fltno { get; set; }
    }
}
