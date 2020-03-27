namespace Domain.Models.ReportPrograms.ExecutiveSummarywithGraphsReport
{
    public class TopCityData
    {
        public TopCityData()
        {
            Citypair = string.Empty;
            Segments = 0m;
            Cost = 0m;
        }
        public string Citypair { get; set; }
        public decimal Segments { get; set; }
        public decimal Cost { get; set; }
    }
}
