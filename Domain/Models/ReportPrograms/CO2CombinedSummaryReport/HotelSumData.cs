namespace Domain.Models.ReportPrograms.CO2CombinedSummaryReport
{
    public class HotelSumData
    {
        public int Bookings { get; set; } = 0;
        public int Nights { get; set; } = 0;
        public decimal Hotelcost { get; set; } = 0m;
        public int Hotelco2 { get; set; } = 0;
        public decimal Avgnites { get; set; } = 0m;
        public decimal Avgntcost { get; set; } = 0m;
        public decimal Costperco2 { get; set; } = 0m;
    }
}
