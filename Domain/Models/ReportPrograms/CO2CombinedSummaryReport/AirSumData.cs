namespace Domain.Models.ReportPrograms.CO2CombinedSummaryReport
{
    public class AirSumData
    {
        public int Invoices { get; set; } = 0;
        public int Credits { get; set; } = 0;
        public int Trips { get; set; } = 0;
        public decimal Airchg { get; set; } = 0m;
        public decimal Avgairchg { get; set; } = 0m;
        public int Miles { get; set; } = 0;
        public int Airco2 { get; set; } = 0;
        public decimal Avgairco2 { get; set; } = 0m;
        public decimal Costperco2 { get; set; } = 0m;
        public int Altrailco2 { get; set; } = 0;
        public int Altcarco2 { get; set; } = 0;
        public string Carbonlink { get; set; } = string.Empty;
    }
}
