using System;

using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.UserDefinedReport
{
    public class ChangeLogData : IRecKey
    {
        public ChangeLogData()
        {
            RecKey = 0;
            Agency = string.Empty;
            Recloc = string.Empty;
            Invoice = string.Empty;
            Invdate = DateTime.MinValue;
            Pseudocity = string.Empty;
            Agentid = string.Empty;
            Segnum = 0;
            Segctr = 0;
            Changecode = 0;
            Changstamp = DateTime.MinValue;
            Parsestamp = DateTime.MinValue;
            Changefrom = string.Empty;
            Changeto = string.Empty;
            Changedby = string.Empty;
            Prioritin = string.Empty;
            Pnrcrdtgmt = DateTime.MinValue;
        }

        public int RecKey { get; set; }
        public string Agency { get; set; }
        public string Recloc { get; set; }
        public string Invoice { get; set; }
        public DateTime Invdate { get; set; }
        public string Pseudocity { get; set; }
        public string Agentid { get; set; }
        public byte Segnum { get; set; }
        public int Segctr { get; set; }
        public int Changecode { get; set; }
        public DateTime? Changstamp { get; set; }
        public DateTime? Parsestamp { get; set; }
        public string Changefrom { get; set; }
        public string Changeto { get; set; }
        public string Changedby { get; set; }
        public string Prioritin { get; set; }
        public DateTime? Pnrcrdtgmt { get; set; }
    }
}
