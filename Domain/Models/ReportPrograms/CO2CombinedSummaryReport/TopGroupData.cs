namespace Domain.Models.ReportPrograms.CO2CombinedSummaryReport
{
    public class TopGroupData
    {
        public TopGroupData()
        {
            GroupFld = string.Empty;
            Segs = 0;
            Miles = 0;
            Airco2 = 0;
        }
        public string GroupFld { get; set; } = string.Empty;
        public int Segs { get; set; } = 0;
        public int Miles { get; set; } = 0;
        public int Airco2 { get; set; } = 0;
    }
}
