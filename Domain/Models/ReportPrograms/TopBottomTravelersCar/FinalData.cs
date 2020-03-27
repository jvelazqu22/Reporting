namespace Domain.Models.ReportPrograms.TopBottomTravelersCar
{
    public class FinalData
    {
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;
        public int Rentals { get; set; } = 0;
        public int Days { get; set; } = 0;
        public decimal Carcost { get; set; } = 0m;
        public decimal Bookrate { get; set; } = 0m;
        public decimal Bookcnt { get; set; } = 0m;
        public decimal Avgbook { get; set; } = 0m;
    }
}
