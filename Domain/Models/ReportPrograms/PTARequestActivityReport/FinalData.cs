using System;

namespace Domain.Models.ReportPrograms.PTARequestActivityReport
{
    public class FinalData
    {
        public int Reckey { get; set; } = 0;
        public string Acct { get; set; } = string.Empty;
        public string Acctdesc { get; set; } = string.Empty;
        public string Break1 { get; set; } = string.Empty;
        public string Break2 { get; set; } = string.Empty;
        public string Break3 { get; set; } = string.Empty;
        public string Depdate { get; set; } = string.Empty;
        public DateTime DepartureDate { get; set; } = DateTime.MinValue;
        public DateTime Bookdate { get; set; } = DateTime.MinValue;
        public DateTime Statustime { get; set; } = DateTime.MinValue;
        public string Recloc { get; set; } = string.Empty;
        public decimal Daysadvanc { get; set; } = 0m;
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;
        public string Rtvlcode { get; set; } = string.Empty;
        public string Tvlreasdes { get; set; } = string.Empty;
        public string Outpolcods { get; set; } = string.Empty;
        public string Oopreasdes { get; set; } = string.Empty;
        public int Authrzrnbr { get; set; } = 0;
        public string Authorizer { get; set; } = string.Empty;
        public string Authstatus { get; set; } = string.Empty;
        public string Statusdesc { get; set; } = string.Empty;
        public string Detlstatus { get; set; } = string.Empty;
        public string Auth1email { get; set; } = string.Empty;
        public string Detstatdes { get; set; } = string.Empty;
        public DateTime Detstattim { get; set; } = DateTime.MinValue;
        public string Apvreason { get; set; } = string.Empty;
        public int Apsequence { get; set; } = 0;
        public string Deptime { get; set; } = string.Empty;
        public int Travauthno { get; set; } = 0;
        public int Sgroupnbr { get; set; } = 0;
        public decimal Airchg { get; set; } = 0m;
        public decimal Offrdchg { get; set; } = 0m;
        public decimal Lostsvngs { get; set; } = 0m;
        public string Authpglink { get; set; } = string.Empty;
        public string CliAuthNbr { get; set; } = string.Empty;
        public string SortCol { get; set; } = string.Empty;
    }
}
