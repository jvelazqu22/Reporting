namespace Domain.Models.ReportPrograms.QuickSummaryByMonthReport
{
    public class HotelFinalData
    {
        public int RptYear { get; set; }
        public int RptMonth { get; set; }
        public string RptMthText { get; set; }
        public int HotStays { get; set; }
        public int HotNights { get; set; }
        public int HPlusMin { get; set; }
        public int Nights { get; set; }
        public int Rooms { get; set; }
        public decimal BookRate { get; set; }
        public string ReasCodh { get; set; }
        public decimal Hexcprat { get; set; }
        public int HotExcepts { get; set; }
        public decimal HotLost { get; set; }
    }
}
