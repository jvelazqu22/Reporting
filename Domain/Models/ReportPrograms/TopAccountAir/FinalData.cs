
namespace Domain.Models.ReportPrograms.TopAccountAir
{
    public class FinalData
    {
        public FinalData()
        {
            Account = string.Empty;
            SourceAbbr = string.Empty;
            Amt = 0m;
            Trips = 0;
            Svcfee = 0m;
            Acommisn = 0m;
            OffrdChg = 0m;
            LowFare = 0m;
            LostAmt = 0m;
            AcctName = string.Empty;
        }

        public decimal LostAmt { get; set; }

        public string Account { get; set; }
        public string SourceAbbr { get; set; }
        public decimal Amt { get; set; }
        public int Trips { get; set; }
        public decimal Svcfee { get; set; }
        public decimal Acommisn { get; set; }
        public decimal OffrdChg { get; set; }
        public decimal LowFare { get; set; }
        public string AcctName { get; set; }
        public decimal AvgCost { get { return Trips == 0 ? 0 : Amt/Trips; } }

    }
}
