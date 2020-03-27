using System.Collections.Generic;
using System.Linq;
using Domain;
using Domain.Helper;
using Domain.Interfaces;

using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared.WhereRoute.Helpers;
using iBank.Services.Implementation.Shared.WhereRoute.RouteFilters;

namespace iBank.Services.Implementation.Shared.WhereRoute
{
    public class RoutingProcessor<T> where T : class, IRouteWhere
    {
        public ReportGlobals ReportGlobals { get; set; }

        private readonly IMasterDataStore _masterStore;

        public RoutingProcessor(ReportGlobals globals, IMasterDataStore store)
        {
            ReportGlobals = globals;
            _masterStore = store;
        }

        public IList<T> ProcessRoutingCriteria(IList<T> rawData, bool returnAllLegs, bool isRoutingBidirectional, bool ignoreTravel)
        {
            rawData = FixOriginAndDestinationCodes(rawData).ToList();
            
            //Handle sections of RouteWhere that were built in the Date section. 
            var dateRangeFilter = new DateRangeRouting<T>();
            rawData = dateRangeFilter.GetDataFilteredOnDateRange(rawData, ReportGlobals, ignoreTravel);

            if (ReportGlobals.LegDIT)
            {
                var flightTypeFilter = new FlightTypeFilter<T>();
                rawData = flightTypeFilter.GetDataFilteredOnDomesticInternationalCode(rawData, ReportGlobals);
            }

            var combiner = new Combiner<T>();
            List<WhereRouteData<T>> filteredData;
            if (isRoutingBidirectional)
            {
                filteredData = GetBidirectionalFilteredData(rawData, returnAllLegs).ToList();
            }
            else
            {
                var useFirstOrigin = ReportGlobals.IsParmValueOn(WhereCriteria.FIRSTORIGIN);
                var firstOriginText = FirstOriginDestinationTextBuilder.BuildText(useFirstOrigin, ReportGlobals.LanguageVariables);
                var originFilteredData = GetOriginFilteredData(rawData, returnAllLegs, useFirstOrigin, firstOriginText);
                
                filteredData = JoinFilteredData(rawData, originFilteredData, useFirstOrigin);

                var useFirstDestination = ReportGlobals.IsParmValueOn(WhereCriteria.FIRSTDEST);
                var firstDestinationText = FirstOriginDestinationTextBuilder.BuildText(useFirstDestination, ReportGlobals.LanguageVariables);
                var destinationFilteredData = GetDestinationFilteredData(rawData, returnAllLegs, useFirstDestination, firstDestinationText);
                destinationFilteredData = JoinFilteredData(rawData, destinationFilteredData, useFirstDestination);

                filteredData.AddRange(destinationFilteredData);
            }
            
            var airlineRouting = new AirlineRouting<T>();
            var airlineData = airlineRouting.GetAirlineFilteredData(rawData, ReportGlobals);
            filteredData.Add(airlineData);

            if (!filteredData.Any(x => x.FiltersApplied)) return rawData;

            return returnAllLegs && !(ReportGlobals.AdvancedParameters.AndOr.Equals(AndOr.And))
                ? combiner.GetAllLegsInTrip(rawData, filteredData)
                : combiner.JoinMatchingData(rawData, filteredData).ToList();
        }

        private List<WhereRouteData<T>> JoinFilteredData(IList<T> rawData, IList<WhereRouteData<T>> filteredData, bool applyCriteriaToSpecificLeg)
        {
            if (applyCriteriaToSpecificLeg && filteredData.Any(x => x.FiltersApplied))
            {
                //if we are using first origin or first destination then we are saying that all the filters must be true for the origin/destination leg
                var combiner = new Combiner<T>();
                var joinedData = combiner.JoinMatchingData(rawData, filteredData);
                return new List<WhereRouteData<T>> { new WhereRouteData<T> { Data = joinedData.ToList(), FiltersApplied = true } };
            }
            else
            {
                return filteredData.ToList();
            }
        }

        private IList<T> FixOriginAndDestinationCodes(IList<T> rawData)
        {
            //make sure rawData origin and dest codes are the same for data that comes from iblegs or ibmktsegs tables
            foreach (var raw in rawData)
            {
                raw.Origin = ReportHelper.CreateOriginDestCode(raw.Origin, raw.Mode, raw.Airline);
                raw.Destinat = ReportHelper.CreateOriginDestCode(raw.Destinat, raw.Mode, raw.Airline);
            }

            return rawData;
        }

