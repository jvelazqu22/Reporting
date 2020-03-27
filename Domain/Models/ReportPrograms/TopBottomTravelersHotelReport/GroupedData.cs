namespace Domain.Models.ReportPrograms.TopBottomTravelersHotelReport
{
    public class GroupedData
    {
        public GroupedData()
        {
            PassLast = string.Empty;
            PassFrst = string.Empty;
            Stays = 0;
            Nights = 0;
            HotelCost = 0;
            BookRate =
            SumBookRate = 0;
        }
        public string PassLast { get; set; }
        public string PassFrst { get; set; }
        public int Nights { get; set; }
        public int Stays { get; set; }
        public decimal HotelCost { get; set; }
        public decimal BookRate { get; set; }
        public decimal SumBookRate { get; set; }
    }
}
