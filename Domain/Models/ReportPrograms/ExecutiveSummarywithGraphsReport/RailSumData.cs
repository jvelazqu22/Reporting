namespace Domain.Models.ReportPrograms.ExecutiveSummarywithGraphsReport
{
    public class RailSumData
    {
        public RailSumData()
        {
            Invoices = 0;
            Credits = 0;
            Trips = 0;
            RailChg = 0;
        }

        public int Invoices { get; set; }
        public int Credits { get; set; }
        public int Trips { get; set; }
        public decimal RailChg { get; set; }
    }
}
