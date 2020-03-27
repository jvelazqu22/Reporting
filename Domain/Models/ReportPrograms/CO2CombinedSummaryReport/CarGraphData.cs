namespace Domain.Models.ReportPrograms.CO2CombinedSummaryReport
{
    public class CarGraphData : IBarGraph
    {
        public string MthName { get; set; } = string.Empty;
        public int MonthNum { get; set; }
        public int YearNum { get; set; }
        public int Carco2 { get; set; } = 0;
        public string Bartitle { get; set; } = string.Empty;
    }
}
