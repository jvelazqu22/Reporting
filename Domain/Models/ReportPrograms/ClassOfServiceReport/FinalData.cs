namespace Domain.Models.ReportPrograms.ClassOfServiceReport
{
    public class FinalData
    {
        public int RecKey { get; set; }
        public string Acct { get; set; } = string.Empty;
        public string Acctdesc { get; set; } = string.Empty;
        public string Break1 { get; set; } = string.Empty;
        public string Break2 { get; set; } = string.Empty;
        public string Break3 { get; set; } = string.Empty;
        public string Carrname { get; set; } = string.Empty;
        public string Homectry { get; set; } = string.Empty;
        public string Airline { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public decimal Segcost { get; set; } = 0m;
        public int Segs { get; set; } = 0;
        public int Carrsegs { get; set; } = 0;
        public decimal Carrcost { get; set; } = 0m;
        public decimal Domsegs { get; set; } = 0m;
        public decimal Domsegcost { get; set; } = 0m;
        public decimal Trnsegs { get; set; } = 0m;
        public decimal Trnsegcost { get; set; } = 0m;
        public decimal Intsegs { get; set; } = 0m;
        public decimal Intsegcost { get; set; } = 0m;
    }
}
