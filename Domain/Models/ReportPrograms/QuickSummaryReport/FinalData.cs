namespace Domain.Models.ReportPrograms.QuickSummaryReport
{
    public class FinalData
    {
        public FinalData()
        {
            Rownum = 0;
            Grp = string.Empty;
            Grpsort = 0;
            Subgrp = 0;
            Descrip = string.Empty;
            TotsDecimal = 0;
            Avgs = string.Empty;
            Svgs = string.Empty;
        }
        public int Rownum { get; set; }
        public string Grp { get; set; }
        public int Grpsort { get; set; }
        public int Subgrp { get; set; }
        public string Descrip { get; set; }
        public decimal TotsDecimal { get; set; } //used for calculating totals
        //for standard report
        public string Tots { get; set; }
        public string Avgs { get; set; }
        public string Svgs { get; set; }

        //for "Dom/Intl" report
        public string Tots1 { get; set; }
        public decimal Tots1Decimal { get; set; } //used for calculating totals
        public string Avgs1 { get; set; }
        public string Svgs1 { get; set; }
        public string Tots2 { get; set; }
        public decimal Tots2Decimal { get; set; } //used for calculating totals
        public string Avgs2 { get; set; }
        public string Svgs2 { get; set; }
        public string Tots3 { get; set; }
        public decimal Tots3Decimal { get; set; } //used for calculating totals
        public string Avgs3 { get; set; }
        public string Svgs3 { get; set; }
    }
}
