namespace iBank.Services.Implementation.Classes
{
    public class PortInformation
    {
        public PortInformation()
        {
            PortCode = string.Empty;
            City = string.Empty;
            State = string.Empty;
            Metro = string.Empty;
            CountryCode = string.Empty;
            RegionCode = string.Empty;
            Mode = string.Empty;
        }

        public string PortCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Metro { get; set; }
        public string CountryCode { get; set; }
        public string RegionCode { get; set; }
        public string Mode { get; set; }
    }
}
