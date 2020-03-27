using Domain.Helper;
using Domain.Interfaces;

namespace iBank.Services.Implementation.Classes
{
    public class TripAirCo2Information : ICarbonCalculations, IAirMileage
    {
        public TripAirCo2Information()
        {
            AirCo2 = 0;
            AltCarCo2 = 0;
            AltRailCo2 = 0;
            ClassCat = string.Empty;
            DitCode = string.Empty;
            Miles = 0;
            RecKey = 0;
            TripCo2 = 0;
        }

        public decimal AirCo2 { get; set; }
        public decimal AltCarCo2 { get; set; }
        public decimal AltRailCo2 { get; set; }
        public decimal TripCo2 { get; set; }
        public int Miles { get; set; }
        public string Mode { get; set; }
        public string Origin { get; set; }
        public string Destinat { get; set; }
        public int RecKey { get; set; }
        public string ClassCat { get; set; }
        public string DitCode { get; set; }
        public string HaulType { get; set; }
    }
}
