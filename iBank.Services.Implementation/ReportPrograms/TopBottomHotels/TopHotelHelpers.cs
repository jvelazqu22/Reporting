using Domain.Models.ReportPrograms.TopBottomHotelsReport;
using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities;
using System.Collections.Generic;
using System.Linq;
using Domain.Services;
using iBank.Services.Implementation.Shared.LookupFunctionHelpers;

namespace iBank.Services.Implementation.ReportPrograms.TopBottomHotels
{
    public static class TopHotelHelpers
    {
        public static string GetReportName(string groupBy, bool secondRange)
        {
            switch (groupBy)
            {
                case "6":
                case "7":
                case "8":
                    return "ibTopHotel4";
                case "5":
                    return "ibTopHotel3";
                default:
                    return secondRange ? "ibTopHotel2" : "ibTopHotels";
            }
        }

        /// <summary>
        /// TRIES TO DERIVE A COUNTRY FROM THE METRO CODE.  IF 
        /// THAT FAILS, USES THE 2ND PARAMETER</summary>
        /// <param name="metroCode"></param>
        /// <param name="country"></param>
        /// <param name="userLanguage"></param>
        /// <returns></returns>
        public static string LookupCountry(string metroCode, string country, string userLanguage)
        {
            metroCode = metroCode.Trim();
            country = country.Trim();
            var countryName = string.Empty;
            var cache = new CacheService();
            var store = new MasterDataStore();

            var countries = CountriesLookup.GetCountries(cache, store.MastersQueryDb);

            //check by metro country
            if (!string.IsNullOrEmpty(metroCode))
            {
                //This report doesn't use the standard lookup functions
                var metros = MetrosLookup.GetMetros(cache, store.MastersQueryDb);
                var metro = metros.FirstOrDefault(s => s.MetroCode.EqualsIgnoreCase(metroCode));
                if (metro != null)
                {
                    var ctry = countries.FirstOrDefault(s => s.CountryCode.EqualsIgnoreCase(metro.CountryCode) && s.LanguageCode.EqualsIgnoreCase(userLanguage));
                    if (ctry != null)
                        countryName = ctry.CountryName;
                }
            }

            //check by country
            if (string.IsNullOrEmpty(countryName) && !string.IsNullOrEmpty(country) && country.Length == 3)
            {
                var ctry = countries.FirstOrDefault(s => s.CountryCode.EqualsIgnoreCase(country) && s.LanguageCode.EqualsIgnoreCase(userLanguage));
                if (ctry != null)
                    countryName = ctry.CountryName;
            }

            if (string.IsNullOrEmpty(countryName) && !string.IsNullOrEmpty(country))
                countryName = country;

            if (countryName == null)
                countryName = string.Empty;

            return countryName.PadRight(36);
        }

        public static List<FinalData> SortFinalData(string sortBy, List<FinalData> list, bool desc)
        {
            return desc ? ApplyDescendingSort(sortBy, list) : ApplyAscendingSort(sortBy, list);
        }

        public static List<FinalData> ApplyDescendingSort(string sortBy, List<FinalData> list)
        {
            switch (sortBy)
            {
                case "1":
                    return list.OrderByDescending(s => s.Hotelcost).ToList();
                case "2":
                    return list.OrderByDescending(s => s.Stays).ToList();
                case "3":
                    return list.OrderByDescending(s => s.Nights).ToList();
                case "4":
                    return list.OrderByDescending(s => s.Avgbook).ToList();
                default:
                    return list.OrderByDescending(s => s.Category).ToList();
            }
        }

        public static List<FinalData> ApplyAscendingSort(string sortBy, List<FinalData> list)
        {
            switch (sortBy)
            {
                case "1":
                    return list.OrderBy(s => s.Hotelcost).ToList();
                case "2":
                    return list.OrderBy(s => s.Stays).ToList();
                case "3":
                    return list.OrderBy(s => s.Nights).ToList();
                case "4":
                    return list.OrderBy(s => s.Avgbook).ToList();
                default:
                    return list.OrderBy(s => s.Category).ToList();
            }
        }
    }
}
