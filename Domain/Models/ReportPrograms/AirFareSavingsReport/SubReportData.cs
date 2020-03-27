namespace Domain.Models.ReportPrograms.AirFareSavingsReport
{
    public class SubReportData
    {
        public SubReportData()
        {
            Savingdesc = string.Empty;
            Svgcount = 0;
            Svgamt = 0m;
            Lossdesc = string.Empty;
            Losscount = 0;
            Lossamt = 0m;
        }
        public string Savingdesc { get; set; }
        public int Svgcount { get; set; }
        public decimal Svgamt { get; set; }
        public string Lossdesc { get; set; }
        public int Losscount { get; set; }
        public decimal Lossamt { get; set; }
    }
}
