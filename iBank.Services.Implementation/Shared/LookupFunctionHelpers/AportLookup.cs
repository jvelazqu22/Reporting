using System.Linq;
using Domain.Constants;
using Domain.Services;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities;

namespace iBank.Services.Implementation.Shared.LookupFunctionHelpers
{
    public static class AportLookup
    {
        private static readonly ICacheService _cache = new CacheService();

        public static string LookupAport(IMasterDataStore store, string inPort, string inMode, string agency, string airline = "")
        {
            inPort = inPort.Trim();
            inMode = inMode.Trim();
            if (string.IsNullOrEmpty(inPort)) return new string(' ', 28);
            string portDescription;

            if (string.IsNullOrEmpty(inMode)) inMode = LookupConstants.Achar;

            var rrStationNumber = 0;
            var railRoadLookup = false;
            if (inMode.EqualsIgnoreCase(LookupConstants.Rchar) && !inPort.Contains(LookupConstants.Echar) && !inPort.Contains(LookupConstants.Dash))
            {
                railRoadLookup = int.TryParse(inPort, out rrStationNumber);
            }

            //TODO: There is a comment here that indicates an odd exception in the logic for a program called ibPrevLoad:
            //** 09/17/2009 - PBLM: THIS FUNCTION IS CALLED BY THE CHANGE MGMT ROUTINE IN **
            //** ibPrevLoad, AND ibPrevLoad DOES NOT HANDLE THE RAIL CURSOR -- IT HAS ITS **
            //** OWN ROUTINE FOR CREATING THE curAirport CURSOR.  WE'RE GOING TO MAKE AN  **
            //** ASSUMPTION HERE, THAT IF THE curRRStations CURSOR IS NOT OPEN, THEN WE   **
            //** ARE NOT GOING TO DEAL WITH RAIL. 
            if (railRoadLookup)
            {
                var railroadStations = RailroadStationLookup.GetRailroadStations(_cache, store.MastersQueryDb);
                var railroadStation = railroadStations.FirstOrDefault(s => s.StationNumber == rrStationNumber);
                if (railroadStation == null)
                {
                    portDescription = string.Format(LookupConstants.NotFound, inPort);
                }
                else
                {
                    portDescription = railroadStation.CountryCode.EqualsIgnoreCase(LookupConstants.Canada) || railroadStation.CountryCode.EqualsIgnoreCase(LookupConstants.Usa)
                        ? railroadStation.StationName.Trim() + LookupConstants.Comma + railroadStation.State
                        : railroadStation.StationName.Trim() + LookupConstants.Comma + railroadStation.CountryCode;
                }
            }
            else
            {
                /*
                    if the data is coming from the iblegs table the inPort might have a '-' in it
                    if the data is coming from the ibmktsegs table the inPort won't have a '-' in it
                    but, to look up a railroad in the airports table, the airport column expects the inPort to have a '-' when the mode = 'R'
                    so, if we get this far, and the inPort isn't coming from iblegs (doesn't have a '-'), need to concat inPort-airline
                */
                inPort = ReportHelper.CreateOriginDestCode(inPort, inMode, airline);
                var airports = AirportLookup.GetAirports(_cache, store.MastersQueryDb);
                var airport = airports.FirstOrDefault(s => s.Airport.Trim() == inPort.Trim());
                if (airport == null)
                {
                    portDescription = string.Format(LookupConstants.NotFound, inPort);
                }
                else
                {
                    portDescription = string.IsNullOrEmpty(airport.State) || agency.Equals("GSA")
                        ? airport.City.Trim()
                        : airport.City.Trim() + LookupConstants.Comma + airport.State;
                }
            }
            return portDescription;
        }
    }
}
