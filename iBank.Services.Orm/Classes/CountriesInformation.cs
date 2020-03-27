using System.Runtime.Serialization;

namespace iBank.Services.Orm.Classes
{
    [DataContract]
    public class CountriesInformation
    {
        public CountriesInformation()
        {
            CountryCode = string.Empty;
            CountryName = string.Empty;
            NumberCountryCode = string.Empty;
            RegionCode = string.Empty;
            LanguageCode = "EN";
        }

        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string NumberCountryCode { get; set; }
        public string RegionCode { get; set; }
        public string LanguageCode { get; set; }
    }
}