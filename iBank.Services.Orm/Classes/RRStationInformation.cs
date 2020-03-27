namespace iBank.Services.Orm.Classes
{
    public class RRStationInformation
    {
        public RRStationInformation()
        {
            StationName  = string.Empty;
            City  = string.Empty;
            State  = string.Empty;
            Metro  = string.Empty;
            CountryCode  = string.Empty;
            RegionCode  = string.Empty;
            GroupCode  = string.Empty;
            
        }
        public int StationNumber { get; set; }
        public string StationName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Metro { get; set; }
        public string CountryCode { get; set; }
        public string RegionCode { get; set; }
        public string GroupCode { get; set; }

    }
}
