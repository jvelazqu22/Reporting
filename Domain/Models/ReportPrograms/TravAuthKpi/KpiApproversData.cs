namespace Domain.Models.ReportPrograms.TravAuthKpi
{
    public class KpiApproversData
    {
        public KpiApproversData()
        {
            Approver = string.Empty;
            Numtrips = 0;
            Statarecs = 0m;
            Statprecs = 0m;
            Statdrecs = 0m;
            Staterecs = 0m;
        }
        public string Approver { get; set; }
        public int Numtrips { get; set; }
        public decimal Statarecs { get; set; }
        public decimal Statprecs { get; set; }
        public decimal Statdrecs { get; set; }
        public decimal Staterecs { get; set; }
    }
}
