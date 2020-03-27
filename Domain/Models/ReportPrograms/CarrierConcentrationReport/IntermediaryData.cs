namespace Domain.Models.ReportPrograms.CarrierConcentrationReport
{
    public class IntermediaryData
    {
        public string OrgDesc { get; set; }
        public string DestDesc { get; set; }
        public string Origin { get; set; }
        public string Destinat { get; set; }
        public string Flt_mkt { get; set; }
        public string Flt_mkt2 { get; set; }
        public string Mode { get; set; }
        public int Segments { get; set; }
        public decimal Fare { get; set; }
        public int Carr1Segs { get; set; }
        public decimal Carr1Fare { get; set; }
    }
}
