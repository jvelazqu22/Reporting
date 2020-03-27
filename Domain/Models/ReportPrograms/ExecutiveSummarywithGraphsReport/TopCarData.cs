namespace Domain.Models.ReportPrograms.ExecutiveSummarywithGraphsReport
{
    public class TopCarData
    {
        public TopCarData()
        {
            Carcity = string.Empty;
            Numdays = 0m;
            Carcost = 0m;
        }
        public string Carcity { get; set; }
        public decimal Numdays { get; set; }
        public decimal Carcost { get; set; }
    }
}
