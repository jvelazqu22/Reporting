namespace Domain.Models.ReportPrograms.AirFareSavingsReport
{
    public class NegotiatedSavingsModel
    {
        public int Reckey { get; set; }
        public string Savingcode { get; set; }
        public string Reascode { get; set; }
        public decimal Airchg { get; set; }
        public decimal Offrdchg { get; set; }
        public decimal Stndchg { get; set; }
        public decimal Lostamt { get; set; }
        public decimal Savings { get; set; }
        public decimal Negosvngs { get; set; }
        public int Plusmin { get; set; }
        public string Origacct { get; set; }
    }
}
