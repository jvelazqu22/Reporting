namespace Domain.Models.ReportPrograms.ExecutiveSummarywithGraphsReport
{
    public class AirSumData
    {
        public int Invoices { get; set; } = 0;
        public int Credits { get; set; } = 0;
        public int Trips { get; set; } = 0;
        public decimal Airchg { get; set; } = 0m;
        public decimal Savings { get; set; } = 0m;
        public int Airexcepts { get; set; } = 0;
        public decimal Lostamt { get; set; } = 0m;
        public decimal Negosvngs { get; set; } = 0m;
        public decimal Svcfee { get; set; } = 0m;
        public decimal Avgairchg { get; set; } = 0m;
        public decimal Avgsvgs { get; set; } = 0m;
        public decimal Avglost { get; set; } = 0m;
        public int Miles { get; set; } = -1;
        public decimal Airco2 { get; set; } = -1;
    }
}
