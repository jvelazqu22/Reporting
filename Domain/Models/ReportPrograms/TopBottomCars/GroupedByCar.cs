namespace Domain.Models.ReportPrograms.TopBottomCars
{
    public class GroupedByCar
    {
        public string Category { get; set; } = string.Empty;
        public string Company { get; set; }
        public int Rentals { get; set; } = 0;
        public int Days { get; set; } = 0;
        public int NzDays { get; set; } = 0;
        public decimal CarCost { get; set; } = 0;
        public decimal BookRate { get; set; } = 0;
        public int BookCnt { get; set; } = 0;
    }
}
