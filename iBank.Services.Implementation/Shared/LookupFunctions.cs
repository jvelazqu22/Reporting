using CODE.Framework.Core.Utilities.Extensions;

using Domain.Helper;
using Domain.Orm.Classes;

using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Classes;
using iBank.Services.Implementation.Shared.LookupFunctionHelpers;

using iBankDomain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Domain.Constants;
using Domain;
using Domain.Services;
using iBank.Services.Implementation.ReportPrograms.UserDefined.Managers;
using iBank.Services.Implementation.Shared.Client;

namespace iBank.Services.Implementation.Shared
{
    public static class LookupFunctions
    {
        private static readonly ICacheService _cache = new CacheService();

        public enum OrgDestType
        {
            Port,
            Metro,
            Country,
            Region
        }
        public enum CO2Type
        {
            AirCo2 = 1,
            TripCo2,
            AltRailCo2,
            AltCarCo2
        }

        public static decimal LookupTrpCo2(List<TripCo2Information> tripCo2, int recKey, CO2Type type)
        {
            if (tripCo2 == null || !tripCo2.Any()) return 0;

            return GetTripLevelCo2ByType(tripCo2, recKey, type);
        }

        public static decimal GetTripLevelCo2ByType(List<TripCo2Information> tripCo2, int recKey, CO2Type type)
        {
            var records = tripCo2.Where(s => s.RecKey == recKey).ToList();
            if (records == null) return 0;
            switch (type)
            {
                case CO2Type.AirCo2:
                    return records.Sum(x => x.AirCo2);
                case CO2Type.TripCo2:
                    return records.Sum(x => x.TripCo2);
                case CO2Type.AltRailCo2:
                    return records.Sum(x => x.AltRailCo2);
                case CO2Type.AltCarCo2:
                    return records.Sum(x => x.AltCarCo2);
                default:
                    return 0;
            }
        }

        public static decimal LookupTrpCo2(List<TripCo2Information> tripCo2, int recKey, int type)
        {
            if (tripCo2 == null || !tripCo2.Any()) return 0;
            var firstOrDefault = tripCo2.FirstOrDefault(s => s.RecKey == recKey);
            if (firstOrDefault == null) return 0;
            switch (type)
            {
                case 1:
                    return firstOrDefault.AirCo2;
                case 2:
                    return firstOrDefault.TripCo2;
                case 3:
                    return firstOrDefault.AltRailCo2;
                case 4:
                    return firstOrDefault.AltCarCo2;
                default:
                    return 0;
            }
        }

        //api done
        public static string LookupCreditCardCompany(string cardNum)
        {
            if (cardNum.IsNullOrWhiteSpace()) return "";
            switch (cardNum.Left(2))
            {
                case "AX":
                    return "AMEX";
                case "CA":
                    return "MASTER CARD";
                case "VI":
                    return "VISA";
                case "DC":
                    return "DINERS CLUB";
                case "DS":
                    return "DISCOVER";
                case "TP":
                    return "UATP";
                default:
                    return "";
            }
        }

        public static string LookupCountryNumber(IMasterDataStore store, string countryCode, ReportGlobals globals = null)
        {
            if (countryCode.IsNullOrWhiteSpace()) return new string(' ', 3);
            var countries = CountriesLookup.GetCountries(_cache, store.MastersQueryDb);
            CountriesInformation country;
            if (globals == null)
            {
                country = countries.FirstOrDefault(s => s.CountryCode == countryCode);
                return country == null ? string.Empty : country.NumberCountryCode;
            }

            //Filter the list of countries by users language code
            country = countries.FirstOrDefault(s => s.CountryCode == countryCode && s.LanguageCode == globals.UserLanguage);
            return country == null ? string.Empty : country.NumberCountryCode;
        }

        public static string LookupDomesticInternational(string key, string langCode, IMasterDataStore store)
        {
            var domesticInternational = DomesticInternationalLookup.GetDomesticInternational(_cache, store.MastersQueryDb, langCode);
            
            var domIntl = domesticInternational.FirstOrDefault(s => s.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase));
            return domIntl == null ? string.Empty : domIntl.Value;
        }
        
