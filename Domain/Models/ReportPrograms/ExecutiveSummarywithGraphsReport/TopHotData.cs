namespace Domain.Models.ReportPrograms.ExecutiveSummarywithGraphsReport
{
    public class TopHotData
    {
        public TopHotData()
        {
            Hotcity = string.Empty;
            Numnits = 0m;
            Hotcost = 0m;
        }
        public string Hotcity { get; set; }
        public decimal Numnits { get; set; }
        public decimal Hotcost { get; set; }
    }
}
