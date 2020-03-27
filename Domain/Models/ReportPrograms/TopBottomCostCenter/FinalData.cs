namespace Domain.Models.ReportPrograms.TopBottomCostCenter
{
    public class FinalData
    {
        public string GrpCol { get; set; } = string.Empty;
        public decimal Airchg { get; set; } = 0m;
        public decimal Lostamt { get; set; } = 0m;
        public int Numtrips { get; set; } = 0;
        public int Nohotel { get; set; } = 0;
        public int Stays { get; set; } = 0;
        public int Nights { get; set; } = 0;
        public decimal Hotelcost { get; set; } = 0m;
        public int Rentals { get; set; } = 0;
        public int Days { get; set; } = 0;
        public decimal Carcost { get; set; } = 0;
        public decimal Totalcost { get { return Airchg + Hotelcost + Carcost; } }
    }
}
