namespace Domain.Models.ReportPrograms.TopBottomHotelsReport
{
    public class FinalData
    {
        public string Category { get; set; } = string.Empty;
        public string Cat2 { get; set; } = string.Empty;
        public string Hotelchain { get; set; } = string.Empty;
        public int Stays { get; set; } = 0;
        public int Nights { get; set; } = 0;
        public int Nznights { get; set; } = 0;
        public decimal Hotelcost { get; set; } = 0m;
        public decimal Bookrate { get; set; } = 0m;
        public int Bookcnt { get; set; } = 0;
        public decimal Avgbook { get; set; } = 0m;
        public int Stays2 { get; set; } = 0;
        public int Nights2 { get; set; } = 0;
        public int Nznights2 { get; set; } = 0;
        public decimal Hotelcost2 { get; set; } = 0m;
        public decimal Bookrate2 { get; set; } = 0m;
        public int Bookcnt2 { get; set; } = 0;
        public decimal Avgbook2 { get; set; } = 0m;
    }
}
