namespace Domain.Models.ReportPrograms.PTATravelersDetailReport
{
    public class SummaryTempData
    {
        public decimal TotTripChg { get; set; } = 0m;
        public decimal HotelCost { get; set; } = 0m;
        public decimal CarCost { get; set; } = 0m;
        public decimal AirChg { get; set; } = 0m;
        public int NumTrips { get; set; } = 0;
        public string AuthStatus { get; set; } = string.Empty;
        public int SGroupNbr { get; set; } = 0;
    }
}
