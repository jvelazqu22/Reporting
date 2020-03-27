
namespace Domain.Models.ReportPrograms.TopBottomTravelerAir
{
    public class FinalData
    {
        public FinalData()
        {
            Passlast = string.Empty;
            Passfrst = string.Empty;
            Homectry = string.Empty;
            Amt = 0m;
            Trips = 0;
            Lostamt = 0m;
            Totbkdays = 0;
            //Avgcost = 0m;
            //Avgbkdays = 0m;
        }
        public string Passlast { get; set; }
        public string Passfrst { get; set; }
        public string Homectry { get; set; }
        public decimal Amt { get; set; }
        public int Trips { get; set; }
        public decimal Lostamt { get; set; }
        public decimal Totbkdays { get; set; }
        public decimal Avgcost { get {return Trips == 0 ? 0 : Amt/Trips;} }
        public decimal Avgbkdays { get { return Trips == 0 ? 0 : Totbkdays / Trips; } }
    }
}
