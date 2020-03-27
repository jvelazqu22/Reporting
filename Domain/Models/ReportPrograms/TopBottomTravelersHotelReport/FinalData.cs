namespace Domain.Models.ReportPrograms.TopBottomTravelersHotelReport
{
    public class FinalData
    {
        public string PassLast { get; set; } = string.Empty;
        public string PassFrst { get; set; } = string.Empty;
        public int Nights { get; set; } = 0;
        public int Stays { get; set; } = 0;
        public decimal HotelCost { get; set; } = 0;
        public decimal BookRate { get; set; } = 0;
        public decimal SumBookRate { get; set; } = 0;
        public int BookCnt { get; set; } = 0;
        public decimal AvgBook { get { return BookCnt == 0 ? 0 : BookRate/BookCnt; } }
    }
}
