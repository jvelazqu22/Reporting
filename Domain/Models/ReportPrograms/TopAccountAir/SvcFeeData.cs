using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.TopAccountAir
{
    public class SvcFeeData : IRecKey
    {
        public SvcFeeData()
        {
            RecKey = 0;
            SourceAbbr = string.Empty;
            Acct = string.Empty;
            SvcAmt = 0m;
        }

        public int RecKey { get; set; }
        public string SourceAbbr { get; set; }
        public string Acct { get; set; }
        public decimal SvcAmt { get; set; }
    }
}
