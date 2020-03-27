namespace Domain.Models.ReportPrograms.TopBottomCityPairRail
{
    public class FinalData
    {
        public string Origin { get; set; } = string.Empty;
        public string Orgdesc { get; set; } = string.Empty;
        public string Destinat { get; set; } = string.Empty;
        public string Destdesc { get; set; } = string.Empty;
        public int Cpsegs { get; set; } = 0;
        public decimal Cpnumticks { get; set; } = 0;
        public decimal Cpcost { get; set; } = 0m;
        public decimal Cpavgcost { get; set; } = 0m;
        public decimal Cpmiles { get; set; } = 0m;
        public string Airline { get; set; } = string.Empty;
        public string Alinedesc { get; set; } = string.Empty;
        public int Segments { get; set; } = 0;
        public decimal Numticks { get; set; } = 0;
        public decimal Cost { get; set; }
        public decimal Miles { get; set; }
        public int Grp1fld { get; set; } = 0;
        public string Mode { get; set; } = string.Empty;

        public decimal CpPctTtl { get; set; } = 0;
        public decimal PctTtl { get; set; } = 0;
        public decimal CpCst_Mile { get; set; } = 0;
        public decimal Cst_Mile { get; set; } = 0;
    }
}