        public static string LookupOperator(string key, string langCode, IMasterDataStore store)
        {
            var operators = OperatorsLookup.GetOperators(_cache, store.MastersQueryDb);
            var oper = new KeyValue();
            
            oper = operators.FirstOrDefault(s => s.LangCode.Equals(langCode, StringComparison.InvariantCultureIgnoreCase)
                                                            && s.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase));
            return oper == null ? string.Empty : oper.Value;            
        }
        
        public static string LookupColumnDisplayName(string colName, string defaultColumnName, string langCode, IMasterDataStore store)
        {
            var columns = ColumnCaptionsLookup.GetColumnCaptions(_cache, store, langCode);
            var col = columns.FirstOrDefault(s => s.Key.Equals(colName.Trim().ToUpper(), StringComparison.InvariantCultureIgnoreCase));

            //if no matching column is found for the user language default to English (EN)
            if (col == null)
            {
                columns = ColumnCaptionsLookup.GetColumnCaptions(_cache, store, "EN");
                col = columns.FirstOrDefault(x => x.Key.Trim().ToUpper().Equals(colName.Trim().ToUpper(), StringComparison.InvariantCultureIgnoreCase));
            }

            //if the column has not been found under both the user's language and English, default to the default column name
            return col == null ? defaultColumnName : col.Value;
        }

        public static string LookupLanguageTranslation(string key, string defaultTranslation, List<LanguageVariableInfo> tags)
        {
            var tag = tags.FirstOrDefault(s => s.VariableName.Equals(key.Trim(), StringComparison.InvariantCultureIgnoreCase));
            return tag == null ? defaultTranslation : tag.Translation;
        }

        //public static string LookupAport(IMasterDataStore store, string inPort, string inMode, string airline = "")
        //{
        //    inPort = inPort.Trim();
        //    inMode = inMode.Trim();
        //    if (string.IsNullOrEmpty(inPort)) return new string(' ', 28);
        //    string portDescription;

        //    if (string.IsNullOrEmpty(inMode)) inMode = LookupConstants.Achar;

        //    var rrStationNumber = 0;
        //    var railRoadLookup = false;
        //    if (inMode.EqualsIgnoreCase(LookupConstants.Rchar) && !inPort.Contains(LookupConstants.Echar) && !inPort.Contains(LookupConstants.Dash))
        //    {
        //        railRoadLookup = int.TryParse(inPort, out rrStationNumber);
        //    }

        //    //TODO: There is a comment here that indicates an odd exception in the logic for a program called ibPrevLoad:
        //    //** 09/17/2009 - PBLM: THIS FUNCTION IS CALLED BY THE CHANGE MGMT ROUTINE IN **
        //    //** ibPrevLoad, AND ibPrevLoad DOES NOT HANDLE THE RAIL CURSOR -- IT HAS ITS **
        //    //** OWN ROUTINE FOR CREATING THE curAirport CURSOR.  WE'RE GOING TO MAKE AN  **
        //    //** ASSUMPTION HERE, THAT IF THE curRRStations CURSOR IS NOT OPEN, THEN WE   **
        //    //** ARE NOT GOING TO DEAL WITH RAIL. 
        //    if (railRoadLookup)
        //    {
        //        var railroadStations = RailroadStationLookup.GetRailroadStations(_cache, store.MastersQueryDb);
        //        var railroadStation = railroadStations.FirstOrDefault(s => s.StationNumber == rrStationNumber);
        //        if (railroadStation == null)
        //        {
        //            portDescription = string.Format(LookupConstants.NotFound, inPort);
        //        }
        //        else
        //        {
        //            portDescription = railroadStation.CountryCode.EqualsIgnoreCase(LookupConstants.Canada) || railroadStation.CountryCode.EqualsIgnoreCase(LookupConstants.Usa)
        //                ? railroadStation.StationName.Trim() + LookupConstants.Comma + railroadStation.State
        //                : railroadStation.StationName.Trim() + LookupConstants.Comma + railroadStation.CountryCode;
        //        }
        //    }
        //    else
        //    {
        //        /*
        //            if the data is coming from the iblegs table the inPort might have a '-' in it
        //            if the data is coming from the ibmktsegs table the inPort won't have a '-' in it
        //            but, to look up a railroad in the airports table, the airport column expects the inPort to have a '-' when the mode = 'R'
        //            so, if we get this far, and the inPort isn't coming from iblegs (doesn't have a '-'), need to concat inPort-airline
        //        */
        //        inPort = ReportHelper.CreateOriginDestCode(inPort, inMode, airline);
        //        var airports = AirportLookup.GetAirports(_cache, store.MastersQueryDb);
        //        var airport = airports.FirstOrDefault(s => s.Airport.Trim() == inPort.Trim());
        //        if (airport == null)
        //        {
        //            portDescription = string.Format(LookupConstants.NotFound, inPort);
        //        }
        //        else
        //        {
        //            portDescription = !string.IsNullOrEmpty(airport.State)
        //                ? airport.City.Trim() + LookupConstants.Comma + airport.State
        //                : airport.City.Trim();
        //        }
        //    }
        //    return portDescription;
        //}

        private static string LookupAirportName(string inPort, string inMode, IMasterDataStore store, bool isTitleCase)
        {
            inPort = inPort.Trim();
            inMode = inMode.Trim();
            if (string.IsNullOrEmpty(inPort)) return new string(' ', 28);
            string portDescription;

            if (string.IsNullOrEmpty(inMode))
                inMode = LookupConstants.Achar;

            var rrStationNumber = 0;
            var railRoadLookup = false;
            if (inMode.EqualsIgnoreCase(LookupConstants.Rchar) && !inPort.Contains(LookupConstants.Echar) && !inPort.Contains(LookupConstants.Dash))
            {
                railRoadLookup = int.TryParse(inPort, out rrStationNumber);
            }

            if (railRoadLookup)
            {
                var railroadStations = RailroadStationLookup.GetRailroadStations(_cache, store.MastersQueryDb);
                var railroadStation = railroadStations.FirstOrDefault(s => s.StationNumber == rrStationNumber);
                portDescription = railroadStation == null ? string.Format(@"RAIL ""{0}"" NOT FND", inPort) : railroadStation.StationName;
            }
            else
            {
                var airports = AirportLookup.GetAirports(_cache, store.MastersQueryDb);
                var airport = airports.FirstOrDefault(s => s.Airport.Trim() == inPort.Trim());
                portDescription = airport == null ? string.Format(@"AIRPORT ""{0}"" NOT FND", inPort) : airport.Airport;
            }

            return isTitleCase ? portDescription.TitleCaseString() : portDescription;
        }

        public static string LookupAportCity(string inPort, IMasterDataStore store, bool isTitleCase, string inMode = "")
        {
            inPort = inPort.Trim();
            inMode = inMode.Trim();
            if (string.IsNullOrEmpty(inPort)) return new string(' ', 28);
            string portDescription;

            if (string.IsNullOrEmpty(inMode))
            {
                inMode = LookupConstants.Achar;
            }

            var rrStationNumber = 0;
            var railRoadLookup = false;
            if (inMode.EqualsIgnoreCase(LookupConstants.Rchar) && !inPort.Contains(LookupConstants.Echar) && !inPort.Contains(LookupConstants.Dash))
            {
                railRoadLookup = int.TryParse(inPort, out rrStationNumber);
            }

            if (railRoadLookup)
            {
                var railroadStations = RailroadStationLookup.GetRailroadStations(_cache, store.MastersQueryDb);
                var railroadStation = railroadStations.FirstOrDefault(s => s.StationNumber == rrStationNumber);
                portDescription = railroadStation != null ? railroadStation.StationName : string.Format(LookupConstants.NotFound, inPort);
            }
            else
            {
                var airports = AirportLookup.GetAirports(_cache, store.MastersQueryDb);
                var airport = airports.FirstOrDefault(s => s.Airport.EqualsIgnoreCase(inPort));
                portDescription = airport != null && !string.IsNullOrEmpty(airport.City) ? airport.City.ToUpper(CultureInfo.InvariantCulture) : string.Format(LookupConstants.NotFound, inPort);
            }
            return isTitleCase ? portDescription.TitleCaseString().PadRight(44) : portDescription.PadRight(44);

        }

        public static string LookupAportName(string inPort, IMasterDataStore store, bool isTitleCase, string inMode = "")
        {
            inPort = inPort.Trim();
            inMode = inMode.Trim();
            if (string.IsNullOrEmpty(inPort)) return new string(' ', 28);
            string portDescription;

            if (string.IsNullOrEmpty(inMode))
            {
                inMode = LookupConstants.Achar;
            }

            var rrStationNumber = 0;
            var railRoadLookup = false;
            if (inMode.EqualsIgnoreCase(LookupConstants.Rchar) && !inPort.Contains(LookupConstants.Echar) && !inPort.Contains(LookupConstants.Dash))
            {
                railRoadLookup = int.TryParse(inPort, out rrStationNumber);
            }

            if (railRoadLookup)
            {
                var railroadStations = RailroadStationLookup.GetRailroadStations(_cache, store.MastersQueryDb);
                var railroadStation = railroadStations.FirstOrDefault(s => s.StationNumber == rrStationNumber);
                portDescription = railroadStation != null ? railroadStation.StationName : string.Format(LookupConstants.NotFound, inPort);
            }
            else
            {
                var airports = AirportLookup.GetAirports(_cache, store.MastersQueryDb);
                var airport = airports.FirstOrDefault(s => s.Airport.EqualsIgnoreCase(inPort));
                portDescription = airport != null && !string.IsNullOrEmpty(airport.AirportName) ? airport.AirportName.ToUpper(CultureInfo.InvariantCulture) : string.Format(LookupConstants.NotFound, inPort);
            }
            return isTitleCase ? portDescription.TitleCaseString().PadRight(44) : portDescription.PadRight(44);

        }
        public static string LookupCountryCode(string airport, IMasterDataStore store)
        {
            var airports = AirportLookup.GetAirports(_cache, store.MastersQueryDb);
            if (string.IsNullOrWhiteSpace(airport)) return new string(' ', 20);

            var airportRec = airports.FirstOrDefault(s => s.Airport.EqualsIgnoreCase(airport));

            return airportRec == null ? new string(' ', 3) : airportRec.CountryCode;

        }

        public static string LookupCountry(IMasterDataStore store, string port, bool isTitleCase, string mode = "", ReportGlobals globals = null)
        {
            port = port.Trim();
            mode = mode.Trim();
            var countryCode = string.Empty;
            var countryName = string.Empty;
            var userLanguage = globals == null ? "EN" : globals.UserLanguage;

            if (string.IsNullOrEmpty(port))
                return new string(' ', 28);
            if (string.IsNullOrEmpty(mode))
                mode = LookupConstants.Achar;

            var railLookup = false;
            var stationNumber = 0;
            if (mode.EqualsIgnoreCase(LookupConstants.Rchar) && !port.Contains(LookupConstants.Echar) && !port.Contains(LookupConstants.Dash))
            {
                railLookup = int.TryParse(port, out stationNumber);
            }

            if (railLookup)
            {
                var railroadStations = RailroadStationLookup.GetRailroadStations(_cache, store.MastersQueryDb);
                var railroadStation = railroadStations.FirstOrDefault(s => s.StationNumber == stationNumber);
                if (railroadStation != null)
                {
                    countryCode = railroadStation.CountryCode;
                }
            }
            else
            {
                var airports = AirportLookup.GetAirports(_cache, store.MastersQueryDb);
                var airport = airports.FirstOrDefault(s => s.Airport == port);
                if (airport != null)
                {
                    countryCode = airport.CountryCode;
                }
            }

            if (!string.IsNullOrEmpty(countryCode))
            {
                var countries = CountriesLookup.GetCountries(_cache, store.MastersQueryDb);
                var country = countries.FirstOrDefault(s => s.CountryCode.EqualsIgnoreCase(countryCode) && s.LanguageCode == userLanguage);
                if (country != null)
                {
                    countryName = country.CountryName.Trim();
                    if (isTitleCase) countryName = countryName.TitleCaseString();
                }
            }

            if (string.IsNullOrEmpty(countryName))
                countryName = string.Format("[Not Found - {0}]", port.Length >= 4 ? port.Substring(0, 4) : port);

            return countryName.PadRight(40);

        }

        public static string LookupCountryName(string countryCode, ReportGlobals globals, IMasterDataStore store)
        {
            var countryName = string.Empty;
            var userLanguage = globals == null ? "EN" : globals.UserLanguage;

            if (string.IsNullOrEmpty(countryCode)) return "[Unknown]".PadRight(40);

            var countries = CountriesLookup.GetCountries(_cache, store.MastersQueryDb);

            var country = countries.FirstOrDefault(s => s.CountryCode.EqualsIgnoreCase(countryCode) && s.LanguageCode == userLanguage);

            if (country != null)
            {
                countryName = country.CountryName;
            }

            if (string.IsNullOrEmpty(countryName)) countryName = "[Not Found]";

            return countryName.PadRight(40);
        }
        
        public static string LookupRegionCode(string airport, string mode, IMasterDataStore store)
        {
            airport = airport.Trim();
            var airports = AirportLookup.GetAirports(_cache, store.MastersQueryDb);
            if (string.IsNullOrWhiteSpace(airport)) return new string(' ', 3);
            if (string.IsNullOrEmpty(mode)) mode = "A";

            //*AGENCIES WHO LOAD RAIL DATA THAT IS NOT THROUGH THE NEW
            //*DATA CLEANSER MAY HAVE A NUMERIC CODE (FROM EVOLVI, FOR INSTANCE).
            //* THE CODE COMES INTO iBank AS "701-VT", AND int(val("701-VT")) RETURNS
            //* THE NUMBER 701.SO, WE HAVE TO FILTER FOR A HYPHEN.
            var rrStation = 0;
            if (mode.Equals("R") && airport.IndexOf("E", StringComparison.OrdinalIgnoreCase) < 0 && airport.IndexOf("-", StringComparison.OrdinalIgnoreCase) < 0)
            {
                rrStation = airport.TryIntParse(0);
            }
            var regionCode = "XXX";
            if (rrStation > 0)
            {
                var railroadStations = RailroadStationLookup.GetRailroadStations(_cache, store.MastersQueryDb);
                var station = railroadStations.FirstOrDefault(s => s.StationNumber == rrStation);
                if (station != null)
                {
                    regionCode = station.RegionCode;
                }
            }
            else
            {
                var airportRec = airports.FirstOrDefault(s => s.Airport == airport);
                if (airportRec != null)
                {
                    regionCode = airportRec.RegionCode;
                }
            }

            return regionCode.PadRight(3, ' ');
        }
        
        public static string LookupRegionName(string regionCode, IMasterDataStore store, bool isTitleCase)
        {
            var worldRegions = WorldRegionsLookup.GetWorldRegions(_cache, store.MastersQueryDb);

            var airports = AirportLookup.GetAirports(_cache, store.MastersQueryDb);
            var airport = airports.FirstOrDefault(s => s.Airport == regionCode.Trim());
            if (airport == null) return $"{regionCode} Region not found";
            var airportRec = airport.RegionCode.Trim();

            var region = worldRegions.FirstOrDefault(s => s.Key.Trim().EqualsIgnoreCase(airportRec) && s.LangCode.Equals("EN"));


            if (region == null) return string.Empty;
            if (isTitleCase) return region.Value.TitleCaseString();

            return region.Value.Trim();
        }

        public static string LookupCountryRegionCode(string countryCode, IMasterDataStore store)
        {
            var countries = CountriesLookup.GetCountries(_cache, store.MastersQueryDb);

            var country = countries.FirstOrDefault(s => s.CountryCode.EqualsIgnoreCase(countryCode));

            return (country != null) ? country.RegionCode.Trim() : "";
        }

        public static string LookupRegion(string port, string mode, IMasterDataStore store, bool isTitleCase)
        {
            port = port.Trim();
            mode = mode.Trim();
            if (string.IsNullOrEmpty(port)) return new string(' ', 20);

            var worldRegions = WorldRegionsLookup.GetWorldRegions(_cache, store.MastersQueryDb);
            if (string.IsNullOrEmpty(mode))
                mode = "A";

            var rrStationNo = 0;

            var railLookup = false;
            if (mode.EqualsIgnoreCase("R") && mode.IndexOf("E", StringComparison.OrdinalIgnoreCase) < 0 && mode.IndexOf("-", StringComparison.OrdinalIgnoreCase) < 0)
            {
                rrStationNo = port.Left(3).TryIntParse(0);
                if (rrStationNo > 0)
                {
                    railLookup = true;
                }
            }

            var regionName = string.Empty;
            if (railLookup)
            {
                var railRoadStations = RailroadStationLookup.GetRailroadStations(_cache, store.MastersQueryDb);
                var rrStation = railRoadStations.FirstOrDefault(s => s.StationNumber == rrStationNo);
                if (rrStation != null)
                {
                    var regionCode = rrStation.RegionCode.Trim();
                    var region = worldRegions.FirstOrDefault(s => s.Key.Equals(regionCode) && s.LangCode.Equals("EN"));
                    if (region != null)
                    {
                        regionName = region.Value;
                    }
                }
            }
            else
            {
                var airports = AirportLookup.GetAirports(_cache, store.MastersQueryDb);
                var airport = airports.FirstOrDefault(s => s.Airport.Equals(port));
                if (airport != null)
                {
                    var regionCode = airport.RegionCode;
                    var region = worldRegions.FirstOrDefault(s => s.Key.Equals(regionCode) && s.LangCode.Equals("EN"));
                    if (region != null)
                    {
                        regionName = region.Value;
                    }
                }
            }

            if (string.IsNullOrEmpty(regionName)) return "[Not Found - " + port + "]";
            if (isTitleCase) return regionName.TitleCaseString().PadRight(20);
            return regionName.PadRight(20);

        }

        public static List<string> LookupAirportRailroadCombo(List<string> items, OrgDestType type, IMasterDataStore store)
        {
            var airportRailroadComboList = RailroadAndAirportComboLookup.GetAirports(_cache, store);

            switch (type)
            {
                case OrgDestType.Port:
                    return airportRailroadComboList.Where(s => items.Contains(s.PortCode)).Select(s => s.PortCode + s.Mode).ToList();
                case OrgDestType.Metro:
                    return airportRailroadComboList.Where(s => items.Contains(s.Metro)).Select(s => s.PortCode + s.Mode).ToList();
                case OrgDestType.Country:
                    return airportRailroadComboList.Where(s => items.Contains(s.CountryCode)).Select(s => s.PortCode + s.Mode).ToList();
                case OrgDestType.Region:
                    return airportRailroadComboList.Where(s => items.Contains(s.RegionCode)).Select(s => s.PortCode + s.Mode).ToList();
            }

            return new List<string>();

        }
        
        /// <summary>
        /// NOTICE: if the data is Railroad (Mode = R), this function returns a Railroad Operator Code. Otherwise, it just returns
        /// whatever it was sent. Left the name alone to avoid confusion in report development. 
        /// </summary>
        /// <param name="getAllRailroadOperatorsQuery"></param>
        /// <param name="airline"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static string LookupAlineCode(IMasterDataStore store, string airline, string mode)
        {
            if (!mode.EqualsIgnoreCase(LookupConstants.Rchar) || airline.Contains(LookupConstants.Echar) || airline.Trim().Length < 3) return airline.PadRight(4);

            int stationNumber;
            if (!int.TryParse(airline, out stationNumber)) return airline.PadRight(4);

            var railroadOperators = RailroadOperatorsLookup.GetRailroadOperators(_cache, store.MastersQueryDb);

            var oper = railroadOperators.FirstOrDefault(s => s.OperatorNumber == stationNumber);

            return oper == null ? string.Empty : oper.OperatorCode.PadRight(4);
        }

        /// <summary>
        /// NOTICE: if the data is Railroad (Mode = R), this function returns a Railroad Operator Code. Otherwise, it just returns
        /// whatever it was sent. Left the name alone to avoid confusion in report development. 
        /// </summary>
        /// <param name="store"></param>
        /// <param name="carrierCode"></param>
        /// <param name="mode"></param>
        /// <param name="isTitleCase"></param>
        /// <returns></returns>
        public static string LookupAline(IMasterDataStore store, string carrierCode, string mode = "", bool isTitleCase = false)
        {
            if (string.IsNullOrEmpty(mode))
                mode = "A";

            carrierCode = carrierCode.Trim();

            if (string.IsNullOrEmpty(carrierCode)) return new string(' ', 30);

            var rrStationNumber = 0;
            var railRoadLookup = false;
            railRoadLookup = int.TryParse(carrierCode, out rrStationNumber);

            if (mode.EqualsIgnoreCase(LookupConstants.Rchar) && carrierCode.Trim().Length > 3 && railRoadLookup) //Rail carrier code
            {
                var railroadOperators = RailroadOperatorsLookup.GetRailroadOperators(_cache, store.MastersQueryDb);
                var rr = railroadOperators.FirstOrDefault(s => s.OperatorNumber.Equals(rrStationNumber));
                return rr == null ? carrierCode + " NOT FOUND" : rr.OperatorName;
            }

            var airlines = AirlineLookup.GetAirlines(_cache, store.MastersQueryDb);
            var airline = mode.EqualsIgnoreCase(LookupConstants.Rchar) || mode.EqualsIgnoreCase(LookupConstants.Achar)
                                   ? airlines.FirstOrDefault(s => s.airline.EqualsIgnoreCase(carrierCode) && s.mode.EqualsIgnoreCase(mode))
                                   : airlines.FirstOrDefault(s => s.airline.EqualsIgnoreCase(carrierCode));

            if (airline == null) return carrierCode + " NOT FOUND";

            if (isTitleCase) return airline.airlndesc.TitleCaseString();
            return airline.airlndesc;
        }

        public static List<string> GetAirlines(IMasterDataStore store, string mode)
        {
            var airlines = AirlineLookup.GetAirlines(_cache, store.MastersQueryDb);
            return airlines.Where(s => s.mode.EqualsIgnoreCase(mode)).Select(s => s.airline.Trim()).ToList();
        }
        
        public static string LookupReportType(int processId, IMasterDataStore store)
        {
            var reportTypes = ReportTypesLookup.GetReportTypes(_cache, store.MastersQueryDb);

            var rptType = reportTypes.FirstOrDefault(s => s.ProcessKey.Equals(processId));
            return rptType == null ? string.Empty : rptType.RptType;
        }

        public static Tuple<int, string> LookupAirline(IMasterDataStore store, string airline)
        {
            var airlines = AirlineLookup.GetAirlines(_cache, store.MastersQueryDb);

            var airlineRec = airlines.FirstOrDefault(s => s.airline.Trim().Equals(airline.Trim(), StringComparison.OrdinalIgnoreCase));
            if (airlineRec == null)
            {
                return new Tuple<int, string>(0, "AIRLINE NOT FOUND");
            }

            return new Tuple<int, string>(airlineRec.airlinenbr ?? 0, airlineRec.airlndesc.Trim().PadRight(30));
        }

        public static Tuple<int, string> LookupAirline(IMasterDataStore store, string airline, string mode)
        {
            var airlines = AirlineLookup.GetAirlines(_cache, store.MastersQueryDb);

            var airlineRec = airlines.FirstOrDefault(s => s.airline.Trim().Equals(airline, StringComparison.OrdinalIgnoreCase) && s.mode == mode);
            if (airlineRec == null)
            {
                if (mode == "R") return new Tuple<int, string>(0, "RAIL NOT FOUND");
                else return new Tuple<int, string>(0, "AIRLINE NOT FOUND");
            }

            return new Tuple<int, string>(airlineRec.airlinenbr ?? 0, airlineRec.airlndesc.Trim().PadRight(30));
        }

        public static string LookupAirlineByNumber(IMasterDataStore store, int airlineNbr)
        {
            var airlines = AirlineLookup.GetAirlines(_cache, store.MastersQueryDb);

            var airlineRec = airlines.FirstOrDefault(s => s.airlinenbr == airlineNbr);
            if (airlineRec == null)
            {
                return "AIRLINE NOT FOUND";
            }

            return airlineRec.airlndesc.Trim().PadRight(30);
        }

        public static string LookupAirlineByNumber(IMasterDataStore store, string airlineNbr)
        {
            int alNbr;
            if (!int.TryParse(airlineNbr, out alNbr))
            {
                return "INVALID AIRLINE NUMBER";
            }
            return LookupAirlineByNumber(store, alNbr);
        }

        public static string LookupCarType(string carType, string langCode, IMasterDataStore store)
        {
            var carTypes = CarLookup.GetCarTypes(_cache, store.MastersQueryDb);

            var carTypeRec = carTypes.FirstOrDefault(s => s.LanguageCode.Equals(langCode, StringComparison.OrdinalIgnoreCase)
                && s.CarType.Equals(carType.Trim(), StringComparison.OrdinalIgnoreCase));

            if (carTypeRec == null)
            {
                return "CAR TYPE NOT FOUND";
            }
            return carTypeRec.Description;
        }

        public static string LookupRoomType(string roomType, string langCode, IMasterDataStore store)
        {
            var roomTypes = RoomTypeLookup.GetRoomTypes(_cache, store.MastersQueryDb);

            var rec = roomTypes.FirstOrDefault(s => s.LanguageCode.Equals(langCode, StringComparison.OrdinalIgnoreCase)
                && s.RoomType.Equals(roomType.Trim(), StringComparison.OrdinalIgnoreCase));

            if (rec == null)
            {
                return roomType.PadRight(10);
            }
            return rec.Description.PadRight(10);
        }

        public static string LookupMasterAgencySource(IMasterDataStore store, string sourceAbbr, string agency)
        {
            var masterAgencySources = MasterAgencySourcesLookup.GetMasterAgencySources(_cache, store.MastersQueryDb);

            var rec = masterAgencySources.FirstOrDefault(s => s.SourceAbbreviation.EqualsIgnoreCase(sourceAbbr) && s.Agency.EqualsIgnoreCase(agency));

            return rec == null ? " NOT FOUND" : rec.SourceDescription;
        }

        /// <summary>
        /// Look up the Account Category Value from acctmast
        /// </summary>
        /// <param name="getAllMasterAccountsQuery"></param>
        /// <param name="account"></param>
        /// <param name="accountCategoryNumber"></param>
        /// <param name="clientFunctions"></param>
        /// <returns></returns>
        public static string LookupAccountCategory(ClientFunctions clientFunctions, IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery, string account, int accountCategoryNumber)
        {
            var retAccountCategory = new string(' ', 20);
            var acct = clientFunctions.GetMasterAccountInfo(getAllMasterAccountsQuery).FirstOrDefault(s => s.AccountId == account);

            if (acct == null) return retAccountCategory;

            switch (accountCategoryNumber)
            {
                case 1:
                    retAccountCategory = acct.AcctCat1;
                    break;
                case 2:
                    retAccountCategory = acct.AcctCat2;
                    break;
                case 3:
                    retAccountCategory = acct.AcctCat3;
                    break;
                case 4:
                    retAccountCategory = acct.AcctCat4;
                    break;
                case 5:
                    retAccountCategory = acct.AcctCat5;
                    break;
            }

            return retAccountCategory;
        }

        public static string LookupAlineNbr(IMasterDataStore store, string airline)
        {
            if (string.IsNullOrEmpty(airline)) return "000";
            
            var airlines = AirlineLookup.GetAirlines(_cache, store.MastersQueryDb);

            var airlineRecord = airlines.FirstOrDefault(s => s.airline.Trim().Equals(airline.Trim(), StringComparison.OrdinalIgnoreCase));
            if (airlineRecord == null) return "000";

            return airlineRecord.airlinenbr.HasValue ? airlineRecord.airlinenbr.ToString().PadLeft(3, '0') : "000";
        }

        public static string LookupSourceDescription(IMasterDataStore store, string sourceAbbrevation, string agency)
        {
            string sourceDescription;
            var masterAgencySources = MasterAgencySourcesLookup.GetMasterAgencySources(_cache, store.MastersQueryDb);

            var source = masterAgencySources.FirstOrDefault(s => s.Agency.EqualsIgnoreCase(agency) && s.SourceAbbreviation == sourceAbbrevation);

            if (source != null)
            {
                sourceDescription = string.IsNullOrEmpty(source.SourceDescription) ? sourceAbbrevation : source.SourceDescription;
            }
            else
            {
                sourceDescription = string.Format(LookupConstants.NotFoundFormat, sourceAbbrevation.Trim());
            }

            return sourceDescription;
        }

        public static string LookupCurrencySymbol(string currencyCode, IMasterDataStore store)
        {
            var currencyLookup = string.Empty;

            if (string.IsNullOrEmpty(currencyCode)) currencyCode = "USD";

            var currencyCountries = CurrencyCountriesLookup.GetCurrencyCountries(_cache, store.MastersQueryDb);

            currencyCode = currencyCode.Trim();

            var currencyRecord = currencyCountries.FirstOrDefault(s => s.CurCode.EqualsIgnoreCase(currencyCode));
            if (currencyRecord != null)
            {
                currencyLookup = currencyRecord.Symbol;
            }

            return currencyLookup.PadRight(4);

        }

        public static string LookupMonth(DateTime date)
        {
            return date.Month.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0');
        }
        public static string LookupMetro(string key, IMasterDataStore store)
        {
            key = key.Trim();
            if (string.IsNullOrEmpty(key))
            {
                return "No Metro Code".PadRight(30);
            }

            var metros = MetrosLookup.GetMetros(_cache, store.MastersQueryDb);
            var metro = metros.FirstOrDefault(s => s.MetroCode.EqualsIgnoreCase(key));
            if (metro != null)
            {
                return string.IsNullOrEmpty(metro.MetroState.Trim())
                    ? metro.MetroCity.Trim().ToUpper()
                    : metro.MetroCity.Trim().ToUpper() + "," + metro.MetroState.Trim().ToUpper();
            }

            //metro not found. Look for airports. 
            var airports = AirportLookup.GetAirports(_cache, store.MastersQueryDb);
            var airport = airports.FirstOrDefault(s => s.Airport.EqualsIgnoreCase(key));
            if (airport != null)
            {
                return string.IsNullOrEmpty(airport.State.Trim())
                    ? airport.City.Trim().ToUpper()
                    : airport.City.Trim().ToUpper() + "," + airport.State.Trim().ToUpper();

            }

            return key + " NOT FOUND";
        }
        public static string LookupAirMetroCode(IMasterDataStore store, string airport, string mode="")
        {
            string metro = string.Empty, code = string.Empty;
            var stationNumber = 0;

            if (string.IsNullOrEmpty(airport))
                return new string(' ', 30);

            mode = string.IsNullOrEmpty(mode) ? LookupConstants.Achar : mode;

            var railLookup = false;
            if (mode.EqualsIgnoreCase(LookupConstants.Rchar) && !airport.Contains(LookupConstants.Echar) && !airport.Contains(LookupConstants.Dash))
            {
                railLookup = int.TryParse(airport, out stationNumber);
            }

            if (railLookup)
            {
                var railRoadStations = RailroadStationLookup.GetRailroadStations(_cache, store.MastersQueryDb);
                var station = railRoadStations.FirstOrDefault(s => s.StationNumber == stationNumber);
                if (station != null)
                {
                    metro = !string.IsNullOrEmpty(station.Metro) ? station.Metro.Trim().ToUpper() : "";
                }
            }
            else
            {
                var airports = AirportLookup.GetAirports(_cache, store.MastersQueryDb);
                var airportLookup = airports.FirstOrDefault(s => s.Airport.EqualsIgnoreCase(airport));
                if (airportLookup != null)
                {
                    metro = !string.IsNullOrEmpty(airportLookup.Metro) ? airportLookup.Metro.Trim().ToUpper() : "";
                }
            }

            if (!string.IsNullOrEmpty(metro))
            {
                var metros = MetrosLookup.GetMetros(_cache, store.MastersQueryDb);
                var metroCode = metros.FirstOrDefault(s => s.MetroCode.EqualsIgnoreCase(metro));

                if (metroCode != null)
                {
                    code = metroCode.MetroCode.ToUpper();
                }
            }

            return code;
        }

        public static string LookupAirMetro(IMasterDataStore store, string airport, string cityState, bool isTitleCaseRequired, string mode = "")
        {
            var result = LookupAirMetro(store, airport, cityState, mode);
            if (isTitleCaseRequired) return result.TitleCaseString();
            return result;
        }

        public static string LookupAirMetro(IMasterDataStore store, string airport, string cityState, string mode = "")
        {
            string metro = string.Empty, city = string.Empty, state = string.Empty, lookupValue;
            var stationNumber = 0;

            if (string.IsNullOrEmpty(airport))
                return new string(' ', 30);

            mode = string.IsNullOrEmpty(mode) ? LookupConstants.Achar : mode;

            var railLookup = false;
            if (mode.EqualsIgnoreCase(LookupConstants.Rchar) && !airport.Contains(LookupConstants.Echar) && !airport.Contains(LookupConstants.Dash))
            {
                railLookup = int.TryParse(airport, out stationNumber);
            }

            if (railLookup)
            {
                var railRoadStations = RailroadStationLookup.GetRailroadStations(_cache, store.MastersQueryDb);
                var station = railRoadStations.FirstOrDefault(s => s.StationNumber == stationNumber);
                if (station != null)
                {
                    metro = !string.IsNullOrEmpty(station.Metro) ? station.Metro.Trim().ToUpper() : "";
                    city = !string.IsNullOrEmpty(station.City) ? station.City.Trim().ToUpper() : "";
                    state = !string.IsNullOrEmpty(station.State) ? station.State.Trim().ToUpper() : "";
                }
            }
            else
            {
                var airports = AirportLookup.GetAirports(_cache, store.MastersQueryDb);
                var airportLookup = airports.FirstOrDefault(s => s.Airport.EqualsIgnoreCase(airport));
                if (airportLookup != null)
                {
                    metro = !string.IsNullOrEmpty(airportLookup.Metro) ? airportLookup.Metro.Trim().ToUpper() : "";
                    city = !string.IsNullOrEmpty(airportLookup.City) ? airportLookup.City.Trim().ToUpper() : "";
                    state = !string.IsNullOrEmpty(airportLookup.State) ? airportLookup.State.Trim().ToUpper() : "";
                }
            }
            
            if (!string.IsNullOrEmpty(metro))
            {
                var metros = MetrosLookup.GetMetros(_cache, store.MastersQueryDb);
                var metroCode = metros.FirstOrDefault(s => s.MetroCode.EqualsIgnoreCase(metro));

                if (metroCode != null)
                {
                    city = !string.IsNullOrEmpty(metroCode.MetroCity) ? metroCode.MetroCity.Trim().ToUpper() : "";
                    state = !string.IsNullOrEmpty(metroCode.MetroState) ? metroCode.MetroState.Trim().ToUpper() : "";
                }
            }

            if (!string.IsNullOrEmpty(cityState) && cityState.ToUpper() == "S")
            {
                lookupValue = state;
            }
            else
            {
                lookupValue = city;
            }

            lookupValue = string.IsNullOrEmpty(lookupValue) ? string.Format("{0} NOT FOUND", airport) : lookupValue;

            return lookupValue.PadRight(30);

        }

        public static string LookupChains(string chainCode, IMasterDataStore store)
        {
            string lookupValue;
            if (string.IsNullOrEmpty(chainCode))
            {
                lookupValue = "No Hotel Chain Code";
                return lookupValue.PadRight(24);
            }

            var chains = ChainsLookup.GetChains(_cache, store.MastersQueryDb);

            var chain = chains.FirstOrDefault(s => s.ChainCode.EqualsIgnoreCase(chainCode.Trim()));
            lookupValue = chain != null ? chain.ChainDescription : string.Format("{0} NOT FOUND", chainCode);

            return lookupValue.PadRight(24);
        }

        public static string LookupChangeDescirption(int changeCode, string userLanguage, IMasterDataStore store)
        {
            var tripChangeCodes = TripChangeInformationLookup.GetTripChangeCodeInformation(_cache, store.MastersQueryDb);

            var tripChangeCode = tripChangeCodes.FirstOrDefault(s => s.ChangeCode == changeCode && s.LanguageCode.EqualsIgnoreCase(userLanguage));

            string lookupValue = (tripChangeCode != null)
                ? tripChangeCode.CodeDescription
                : string.Format("{0} NOT FOUND", changeCode.ToString(CultureInfo.InvariantCulture));

            return lookupValue.PadRight(50);
        }

        public static string LookupChangeDescirption(int changeCode, string changeCodeDescription, string changeFrom, string changeTo, string priorItinerary, ReportGlobals globals, IMasterDataStore store)
        {
            var tripChangeInfo = TripChangeInformationLookup.GetTripChangeCodeInformation(_cache, store.MastersQueryDb);

            var tripChangeCodes = tripChangeInfo.Where(s => s.LanguageCode.EqualsIgnoreCase(globals.UserLanguage));

            if (changeCodeDescription.EqualsIgnoreCase("LOOKITUP"))
            {
                var desc = tripChangeCodes.FirstOrDefault(s => s.ChangeCode == changeCode);
                changeCodeDescription = desc != null ? desc.CodeDescription.PadRight(50) : string.Format("{0} NOT FOUND", changeCode).PadRight(50);
            }

            string returnValue;

            if (changeCode == 202)
            {
                returnValue = "ITINERARY CHANGED - " + priorItinerary.Trim();
            }
            else if (changeCodeDescription.ToUpper().Contains("TRAVELER INFO CHANGE -"))
            {
                returnValue = changeCodeDescription.Replace("Traveler Info Change -", "") + " from " + changeFrom.Trim() +
                              " to " + changeTo.Trim();
            }
            else if (changeCodeDescription.ToUpper().Contains("TRIP CHANGE -"))
            {
                returnValue = changeCodeDescription.Replace("Trip Change -", "") + " from " + changeFrom.Trim() +
                              " to " + changeTo.Trim();
            }
            else if (changeCodeDescription.ToUpper().Contains("SEGMENT CHANGE -"))
            {
                returnValue = changeCodeDescription.Replace("Segment Change -", "") + " from " + changeFrom.Trim() +
                              " to " + changeTo.Trim();
            }
            else if (changeCodeDescription.ToUpper().Contains("HOTEL CHANGE -"))
            {
                returnValue = changeCodeDescription.Replace("Hotel Change -", "") + " from " + changeFrom.Trim() +
                              " to " + changeTo.Trim();
            }
            else if (changeCodeDescription.ToUpper().Contains("CAR CHANGE -"))
            {
                returnValue = changeCodeDescription.Replace("Car Change -", "") + " from " + changeFrom.Trim() +
                              " to " + changeTo.Trim();
            }
            else
            {
                returnValue = changeCodeDescription + " from " + changeFrom.Trim() +
                              " to " + changeTo.Trim();
            }

            return returnValue.PadRight(200);

        }

        public static string LookupLegRoute(List<LegSegInfo> recs, int reckey)
        {
            var route = recs.FirstOrDefault(s => s.RecKey == reckey);
            if (route != null) return route.DerivedLegRouting;

            return string.Empty;
        }

        public static string LookupSegRoute(List<LegSegInfo> recs, int reckey)
        {
            var route = recs.FirstOrDefault(s => s.RecKey == reckey);
            if (route != null) return route.DerivedSegRouting;

            return string.Empty;
        }

        public static string LookupTransId(List<LegSegInfo> recs, int reckey)
        {
            var rec = recs.FirstOrDefault(s => s.RecKey == reckey);
            if (rec != null) return rec.DerivedTransId;

            return string.Empty;
        }

        public static string LookupSegRouteDeprecate(List<LegSegInfo> segs, int reckey)
        {
            var route = segs.FirstOrDefault(s => s.RecKey == reckey);
            if (route != null) return route.LegRouting;

            return string.Empty;
        }

        public static string LookupSegRouteCarriers(List<LegSegInfo> segs, int reckey)
        {
            var route = segs.FirstOrDefault(s => s.RecKey == reckey);
            if (route != null) return route.Carriers;

            return string.Empty;
        }

        public static string LookupSegRouteClasses(List<LegSegInfo> segs, int reckey)
        {
            var route = segs.FirstOrDefault(s => s.RecKey == reckey);
            if (route != null) return route.Classes;

            return string.Empty;
        }

        public static string LookupSegRouteFbCodes(List<LegSegInfo> segs, int reckey)
        {
            var route = segs.FirstOrDefault(s => s.RecKey == reckey);
            if (route != null) return route.FbCodes;

            return string.Empty;
        }

        public static string LookupSegRouteClassCats(List<LegSegInfo> segs, int reckey)
        {
            var route = segs.FirstOrDefault(s => s.RecKey == reckey);
            if (route != null) return route.ClassCats;

            return string.Empty;
        }

        public static string LookupSegRouteFirstDestination(List<LegSegInfo> segs, int reckey)
        {
            //first item after the first space

            var route = segs.FirstOrDefault(s => s.RecKey == reckey);
            if (route == null) return string.Empty;

            var stops = route.LegRouting.Split(' ');

            return stops.Length > 1 ? stops[1] : string.Empty;
        }

        public static string LookupSegRouteFirstOrigin(List<LegSegInfo> segs, int reckey)
        {
            //first item after the first space

            var route = segs.FirstOrDefault(s => s.RecKey == reckey);
            if (route == null) return string.Empty;

            var stops = route.LegRouting.Split(' ');

            return stops.Length > 0 ? stops[0] : string.Empty;
        }

        public static int LookupTripMiles(List<LegSegInfo> segs, int reckey)
        {
            var route = segs.FirstOrDefault(s => s.RecKey == reckey);
            if (route != null) return route.Miles;

            return 0;
        }
        //TODO: need to use (h)ibTripDerivedData DerivedTripClass when verified
        public static string LookupTripClass(List<LegSegInfo> segs, int reckey)
        {
            var route = segs.FirstOrDefault(s => s.RecKey == reckey);
            if (route != null) return route.TripClass;

            return string.Empty;
        }

        //TODO: need to use (h)ibTripDerivedData DerivedTripClassCat when verified
        public static string LookupTripClassCat(List<LegSegInfo> segs, int reckey)
        {
            var route = segs.FirstOrDefault(s => s.RecKey == reckey);
            if (route != null) return route.TripClassCat;

            return string.Empty;
        }

        public static string LookupRoundTrip(List<LegSegInfo> segs, int reckey)
        {
            var route = segs.FirstOrDefault(s => s.RecKey == reckey);
            if (route != null) return route.RoundTrip ? "YES" : "NO";

            return string.Empty;
        }

        public static string LookupTripClassCatCode(List<LegSegInfo> segs, int reckey)
        {
            var route = segs.FirstOrDefault(s => s.RecKey == reckey);
            if (route != null) return route.TripClassCat;

            return string.Empty;
        }

        public static string LookupHomeCountryCode(string sourceAbbr, ReportGlobals globals, IMasterDataStore store)
        {
            var homeCountries = HomeCountriesLookup.GetHomeCountries(_cache, store.MastersQueryDb, globals.ClientType, globals.Agency);

            var homeCountry = homeCountries.FirstOrDefault(s => s.Key.EqualsIgnoreCase(sourceAbbr));

            return homeCountry == null ? "[none]" : homeCountry.Value.PadRight(3);

        }

        public static string LookupHomeCountry(string countryCode, ReportGlobals globals, IMasterDataStore store)
        {
            var homeCountries = HomeCountriesLookup.GetHomeCountries(_cache, store.MastersQueryDb, globals.ClientType, globals.Agency);

            var homeCountry = homeCountries.FirstOrDefault(s => s.Value.EqualsIgnoreCase(countryCode.Trim()));

            return homeCountry == null ? string.Empty : homeCountry.Key.Trim();

        }

        public static string LookupHomeCountryName(string sourceAbbr, ReportGlobals globals, IMasterDataStore store)
        {
            var homeCountries = HomeCountriesLookup.GetHomeCountries(_cache, store.MastersQueryDb, globals.ClientType, globals.Agency);

            var homeCountry = homeCountries.FirstOrDefault(s => s.Key.EqualsIgnoreCase(sourceAbbr));
            if (homeCountry != null)
            {
                var countries = CountriesLookup.GetCountries(_cache, store.MastersQueryDb);
                var country = countries.FirstOrDefault(s => s.CountryCode.EqualsIgnoreCase(homeCountry.Value) && s.LanguageCode.EqualsIgnoreCase(globals.UserLanguage));
                if (country != null)
                {
                    return country.CountryName.PadRight(40);
                }
            }

            return "[none]";
        }

        public static string LookupUdid(List<UdidInformation> udids, int recKey, int udidNumber, int padWidth)
        {
            var udid = udids.FirstOrDefault(s => s.RecKey == recKey && s.UdidNumber == udidNumber);

            return (udid != null) ? udid.UdidText.PadRight(padWidth) : new string(' ', padWidth);
        }

        public static string LookupStateName(string stateCode, IMasterDataStore store)
        {
            var stateNames = StateNamesLookup.GetStatesNames(_cache, store.MastersQueryDb);

            var state = stateNames.FirstOrDefault(s => s.abbreviation.EqualsIgnoreCase(stateCode));

            return (state != null) ? state.name : stateCode;
        }
        
        public static string LookupEquipment(string key, IMasterDataStore store)
        {
            if (string.IsNullOrEmpty(key)) return string.Empty;

            var equipments = EquipmentsLookup.GetEquipment(_cache, store.MastersQueryDb);

            var equipment = equipments.FirstOrDefault(s => s.Key.EqualsIgnoreCase(key));

            return equipment != null ? equipment.Value.Trim() : key.Trim();
        }

        public static string LookupVendorType(List<VendorTypeInformation> vendors, int recKey)
        {
            var vendorTypes = vendors.Where(s => s.RecKey == recKey).Select(s => s.VendorType);

            return string.Join(",", vendorTypes);
        }

        public static string LookupMetroCode(IMasterDataStore store, string port, string mode = "")
        {
            if (string.IsNullOrEmpty(mode)) mode = LookupConstants.Achar;

            if (string.IsNullOrEmpty(port)) return new string(' ', 3);
            
            var airportRailroadComboList = RailroadAndAirportComboLookup.GetAirports(_cache, store);

            var item = airportRailroadComboList.FirstOrDefault(s => s.PortCode.EqualsIgnoreCase(port) && s.Mode.EqualsIgnoreCase(mode));

            return (item != null && !string.IsNullOrEmpty(item.Metro)) ? item.Metro : string.Empty;

        }

        public static string LookupAirportCountryCode(IMasterDataStore store, string port, string mode = "")
        {
            if (string.IsNullOrEmpty(mode)) mode = LookupConstants.Achar;

            if (string.IsNullOrEmpty(port)) return new string(' ', 3);

            var airportRailroadComboList = RailroadAndAirportComboLookup.GetAirports(_cache, store);

            var item = airportRailroadComboList.FirstOrDefault(s => s.PortCode.EqualsIgnoreCase(port) && s.Mode.EqualsIgnoreCase(mode));

            return (item != null && !string.IsNullOrEmpty(item.CountryCode)) ? item.CountryCode : string.Empty;
        }

        public static string LookupAirportState(IMasterDataStore store, string port, string mode = "")
        {
            if (string.IsNullOrEmpty(mode)) mode = LookupConstants.Achar;

            if (string.IsNullOrEmpty(port)) return new string(' ', 20);

            var airportRailroadComboList = RailroadAndAirportComboLookup.GetAirports(_cache, store);

            var item = airportRailroadComboList.FirstOrDefault(s => s.PortCode.EqualsIgnoreCase(port) && s.Mode.EqualsIgnoreCase(mode));

            return (item != null && !string.IsNullOrEmpty(item.CountryCode)) ? item.State : string.Empty;
        }

        public static string LookupCityPair(string cityPair, string mode, string cityOrName, IMasterDataStore store, bool isTitleCase = false)
        {
            if (cityPair == null) return "";
            //Split the city pair
            if (cityPair.IndexOf('-') < 0)
                return cityPair;

            string originName, destinationName;

            var splitValues = cityPair.Trim().Split('-');
            var origin = splitValues[0];
            var destination = splitValues[1];

            if (cityOrName.EqualsIgnoreCase("CITY") || cityOrName.EqualsIgnoreCase("CITYDESC"))
            {
                originName = LookupAportCity(origin, store, isTitleCase, mode).Trim();
                destinationName = LookupAportCity(destination, store, isTitleCase, mode).Trim();
            }
            else
            {
                if (cityOrName.EqualsIgnoreCase("METRO"))
                {
                    originName = LookupMetroCode(store, origin, mode).Trim();
                    destinationName = LookupMetroCode(store, destination, mode).Trim();
                }
                else
                {
                    originName = LookupAirportName(origin, mode, store, isTitleCase).Trim();
                    destinationName = LookupAirportName(destination, mode, store, isTitleCase).Trim();
                }
            }
            
            if (cityOrName.EqualsIgnoreCase("CITYDESC"))
            {
                originName += " (" + origin.Trim() + ")";
                destinationName += " (" + destination.Trim() + ")";
            }

            return string.Format("{0}-{1}", originName, destinationName);
        }

        //TODO: GSA - find more elegant solution
        public static string LookupCityPair_GSA(string cityPair, string mode, string cityOrName, IMasterDataStore store, bool isTitleCase)
        {
            if (string.IsNullOrEmpty(cityPair)) return string.Empty;

            //Split the city pair
            if (cityPair.IndexOf('-') < 0)
                return cityPair;

            string originName, destinationName;

            var splitValues = cityPair.Trim().Split('-');
            var origin = splitValues[0];
            var destination = splitValues[1];

            if (cityOrName.EqualsIgnoreCase("CITY") || cityOrName.EqualsIgnoreCase("CITYDESC"))
            {
                originName = LookupAportName(origin, store, isTitleCase, mode).Trim();
                destinationName = LookupAportName(destination, store, isTitleCase, mode).Trim();
            }
            else
            {
                if (cityOrName.EqualsIgnoreCase("METRO"))
                {
                    originName = LookupMetroCode(store, origin, mode).Trim();
                    destinationName = LookupMetroCode(store, destination, mode).Trim();
                }
                else
                {
                    originName = LookupAirportName(origin, mode, store, isTitleCase).Trim();
                    destinationName = LookupAirportName(destination, mode, store, isTitleCase).Trim();
                }
            }
            
            //this is the part that is different for GSA
            if (cityOrName.EqualsIgnoreCase("CITYDESC"))
            {
                originName += " Airport";
                destinationName += " Airport";
            }

            return string.Format("{0}-{1}", originName, destinationName);
        }

        public static string LookUpCityPairCode(string cityPair, string domintl, IMasterDataStore store)
        {
            if (string.IsNullOrEmpty(cityPair)) return string.Empty;
            if (domintl == "D") return cityPair;

            //international use Metrol
            if (cityPair == null) return "";
            //Split the city pair
            if (cityPair.IndexOf('-') < 0)
                return cityPair;

            var splitValues = cityPair.Split('-');
            var origin = splitValues[0];
            var destination = splitValues[1];

            origin = LookupMetroCodeByAirportCode(origin, store);
            destination = LookupMetroCodeByAirportCode(destination, store);

            return string.Format("{0}-{1}", origin, destination);
        }

        public static string LookUpCityPairCode_GSA(string cityPair, string domintl, IMasterDataStore store)
        {
            if (string.IsNullOrEmpty(cityPair)) return string.Empty;
            if (domintl == "D") return cityPair;

            //international use Metrol
            if (cityPair == null) return "";
            //Split the city pair
            if (cityPair.IndexOf('-') < 0)
                return cityPair;

            var splitValues = cityPair.Split('-');
            var origin = splitValues[0];
            var destination = splitValues[1];

            origin = LookupMetroCodeByAirportCode(origin, store, false);
            destination = LookupMetroCodeByAirportCode(destination, store, false);

            if (origin.Equals(string.Empty)) origin = LookupConstants.UnknownAirport;
            if (destination.Equals(string.Empty)) destination = LookupConstants.UnknownAirport;

            return (origin.CompareTo(destination) > 0) ? string.Format($"{destination}-{origin}"): string.Format($"{origin}-{destination}");
        }
        public static string LookupMetroCodeByAirportCode(string airport, IMasterDataStore store, bool hasNotFound = true)
        {
            var airports = AirportLookup.GetAirports(_cache, store.MastersQueryDb);

            var rec = airports.FirstOrDefault(s => s.Airport.Trim() == airport.Trim());
            var strNotFound = hasNotFound ? " not found metro" : "";
            return rec != null ? rec.Metro : $"{airport.Trim()}{strNotFound}";
        }

        public static string LookupCityPairDescription(string cityPair, string domintl, IMasterDataStore store, bool isTitleCase)
        {
            if (cityPair == null) return "";
            //Split the city pair
            if (cityPair.IndexOf('-') < 0)
                return cityPair;

            string originName, destinationName;

            var splitValues = cityPair.Split('-');
            var origin = splitValues[0];
            var destination = splitValues[1];

            originName = LookupAportCity(origin, store, isTitleCase, "A").Trim();
            if (isTitleCase) originName = originName.TitleCaseString();
            originName += ((domintl == "D") ? " (" + origin.Trim() + ")" : "");

            destinationName = LookupAportCity(destination, store, isTitleCase, "A").Trim();
            if (isTitleCase) destinationName = destinationName.TitleCaseString();
            destinationName += ((domintl == "D") ? " (" + destination.Trim() + ")" : "");

            return string.Format("{0}-{1}", originName, destinationName);
        }

        public static string LookupOrgCountryToDestCountry(string origin, string destination, bool isTitleCase, IMasterDataStore store, ReportGlobals globals = null)
        {
            var airports = AirportLookup.GetAirports(_cache, store.MastersQueryDb);
            var airport = airports.FirstOrDefault(s => s.Airport == origin.Trim());
            if (airport == null) return $"{origin} Country not found";
            var originCountryCode = airport.CountryCode.Trim();

            airport = airports.FirstOrDefault(s => s.Airport == destination.Trim());
            if (airport == null) return $"{destination} Country not found";
            var destinationCountryCode = airport.CountryCode.Trim();

            var originName = LookupCountryName(originCountryCode, globals, store).Trim();
            var destinationName = LookupCountryName(destinationCountryCode, globals, store).Trim();

            originName = isTitleCase ? originName.TitleCaseString() : originName;
            destinationName = isTitleCase ? destinationName.TitleCaseString() : destinationName;

            return string.Format($"{originName}-{destinationName}");
        }
        public static string LookupOrgContinentToDestContinent(string origin, string destination, bool isTitleCaseRequired, IMasterDataStore store)
        {
            var originName = LookupRegionName(origin.Trim(), store, isTitleCaseRequired);
            var destinationName = LookupRegionName(destination.Trim(), store, isTitleCaseRequired);

            return  string.Format($"{originName}-{destinationName}");
        }

        public static string LookupCityPairCountryToCountry(string cityPair, ReportGlobals globals, IMasterDataStore store)
        {
            if (cityPair == null || cityPair == "") return string.Empty;
            var splitValues = cityPair.Trim().Split('-');
            var origin = splitValues[0];
            var destination = splitValues[1];

            var airports = AirportLookup.GetAirports(_cache, store.MastersQueryDb);
            var airport = airports.FirstOrDefault(s => s.Airport == origin);
            if (airport == null) return $"{origin} Country not found";
            var originCountryCode = airport.CountryCode.Trim();

            airport = airports.FirstOrDefault(s => s.Airport == destination);
            if (airport == null) return $"{destination} Country not found";
            var destinationCountryCode = airport.CountryCode.Trim();

            var originName = LookupCountryName(originCountryCode, globals, store).Trim();
            var destinationName = LookupCountryName(destinationCountryCode, globals, store).Trim();

            return (originName.CompareTo(destinationName) > 0) ? string.Format($"{originName}-{destinationName}") : string.Format($"{destinationName}-{originName}");
        }

        //trip level
        public static string LookupCityPairRegionToRegion(string cityPair, IMasterDataStore store, bool isTitleCaseRequired)
        {
            if (cityPair == null || cityPair == "") return string.Empty;

            var splitValues = cityPair.Trim().Split('-');
            var origin = splitValues[0];
            var destination = splitValues[1];

            var originName = LookupRegionName(origin, store, isTitleCaseRequired);
            var destinationName = LookupRegionName(destination, store, isTitleCaseRequired);

            return (originName.CompareTo(destinationName) > 0) ? string.Format($"{originName}-{destinationName}") : string.Format($"{destinationName}-{originName}");
        }
        
        public static string LookupTwoDateGroup(DateTime date1, DateTime date2)
        {
            var timespan = (date2 - date1);
            var days = Convert.ToInt32(timespan.TotalDays);

            if (days.IsBetween(0, 2)) return "0-2 Days";
            if (days.IsBetween(3, 6)) return "3-6 Days";
            if (days.IsBetween(7, 13)) return "7-13 Days";
            if (days.IsBetween(14, 20)) return "14-20 Days";
            return "21+ Days";
        }

        public static string LookupAirportCountryCodeByMetro(string metro, IMasterDataStore store)
        {
            if (string.IsNullOrEmpty(metro))
                return new string(' ', 20);

            metro = metro.Trim();

            var airports = AirportLookup.GetAirports(_cache, store.MastersQueryDb);

            var countryCode = string.Empty;

            var airport = airports.FirstOrDefault(s => s.Metro.EqualsIgnoreCase(metro));
            if (airport != null)
            {
                if (airport.CountryCode != null) countryCode = airport.CountryCode.Trim().ToUpper();
            }
            else
            {
                airport = airports.FirstOrDefault(s => s.Airport.EqualsIgnoreCase(metro));
                if (airport == null) return countryCode.PadRight(20);
                if (airport.CountryCode != null) countryCode = airport.CountryCode.Trim().ToUpper();
            }

            return countryCode.PadRight(20);
        }

        public static List<TripChangeCodeInformation> GetAllTripChangeCodes(IMasterDataStore store)
        {
            return TripChangeInformationLookup.GetTripChangeCodeInformation(_cache, store.MastersQueryDb);
        }
    }
}
