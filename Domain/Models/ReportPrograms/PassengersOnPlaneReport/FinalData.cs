using System;

namespace Domain.Models.ReportPrograms.PassengersOnPlaneReport
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
            Passlast = string.Empty;
            Passfrst = string.Empty;
            Trantype = string.Empty;
            Pseudocity = string.Empty;
            Origin = string.Empty;
            Destinat = string.Empty;
            Orgdesc = string.Empty;
            Destdesc = string.Empty;
            Airline = string.Empty;
            Alinedesc = string.Empty;
            Depdate = DateTime.MinValue;
            Deptime = string.Empty;
            Arrtime = string.Empty;
            Fltno = string.Empty;
            ClassCode = string.Empty;
            Ticket = string.Empty;
            Agentid = string.Empty;
            Rdepdate = DateTime.MinValue;
            Bookdate = DateTime.MinValue;
            Udidtext = "IGNORE";

            BreakCombo = string.Empty;
            Mode = string.Empty;
        }
        public string Recloc { get; set; }
        public string Acct { get; set; }
        public string Acctdesc { get; set; }
        public string Break1 { get; set; }
        public string Break2 { get; set; }
        public string Break3 { get; set; }
        public string Passlast { get; set; }
        public string Passfrst { get; set; }
        public string Trantype { get; set; }
        public string Pseudocity { get; set; }
        public string Origin { get; set; }
        public string Destinat { get; set; }
        public string Orgdesc { get; set; }
        public string Destdesc { get; set; }
        public string Airline { get; set; }
        public string Alinedesc { get; set; }
        public DateTime Depdate { get; set; }
        public string Deptime { get; set; }
        public string Arrtime { get; set; }
        public string Fltno { get; set; }
        public string ClassCode { get; set; }
        public string Ticket { get; set; }
        public string Agentid { get; set; }
        public DateTime Rdepdate { get; set; }
        public DateTime Bookdate { get; set; }
        public string Udidtext { get; set; }

        //Added by hand
        public int Reckey { get; set; }
        public string Mode { get; set; }
        public string BreakCombo { get; set; }
        public int Count { get; set; }
    }
}
