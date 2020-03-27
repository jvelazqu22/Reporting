namespace Domain.Models.ReportPrograms.TopBottomCityPairReport
{
    public class SemiFinalData
    {
        public string Origin { get; set; } = string.Empty;
        public string Destinat { get; set; } = string.Empty;
        public string Airline { get; set; } = string.Empty;
        public string Mode { get; set; } = string.Empty;
        public decimal AirCO2 { get; set; } = 0m;
        public int Segments { get; set; } = 0;
        public decimal Numticks { get; set; } = 0m;
        public decimal OnlineTkts { get; set; } = 0m;
        public decimal AgentTkts { get; set; } = 0m;
        public int OnlineSegs { get; set; } = 0;
        public int AgentSegs { get; set; } = 0;
        public decimal AgentCost { get; set; } = 0m;
        public decimal OnlineCost { get; set; }
        public decimal Cost { get; set; } = 0m;
        public decimal Miles { get; set; } = 0m;
    }
}