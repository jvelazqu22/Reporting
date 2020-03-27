namespace Domain.Models.ReportPrograms.HotelFareSavings
{
    public class SubReportData
    {
        public string Savingdesc { get; set; } = string.Empty;
        public int Svgcount { get; set; } = 0;
        public decimal Svgamt { get; set; } = 0m;
        public string Lossdesc { get; set; } = string.Empty;
        public int Losscount { get; set; } = 0;
        public decimal Lossamt { get; set; } = 0m;
    }
}
