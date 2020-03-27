namespace Domain.Models.ReportPrograms.TravelManagementSummary
{
    public class FinalData
    {
        public FinalData()
        {
            Category = string.Empty;
            GrpCode = string.Empty;
            RowDesc = string.Empty;
            DataType = string.Empty;
        }

        public string Category { get; set; }
        public string GrpCode { get; set; }
        public int RowNum { get; set; }
        public string RowDesc { get; set; }
        public string DataType { get; set; }
        public decimal Mth1 { get; set; }
        public decimal Mth2 { get; set; }
        public decimal Mth3 { get; set; }
        public decimal Mth4 { get; set; }
        public decimal Mth5 { get; set; }
        public decimal Mth6 { get; set; }
        public decimal Mth7 { get; set; }
        public decimal Mth8 { get; set; }
        public decimal Mth9 { get; set; }
        public decimal Mth10 { get; set; }
        public decimal Mth11 { get; set; }
        public decimal Mth12 { get; set; }
        public decimal Ytd { get; set; }
    }
}

