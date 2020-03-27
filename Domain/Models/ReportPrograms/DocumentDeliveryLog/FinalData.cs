using System;

namespace Domain.Models.ReportPrograms.DocumentDeliveryLog
{
    public class FinalData
    {
        public FinalData()
        {
            Reckey = 0;
            Acct = string.Empty;
            Acctdesc = string.Empty;
            Depdate = DateTime.MinValue;
            Recloc = string.Empty;
            Passlast = string.Empty;
            Passfrst = string.Empty;
            Timezone = string.Empty;
            Bookedtim = string.Empty;
            Rtvlcode = string.Empty;
            Travauthno = 0;
            Sgroupnbr = 0;
            Authstatus = string.Empty;
            Statusdesc = string.Empty;
            Statustime = DateTime.MinValue;
            Authlognbr = 0;
            Statusnbr = 0;
            Docstattim = DateTime.MinValue;
            Docsuccess = false;
            Doctype = string.Empty;
            Docrecips = string.Empty;
            Docsubject = string.Empty;
            Doctext = string.Empty;
            Dochtml = string.Empty;
            Dlvrespons = string.Empty;
        }
        public int Reckey { get; set; }
        public string Acct { get; set; }
        public string Acctdesc { get; set; }
        public DateTime Depdate { get; set; }
        public string Recloc { get; set; }
        public string Passlast { get; set; }
        public string Passfrst { get; set; }
        public string Timezone { get; set; }
        public string Bookedtim { get; set; }
        public string Rtvlcode { get; set; }
        public int Travauthno { get; set; }
        public int Sgroupnbr { get; set; }
        public string Authstatus { get; set; }
        public string Statusdesc { get; set; }
        public DateTime Statustime { get; set; }
        public int Authlognbr { get; set; }
        public int Statusnbr { get; set; }
        public DateTime Docstattim { get; set; }
        public bool Docsuccess { get; set; }
        public string Doctype { get; set; }
        public string Docrecips { get; set; }
        public string Docsubject { get; set; }
        public string Doctext { get; set; }
        public string Dochtml { get; set; }
        public string Dlvrespons { get; set; }

    }
}
