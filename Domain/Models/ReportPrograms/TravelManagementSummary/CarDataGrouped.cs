namespace Domain.Models.ReportPrograms.TravelManagementSummary
{
    public class CarDataGrouped : IGroupedByMonth
    {
        public int MonthNum { get; set; }
        public int Rents { get; set; }
        public int Days { get; set; }
        public decimal CarCost { get; set; }
        public decimal CarCo2 { get; set; }
    }
}
