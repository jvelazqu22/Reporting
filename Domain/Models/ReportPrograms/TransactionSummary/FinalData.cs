using Domain.Helper;

namespace Domain.Models.ReportPrograms.TransactionSummary
{
    public class FinalData
    {
        [Exportable]
        public string Acct { get; set; } = string.Empty;

        [Exportable]
        public string AcctDesc { get; set; } = string.Empty;

        [Exportable]
        public int AuthCount { get; set; } = 0;

        [Exportable]
        public int BothCount { get; set; } = 0;

        [Exportable]
        public int ChgMgmtCnt { get; set; } = 0;

        [Exportable]
        public int HistCount { get; set; } = 0;

        public int Month { get; set; } = 0;

        [Exportable]
        public string MthName { get; set; } = string.Empty;

        [Exportable]
        public int PcmCount { get; set; } = 0;

        [Exportable]
        public int PrevCount { get; set; } = 0;

        [Exportable]
        public string SourceAbbr { get; set; } = string.Empty;

        [Exportable]
        public string SourceDesc { get; set; } = string.Empty;

        [Exportable]
        public int TrackerCnt { get; set; } = 0;

        [Exportable]
        public int Year { get; set; } = 0;

        [Exportable]
        public int YearMth { get; set; } = 0;
    }
}
