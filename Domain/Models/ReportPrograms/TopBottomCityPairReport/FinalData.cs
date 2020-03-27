namespace Domain.Models.ReportPrograms.TopBottomCityPairReport
{
    public class FinalData
    {
        public string Origin { get; set; } = string.Empty;
        public string Orgdesc { get; set; } = string.Empty;
        public string Destinat { get; set; } = string.Empty;
        public string Destdesc { get; set; } = string.Empty;
        public int Cpsegs { get; set; } = 0;
        public decimal CpNumticks { get; set; } = 0m;
        public decimal CpPctTtl { get; set; } = 0m;
        public decimal Cpcost { get; set; } = 0m;
        public decimal CpAvgcost { get; set; } = 0m;
        public decimal CpOnlnTkts { get; set; } = 0m;
        public decimal CpAgntTkts { get; set; } = 0m;
        public decimal CpOnlnCost { get; set; } = 0m;
        public decimal CpAgntCost { get; set; } = 0m;
        public int CpAgntSegs { get; set; } = 0;
        public int CpOnlnSegs { get; set; } = 0;
        public decimal CpAgntAvg { get; set; } = 0m;

        public decimal CpOnlnAvg { get; set; } = 0m;
        public decimal CpAirCO2 { get; set; } = 0m;

        public decimal Cpmiles { get; set; } = 0m;

        public string Airline { get; set; } = string.Empty;
        public string Alinedesc { get; set; } = string.Empty;
        public decimal AirCO2 { get; set; } = 0m;
        public int Segments { get; set; } = 0;
        public decimal PctTtl { get; set; } = 0m;
        public decimal Numticks { get; set; } = 0m;
        public decimal Cost { get; set; } = 0m;
        public decimal Miles { get; set; } = 0m;
        public decimal Cpcst_Mile { get; set; } = 0m;
        public decimal Cst_Mile { get; set; } = 0m;

        public decimal Cpcst_km { get; set; } = 0m;
        public decimal Cst_km { get; set; } = 0m;
        public string Grp1fld { get; set; } = string.Empty;
    }
}
