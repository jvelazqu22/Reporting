using System.Runtime.Serialization;

namespace Domain.Orm.Classes
{
    [DataContract]
    public class CountriesInformation
    {

        public string CountryCode { get; set; } = string.Empty;
        public string CountryName { get; set; } = string.Empty;
        public string NumberCountryCode { get; set; } = string.Empty;
        public string RegionCode { get; set; } = string.Empty;
        public string LanguageCode { get; set; } = "EN";
    }
}