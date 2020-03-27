namespace Domain.Models.ReportPrograms.TopBottomHotelsReport
{
    public class GroupedByHotel
    {
        public string Category { get; set; } = string.Empty;
        public int Stays { get; set; }
        public int Nights { get; set; }
        public int NzNights { get; set; }
        public decimal HotelCost { get; set; }
        public decimal BookRate { get; set; }
        public int BookCnt { get; set; }
        public string Cat2 { get; set; } = string.Empty;
    }
}
