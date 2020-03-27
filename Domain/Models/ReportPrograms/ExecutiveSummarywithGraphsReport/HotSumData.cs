namespace Domain.Models.ReportPrograms.ExecutiveSummarywithGraphsReport
{
    public class HotSumData
    {
        public HotSumData()
        {
            Numhotels = 0m;
            Numnits = 0m;
            Roomchg = 0m;
            Avgrate = 0m;
            Avgnitcost = 0m;
            Avgnumnits = 0m;
        }
        public decimal Numhotels { get; set; }
        public decimal Numnits { get; set; }
        public decimal Roomchg { get; set; }
        public decimal Avgrate { get; set; }
        public decimal Avgnitcost { get; set; }
        public decimal Avgnumnits { get; set; }
    }
}
