

namespace iBank.Services.Implementation.Classes
{
    //Can this be refactored/consolated with TripAirCo2Information, TripCarCo2Information, TripHotelCo2Information?
    public class TripCo2Information
    {
        public decimal AirCo2 { get; set; } = 0;
        public decimal AltCarCo2 { get; set; } = 0;
        public decimal AltRailCo2 { get; set; } = 0;
        public decimal TripCarCo2 { get; set; } = 0; // FP code uses this for both car and hotel
        public decimal RecKey { get; set; } = 0;
        public decimal TripCo2 { get; set; } = 0;
        public string Origin { get; set; } = string.Empty;
        public string Destinat { get; set; } = string.Empty;
    }
}
