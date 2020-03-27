namespace Domain.Models.ReportPrograms.TopBottomCars
{
    public class GroupedRawData
    {
        public string Category { get; set; } = string.Empty;
        public string GroupColumn2 { get; set; } = string.Empty;
        public string GroupColumn3 { get; set; } = string.Empty;
        public int Rentals { get; set; } = 0;
        public int Days { get; set; } = 0;
        public decimal CarCost { get; set; } = 0;
        public decimal SumBkRate { get; set; } = 0;
        public decimal ABookRate { get; set; } = 0;
    }
}
