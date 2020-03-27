namespace Domain.Models.ReportPrograms.ExecutiveSummarywithGraphsReport
{
    public class PieData
    {
        public PieData()
        {
            Catdesc = string.Empty;
            Data1 = 0m;
            Pietitle = string.Empty;
        }
        public string Catdesc { get; set; }
        public decimal Data1 { get; set; }
        public string Pietitle { get; set; }
    }
}
