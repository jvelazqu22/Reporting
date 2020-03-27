namespace Domain.Orm.Classes
{
    public class AirportInformation
    {
        public int RecordNumber { get; set; }
        public string Airport { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Mode { get; set; } = string.Empty;
        public string Metro { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string RRCarrier { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
        public string RegionCode { get; set; } = string.Empty;
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public string AirportName { get; set; }
    }
}
