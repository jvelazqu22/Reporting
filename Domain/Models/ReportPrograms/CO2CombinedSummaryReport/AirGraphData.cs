
namespace Domain.Models.ReportPrograms.CO2CombinedSummaryReport
{
    public class AirGraphData : IBarGraph
    {
        public string MthName { get; set; } = string.Empty;
        public int Airco2 { get; set; } = 0;
        public int Altrailco2 { get; set; } = 0;
        public int Altcarco2 { get; set; } = 0;
        public string Bartitle { get; set; } = string.Empty;
        public int MonthNum { get; set; } = 0;
        public int YearNum { get; set; } = 0;
    }
}
