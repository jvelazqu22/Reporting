namespace Domain.Models.ReportPrograms.HotelAccountTopBottom
{
    public class FinalData
    {
        public string SourceAbbr { get; set; }
        public string Account { get; set; } = string.Empty;
        public string acctname { get; set; }
        public int Stays { get; set; } = 0;
        public int Nights { get; set; } = 0;
        public decimal Hotelcost { get; set; } = 0m;
        public decimal Bookrate { get; set; } = 0m;
        public decimal AveBookRate { get; set; } = 0m;
        public decimal Bookcnt { get; set; } = 0m;
        public decimal sumbkrate { get; set; }
        public decimal AvgBook { get; set; }
        public string AccountNumber { get; set; }
    }
}
