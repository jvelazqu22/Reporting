namespace Domain.Models.ReportPrograms.TravelManagementSummary
{
    public class HotelDataGrouped : IGroupedByMonth
    {
        public int MonthNum { get; set; }
        public int Stays { get; set; }
        public int Nights { get; set; }
        public int Rooms { get; set; }
        public decimal HotelCost { get; set; }
        public decimal HotelCo2 { get; set; }
    }
}
