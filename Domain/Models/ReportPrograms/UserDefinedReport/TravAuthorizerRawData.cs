using System;

using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.UserDefinedReport
{
    public class TravAuthorizerRawData : IRecKey
    {
        public TravAuthorizerRawData()
        {
            RecKey = 0;
            Agency = string.Empty;
            Recloc = string.Empty;
            Invoice = string.Empty;
            Invdate = DateTime.MinValue;
            Pseudocity = string.Empty;
            Agentid = string.Empty;
            Authrzrnbr = string.Empty;
            Authstatus = string.Empty;
            Statustime = string.Empty;
            Auth1Email = string.Empty;
            Auth2Email = string.Empty;
            Apvreason = string.Empty;
        }

        public int RecKey { get; set; }
        public string Agency { get; set; }
        public string Recloc { get; set; }
        public string Invoice { get; set; }
        public DateTime? Invdate { get; set; }
        public string Pseudocity { get; set; }
        public string Agentid { get; set; }
        public string Authrzrnbr { get; set; }
        public string Authstatus { get; set; }
        public string Statustime { get; set; }
        public string Auth1Email { get; set; }
        public string Auth2Email { get; set; }
        public string Apvreason { get; set; }
    }
}
