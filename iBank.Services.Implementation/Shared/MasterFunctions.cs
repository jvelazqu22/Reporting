using Domain.Helper;
using Domain.Orm.Classes;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using System.Collections.Generic;
using System.Linq;

using iBankDomain.RepositoryInterfaces;

namespace iBank.Services.Implementation.Shared
{
    public class MasterFunctions
    {
        private List<KeyValue> HomeCountries { get; set; } = new List<KeyValue>();
        private List<CountriesInformation> Countries { get; set; } = new List<CountriesInformation>();

        public string LookupHomeCountryName(string sourceAbbr, ReportGlobals globals, IQuery<IList<KeyValue>> GetHomeCountriesByAgencyForSharerClientQuery,
            IQuery<IList<KeyValue>> GetHomeCountriesByAgencyForNonSharerClientQuery, IQuery<IList<CountriesInformation>> GetAllCountriesQuery)
        {
            HomeCountries = LoadHomeCountries(HomeCountries, globals.ClientType, GetHomeCountriesByAgencyForSharerClientQuery, GetHomeCountriesByAgencyForNonSharerClientQuery);

            var homeCountry = HomeCountries.FirstOrDefault(s => s.Key.EqualsIgnoreCase(sourceAbbr));
            if (homeCountry != null)
            {
                Countries = LoadCountries(Countries, GetAllCountriesQuery);
                var country = Countries.FirstOrDefault(s => s.CountryCode.EqualsIgnoreCase(homeCountry.Value) && s.LanguageCode.EqualsIgnoreCase(globals.UserLanguage));
                if (country != null) return country.CountryName.PadRight(40);
            }

            return "[none]";
        }

        public List<KeyValue> LoadHomeCountries(List<KeyValue> homeCountries, ClientType clientType, IQuery<IList<KeyValue>> GetHomeCountriesByAgencyForSharerClientQuery, 
            IQuery<IList<KeyValue>> GetHomeCountriesByAgencyForNonSharerClientQuery)
        {
            if (homeCountries == null || !homeCountries.Any())
            {
                if (clientType == ClientType.Sharer)
                {
                    return GetHomeCountriesByAgencyForSharerClientQuery.ExecuteQuery().ToList();
                }
                else
                {
                    return GetHomeCountriesByAgencyForNonSharerClientQuery.ExecuteQuery().ToList();
                }
            }

            return homeCountries;
        }

        public List<CountriesInformation> LoadCountries(List<CountriesInformation> countries, IQuery<IList<CountriesInformation>> GetAllCountriesQuery)
        {
            if (countries != null && countries.Any()) return countries;

            return GetAllCountriesQuery.ExecuteQuery().ToList();
        }

    }
}
