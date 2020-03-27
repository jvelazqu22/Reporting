namespace Domain.Models.ReportPrograms.CarTopBottomAccountsReport
{
    public class FinalData
    {
        public string SourceAbbr { get; set; }
        public string Account { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public decimal Bookrate { get; set; } = 0m;
        public int Rentals { get; set; } = 0;
        public int Days { get; set; } = 0;
        public decimal Carcost { get; set; } = 0m;
        public decimal Bookcnt { get; set; } = 0m;
        public decimal sumbkrate { get; set; }
        public decimal avgbook { get; set; }
        public decimal VolumeBooked { get; set; }
        public string acctname { get; set; } = string.Empty;
    }
}
