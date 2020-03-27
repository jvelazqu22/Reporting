using System;

namespace Domain.Models.ReportPrograms.CCReconReport
{
    public class BreakRecord
    {
        public string Acct { get; set; } = string.Empty;
        public string Break1 { get; set; } = string.Empty;
        public string Break2 { get; set; } = string.Empty;
        public string Break3 { get; set; } = string.Empty;
        public int RecKey { get; set; } 
        public string Invoice { get; set; } = string.Empty;
        public DateTime? Invdate { get; set; } = DateTime.Now;
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;
        public string RecLoc { get; set; } = string.Empty;
    }
}
