namespace Domain.Orm.Classes
{
    public class RRStationInformation
    {
        public int StationNumber { get; set; }
        public string StationName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Metro { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
        public string RegionCode { get; set; } = string.Empty;
        public string GroupCode { get; set; } = string.Empty;

    }
}
