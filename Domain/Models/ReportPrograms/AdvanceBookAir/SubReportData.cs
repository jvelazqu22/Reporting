namespace Domain.Models.ReportPrograms.AdvanceBookAir
{
    public class SubReportData
    {
        public SubReportData()
        {
            Bookcat = string.Empty;
            Category = string.Empty;
            Shortdesc = string.Empty;
            Trips = 0;
            Totairchg = 0m;
            Avgairchg = 0m;
        }
        public string Bookcat { get; set; }
        public string Category { get; set; }
        public string Shortdesc { get; set; }
        public int Trips { get; set; }
        public decimal Totairchg { get; set; }
        public decimal Avgairchg { get; set; }
    }
}
