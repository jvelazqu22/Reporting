using iBank.Services.Implementation.Classes;
using System.Collections.Generic;
using System.Linq;

using Domain.Models.ReportPrograms.UserDefinedReport;

using iBank.Services.Implementation.Shared;
using iBank.Server.Utilities.Classes;
using iBank.Repository.SQL.Interfaces;
using iBank.Repository.SQL.Repository;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers
{
    public class LocateTravelerManager
    {
        private readonly RoutingCriteria _routingCriteria;
        private readonly UserDefinedParameters _userDefinedParams;
        private readonly ReportGlobals _globals;
        private readonly IMasterDataStore _masterStore;

        public LocateTravelerManager(UserDefinedParameters userDefinedParams, RoutingCriteria routingCriteria, ReportGlobals globals)
        {
            _userDefinedParams = userDefinedParams;
            _routingCriteria = routingCriteria;
            _globals = globals;
            _masterStore = new MasterDataStore();
        }
        public void LocateTraveler(bool bExcludeTravesArrivingHome)
        {
            var recToDelete = new List<RawData>();
            foreach (var trip in _userDefinedParams.TripDataList)
            {
                var trip1 = trip;

                var numLegsSelected = 0;
                var firstLegOrigin = string.Empty;
                var lastLegDestination = string.Empty;
                var lastLegMode = string.Empty;

                var matchedOnList = string.Empty;


                var legs = _userDefinedParams.LegDataList.Where(s => s.RecKey == trip1.RecKey);
                var lastMatchedOn = string.Empty;
                var legSelected = true;
                foreach (var leg in legs)
                {
                    var legMatched = false;
                    var matchedOn = string.Empty;

                    if (string.IsNullOrEmpty(firstLegOrigin))
                    {
                        firstLegOrigin = leg.Origin.Trim();
                    }
                    lastLegDestination = leg.Destinat.Trim();
                    lastLegMode = leg.Mode.Trim();
                    //is the date of travel within the "on the road" date range? 
                    if (_globals.EndDate != null && (_globals.BeginDate != null && (leg.RDepDate < _globals.BeginDate.Value || leg.RArrDate > _globals.EndDate.Value)))
                    {
                        legSelected = false;
                    }

                    if (legSelected)
                    {
                        var origin = leg.Origin.Trim();
                        var destination = leg.Destinat.Trim();
                        var originGo = false;
                        var destinationGo = false;

                        if (_routingCriteria.NoRoutingCriteria)
                        {
                            originGo = true;
                            destinationGo = true;
                        }

                        if (_routingCriteria.OriginsContains(origin) || _routingCriteria.DestinationsContains(origin))
                        {
                            originGo = true;
                            legMatched = true;
                            matchedOn = origin;
                        }
                        if (_routingCriteria.OriginsContains(destination) || _routingCriteria.DestinationsContains(destination))
                        {
                            originGo = true;
                            legMatched = true;
                            matchedOn = origin;
                        }


                        //Check country
                        if (_routingCriteria.HasCountry)
                        {
                            var countryCode = LookupFunctions.LookupCountryCode(origin, _masterStore);
                            if (_routingCriteria.OriginCountriesContains(countryCode))
                            {
                                originGo = true;
                                legMatched = true;
                                matchedOn = countryCode;
                            }

                            if (_routingCriteria.DestinationCountriesContains(countryCode))
                            {
                                destinationGo = true;
                                legMatched = true;
                                matchedOn = countryCode;
                            }

                            countryCode = LookupFunctions.LookupCountryCode(destination, _masterStore);
                            if (_routingCriteria.OriginCountriesContains(countryCode))
                            {
                                originGo = true;
                                legMatched = true;
                                matchedOn = countryCode;
                            }

                            if (_routingCriteria.DestinationCountriesContains(countryCode))
                            {
                                destinationGo = true;
                                legMatched = true;
                                matchedOn = countryCode;
                            }
                        }

                        //Check region
                        if (_routingCriteria.HasCountry)
                        {
                            var regionCode = LookupFunctions.LookupRegionCode(origin, leg.Mode, _masterStore);
                            if (_routingCriteria.OriginCountriesContains(regionCode))
                            {
                                originGo = true;
                                legMatched = true;
                                matchedOn = regionCode;
                            }

                            if (_routingCriteria.DestinationCountriesContains(regionCode))
                            {
                                destinationGo = true;
                                legMatched = true;
                                matchedOn = regionCode;
                            }

                            regionCode = LookupFunctions.LookupRegionCode(destination, leg.Mode, _masterStore);
                            if (_routingCriteria.OriginCountriesContains(regionCode))
                            {
                                originGo = true;
                                legMatched = true;
                                matchedOn = regionCode;
                            }

                            if (_routingCriteria.DestinationCountriesContains(regionCode))
                            {
                                destinationGo = true;
                                legMatched = true;
                                matchedOn = regionCode;
                            }
                        }
                        if (!originGo && !destinationGo)
                        {
                            legSelected = false;
                        }
                    }

                    if (legSelected)
                    {
                        numLegsSelected++;
                    }

                    if (legMatched)
                    {
                        matchedOnList += "," + matchedOn;
                        lastMatchedOn = matchedOn;
                    }
                }

                //"exclude travelers arriving home" option. 
                if (numLegsSelected > 0)
                {
                    if (bExcludeTravesArrivingHome)
                    {
                        matchedOnList = matchedOnList.Replace("," + lastMatchedOn, string.Empty);

                        if (_routingCriteria.OriginsContains(lastLegDestination) || _routingCriteria.DestinationsContains(lastLegDestination))
                        {
                            if (string.IsNullOrWhiteSpace(matchedOnList))
                            {
                                numLegsSelected = 0;
                            }
                        }

                        if (legSelected)
                        {
                            if (_routingCriteria.HasCountry)
                            {
                                var countryCode = LookupFunctions.LookupCountryCode(lastLegDestination, _masterStore);
                                if (_routingCriteria.OriginCountriesContains(countryCode) || _routingCriteria.DestinationCountriesContains(countryCode))
                                {
                                    if (string.IsNullOrWhiteSpace(matchedOnList))
                                    {
                                        numLegsSelected = 0;
                                    }
                                }
                            }
                        }

                        if (legSelected)
                        {
                            if (_routingCriteria.HasCountry)
                            {
                                var regionCode = LookupFunctions.LookupRegionCode(lastLegDestination, lastLegMode, _masterStore);
                                if (_routingCriteria.OriginRegionsContains(regionCode) || _routingCriteria.DestinationRegionsContains(regionCode))
                                {
                                    if (string.IsNullOrWhiteSpace(matchedOnList))
                                    {
                                        numLegsSelected = 0;
                                    }
                                }
                            }
                        }
                    }

                    if (numLegsSelected == 0)
                    {
                        recToDelete.Add(trip);
                    }
                }
            }

            foreach (var rec in recToDelete)
            {
                _userDefinedParams.TripDataList.Remove(rec);
            }
        }
    }
}
