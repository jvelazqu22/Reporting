using Domain.Interfaces;


namespace iBank.Services.Implementation.Classes
{
    public class TripCarCo2Information:ICarbonCalculationsCar
    {
        public TripCarCo2Information()
        {
            AirCo2 = 0;
            AltCarCo2 = 0;
            AltRailCo2 = 0;
            ClassCat = string.Empty;
            DitCode = string.Empty;
            Miles = 0;
            CPlusMin = 0;
            RecKey = 0;
            TripCarCo2 = 0;
            TripCo2 = 0;
            CarType = string.Empty;
        }
        public decimal AirCo2 { get; set; }
        public decimal AltCarCo2 { get; set; }
        public decimal AltRailCo2 { get; set; }
        public decimal TripCarCo2 { get; set; }
        public decimal TripCo2 { get; set; }
        public int Miles { get; set; }
        public int RecKey { get; set; }
        public string ClassCat { get; set; }
        public string DitCode { get; set; }
        public string CarType { get; set; }
        public int Days { get; set; }
        public decimal CarCo2 { get; set; }
        public int CPlusMin { get; set; }
    }
}
