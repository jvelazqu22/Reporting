namespace Domain.Models.ReportPrograms.TopBottomHotelsReport
{
    public class GroupedRawData
    {
        public string Category { get; set; } = string.Empty;
        public string GroupColumn2 { get; set; } = string.Empty;
        public string GroupColumn3 { get; set; } = string.Empty;
        public int Stays { get; set; } = 0;
        public int Nights { get; set; } = 0;
        public decimal HotelCost { get; set; } = 0;
        public decimal SumBkRate { get; set; } = 0;
        public decimal BookRate { get; set; } = 0;
    }
}
