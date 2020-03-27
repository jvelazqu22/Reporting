namespace Domain.Models.ReportPrograms.CO2CombinedSummaryReport
{
    public class TopCarrierData
    {
        public string Airline { get; set; } = string.Empty;
        public string AirlineDes { get; set; } = string.Empty;
        public int Segs { get; set; } = 0;
        public int Miles { get; set; } = 0;
        public int Airco2 { get; set; } = 0;
    }
}
