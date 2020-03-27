namespace Domain.Models.ReportPrograms.ClassOfServiceReport
{
    public class SubReportFinalData
    {
        public string Class { get; set; } = string.Empty;
        public decimal Segcost { get; set; } = 0m;
        public int Segs { get; set; } = 0;
        public decimal Domsegs { get; set; } = 0m;
        public decimal Domsegcost { get; set; } = 0m;
        public decimal Trnsegs { get; set; } = 0m;
        public decimal Trnsegcost { get; set; } = 0m;
        public decimal Intsegs { get; set; } = 0m;
        public decimal Intsegcost { get; set; } = 0m;
        public decimal Totsegcost { get; set; } = 0m;
        public decimal Totsegs { get; set; } = 0m;
    }
}
