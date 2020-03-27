using System;

using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.UserDefinedReport
{
    public class TravAuthRawData : IRecKey
    {
        public TravAuthRawData()
        {
            RecKey = 0;
            Agency = string.Empty;
            Recloc = string.Empty;
            Invoice = string.Empty;
            Invdate = DateTime.MinValue;
            Acct = string.Empty;
            Pseudocity = string.Empty;
            Agentid = string.Empty;
            Travauthno = 0;
            Authstatus = string.Empty;
            Statustime = DateTime.MinValue;
            Trvlremail = string.Empty;
            Tvlrccaddr = string.Empty;
            Rtvlcode = string.Empty;
            Outpolcods = string.Empty;
            Authcomm = string.Empty;
            Bookedgmt = DateTime.MinValue;
            Cliauthnbr = string.Empty;
        }

        public int RecKey { get; set; }
        public string Agency { get; set; }
        public string Recloc { get; set; }
        public string Invoice { get; set; }
        public DateTime? Invdate { get; set; }
        public string Acct { get; set; }
        public string Pseudocity { get; set; }
        public string Agentid { get; set; }
        public int Travauthno { get; set; }
        public string Authstatus { get; set; }
        public DateTime? Statustime { get; set; }
        public string Trvlremail { get; set; }
        public string Tvlrccaddr { get; set; }
        public string Rtvlcode { get; set; }
        public string Outpolcods { get; set; }
        public string Authcomm { get; set; }
        public DateTime? Bookedgmt { get; set; }
        public string Cliauthnbr { get; set; }
    }

}
