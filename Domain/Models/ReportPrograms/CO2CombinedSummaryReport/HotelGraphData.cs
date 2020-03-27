namespace Domain.Models.ReportPrograms.CO2CombinedSummaryReport
{
    public class HotelGraphData : IBarGraph
    {
        public string MthName { get; set; } = string.Empty;
        public int MonthNum { get; set; }
        public int YearNum { get; set; }
        public int Hotelco2 { get; set; } = 0;
        public string Bartitle { get; set; } = string.Empty;
    }
}
