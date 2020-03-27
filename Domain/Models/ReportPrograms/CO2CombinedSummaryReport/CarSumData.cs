namespace Domain.Models.ReportPrograms.CO2CombinedSummaryReport
{
    public class CarSumData
    {
        public int Rentals { get; set; } = 0;
        public int Days { get; set; } = 0;
        public decimal Carcost { get; set; } = 0m;
        public int Carco2 { get; set; } = 0;
        public decimal Avgdays { get; set; } = 0m;
        public decimal Avgdaycost { get; set; } = 0m;
        public decimal Costperco2 { get; set; } = 0m;

    }
}
