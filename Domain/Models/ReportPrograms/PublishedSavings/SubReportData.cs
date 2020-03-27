namespace Domain.Models.ReportPrograms.PublishedSavings
{
    public class SubReportData
    {
        public SubReportData()
        {
            Svngdesc = string.Empty;
            NumRecs = 0;
            Savings = 0;
        }
        public string Svngdesc { get; set; }

        public int NumRecs { get; set; }

        public decimal Savings { get; set; }
    }
}
