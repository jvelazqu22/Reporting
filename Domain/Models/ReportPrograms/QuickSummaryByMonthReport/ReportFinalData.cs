namespace Domain.Models.ReportPrograms.QuickSummaryByMonthReport
{
    public class ReportFinalData
    {
        public int RptYear { get; set; }
        public int RptMonth { get; set; }
        public string RptMthText { get; set; }
        public int AirTrips { get; set; }
        public decimal AirVolume { get; set; }
        public decimal AirSvgs { get; set; }
        public int AirExcepts { get; set; }
        public decimal AirLost { get; set; }
        public int CarRents { get; set; }
        public int CarDays { get; set; }
        public decimal CarVolume { get; set; }
        public int CarExcepts { get; set; }
        public decimal CarLost { get; set; }
        public int HotStays { get; set; }
        public int HotNights { get; set; }
        public decimal HotVolume { get; set; }
        public int HotExcepts { get; set; }
        public decimal HotLost { get; set; }
    }
}