        private List<WhereRouteData<T>> GetBidirectionalFilteredData(IList<T> rawData, bool returnAllLegs)
        {
            var biDirectionalRouter = new BidirectionalRouting<T>(rawData, returnAllLegs, _masterStore);

            var displayNameFactory = new BidirectionalDisplayNameFactory(RoutingCriteriaType.Airport, _masterStore, ReportGlobals.UserLanguage);
            var routingFactory = new RoutingCriteriaFactory(RoutingCriteriaType.Airport, ReportGlobals);
            var airportData = biDirectionalRouter.GetAirportData(ReportGlobals, displayNameFactory, routingFactory);

            displayNameFactory.Type = RoutingCriteriaType.Metro;
            routingFactory.Type = RoutingCriteriaType.Metro;
            var metroData = biDirectionalRouter.GetMetroData(ReportGlobals, displayNameFactory, routingFactory);

            displayNameFactory.Type = RoutingCriteriaType.Country;
            routingFactory.Type = RoutingCriteriaType.Country;
            var countryData = biDirectionalRouter.GetCountryData(ReportGlobals, displayNameFactory, routingFactory);

            displayNameFactory.Type = RoutingCriteriaType.Region;
            routingFactory.Type = RoutingCriteriaType.Region;
            var regionData = biDirectionalRouter.GetRegionData(ReportGlobals, displayNameFactory, routingFactory);
            
            return new List<WhereRouteData<T>> { airportData, metroData, countryData, regionData };
        }

        private List<WhereRouteData<T>> GetOriginFilteredData(IList<T> rawData, bool returnAllLegs, bool useFirstOrigin, string firstOriginText)
        {
            var originRouter = new OriginRouting<T>(useFirstOrigin, firstOriginText, returnAllLegs, rawData, _masterStore);

            var originDisplayNameFactory = new OriginDisplayNameFactory(RoutingCriteriaType.Airport, _masterStore, ReportGlobals.UserLanguage);
            var routingFactory = new RoutingCriteriaFactory(RoutingCriteriaType.Airport, ReportGlobals);
            var airportOriginData = originRouter.GetAirportData(ReportGlobals, originDisplayNameFactory, routingFactory);

            originDisplayNameFactory.Type = RoutingCriteriaType.Metro;
            routingFactory.Type = RoutingCriteriaType.Metro;
            var metroOriginData = originRouter.GetMetroData(ReportGlobals, originDisplayNameFactory, routingFactory);

            originDisplayNameFactory.Type = RoutingCriteriaType.Country;
            routingFactory.Type = RoutingCriteriaType.Country;
            var countryOriginData = originRouter.GetCountryData(ReportGlobals, originDisplayNameFactory, routingFactory);

            originDisplayNameFactory.Type = RoutingCriteriaType.Region;
            routingFactory.Type = RoutingCriteriaType.Region;
            var regionOriginData = originRouter.GetRegionData(ReportGlobals, originDisplayNameFactory, routingFactory);

            return new List<WhereRouteData<T>> { airportOriginData, metroOriginData, countryOriginData, regionOriginData };
        }

        private List<WhereRouteData<T>> GetDestinationFilteredData(IList<T> rawData, bool returnAllLegs, bool useFirstDestination, string firstDestinationText)
        {
            var destinationRouter = new DestinationRouting<T>(useFirstDestination, firstDestinationText, returnAllLegs, rawData, _masterStore);

            var destinationDisplayNameFactory = new DestinationDisplayNameFactory(RoutingCriteriaType.Airport, _masterStore, ReportGlobals.UserLanguage);
            var routingFactory = new RoutingCriteriaFactory(RoutingCriteriaType.Airport, ReportGlobals);
            var airportDestinationData = destinationRouter.GetAirportData(ReportGlobals, destinationDisplayNameFactory, routingFactory);

            destinationDisplayNameFactory.Type = RoutingCriteriaType.Metro;
            routingFactory.Type = RoutingCriteriaType.Metro;
            var metroDestinationData = destinationRouter.GetMetroData(ReportGlobals, destinationDisplayNameFactory, routingFactory);

            destinationDisplayNameFactory.Type = RoutingCriteriaType.Country;
            routingFactory.Type = RoutingCriteriaType.Country;
            var countryDestinationData = destinationRouter.GetCountryData(ReportGlobals, destinationDisplayNameFactory, routingFactory);

            destinationDisplayNameFactory.Type = RoutingCriteriaType.Region;
            routingFactory.Type = RoutingCriteriaType.Region;
            var regionDestinationData = destinationRouter.GetRegionData(ReportGlobals, destinationDisplayNameFactory, routingFactory);

            return new List<WhereRouteData<T>> { airportDestinationData, metroDestinationData, countryDestinationData, regionDestinationData };
        }
    }
}
