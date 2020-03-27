namespace Domain.Models.ReportPrograms.CarrierConcentrationReport
{
    public class FinalData
    {
        public FinalData()
        {
            OrgDesc = string.Empty;
            DestDesc = string.Empty;
            Origin = string.Empty;
            Destinat = string.Empty;
            Segments = 0;
            Totvolume = 0m;
            Avgsegcost = 0m;
            Carrier = string.Empty;
            Carrsegs = 0;
            Pcntoftot = 0m;
            Carrvolume = 0m;
            Carravgseg = 0m;
            Avgsegdiff = 0m;
            Carrsvngs = 0m;
            Othsvngs = 0m;
            Fare = 0m;
        }
        public string OrgDesc { get; set; }
        public string DestDesc { get; set; }
        public string Origin { get; set; }
        public string Destinat { get; set; }
        public int Segments { get; set; }
        public decimal Totvolume { get; set; }
        public decimal Avgsegcost { get; set; }
        public string Carrier { get; set; }
        public int Carrsegs { get; set; }
        public decimal Pcntoftot { get; set; }
        public decimal Carrvolume { get; set; }
        public decimal Carravgseg { get; set; }
        public decimal Avgsegdiff { get; set; }
        public decimal Carrsvngs { get; set; }
        public decimal Othsvngs { get; set; }
        public decimal Fare { get; set; }

    }
}
