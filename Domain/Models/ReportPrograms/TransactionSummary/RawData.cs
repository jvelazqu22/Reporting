using Domain.Interfaces;

namespace Domain.Models.TransactionSummary
{
    public class RawData : IRecKey
    {
        public int RecKey { get; set; }
        public string Acct { get; set; } = string.Empty;
        public string Agency { get; set; } = string.Empty;
        public int Catcount { get; set; } = 0;
        public string Category { get; set; } = string.Empty;
        public int Month { get; set; } = 0;
        public string PrelimFinal { get; set; } = string.Empty;
        public int Recordnum { get; set; } = 0;
        public string Source { get; set; } = string.Empty;
        public string SourceAbbr { get; set; } = string.Empty;
        public int Year { get; set; } = 0;
    }
}
