namespace iBank.Services.Orm.Classes
{
    public class AirportInformation
    {
        public AirportInformation()
        {
            Airport = string.Empty;
            City = string.Empty;
            State = string.Empty;
            Mode = string.Empty;
            Metro = string.Empty;
            Country = string.Empty;
            Region = string.Empty;
            RRCarrier = string.Empty;
            CountryCode = string.Empty;
            RegionCode = string.Empty;
        }
        public int RecordNumber { get; set; }
        public string Airport { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Mode { get; set; }
        public string Metro { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string RRCarrier { get; set; }
        public string CountryCode { get; set; }
        public string RegionCode { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
    }
}
