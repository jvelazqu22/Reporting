namespace Domain.Models.ReportPrograms.ExecutiveSummaryByHomeCountryReport
{
    public class FinalData
    {
        public string RowType { get; set; }
        public string HomeCtry { get; set; } = string.Empty;
        public string RowDesc { get; set; } = string.Empty;
        public int NetTrans { get; set; } = 0;
        public decimal Volume { get; set; } = 0;
        public decimal AvgCost { get; set; } = 0;
        public double AvgDays { get; set; } = 0;
        public double SvngsPct { get; set; } = 0;
        public double LossPct { get; set; } = 0;
        public decimal StandardCharge { get; set; } = 0;
        public int Days { get; set; } = 0;
        public decimal Savings { get; set; } = 0;
        public decimal LostAmt { get; set; } = 0;
    }
}
