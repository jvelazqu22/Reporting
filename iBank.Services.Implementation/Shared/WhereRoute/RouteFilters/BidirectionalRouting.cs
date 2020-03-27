using System.Collections.Generic;
using System.Linq;

using Domain.Interfaces;

using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using iBank.Services.Implementation.Shared.WhereRoute.Helpers;

using iBankDomain.Interfaces;

namespace iBank.Services.Implementation.Shared.WhereRoute.RouteFilters
{
    public class BidirectionalRouting<T> : IRouting<T> where T : class, IRouteWhere
    {
        private readonly WhereTextBuilder _whereTextBuilder = new WhereTextBuilder();

        private readonly IMasterDataStore _masterStore;

        private IList<T> Data { get; set; }

        private bool ReturnAllLegs { get; set; }

        public BidirectionalRouting(IList<T> data, bool returnAllLegs, IMasterDataStore store)
        {
            Data = data;
            ReturnAllLegs = returnAllLegs;
            _masterStore = store;
        }

        public WhereRouteData<T> GetAirportData(ReportGlobals globals, IFactory<string> displayNameFactory, IFactory<RoutingCriteria> routingFactory)
        {
            var routedData = new WhereRouteData<T> { FiltersApplied = false };
            
            var routing = routingFactory.Build();
            var displayName = displayNameFactory.Build();

            var pl = new PickListParms(globals);

            if (routing.IsOnlyOneWayCriteriaSpecified)
            {
                routedData.FiltersApplied = true;
                var xorCrit = new OnlyOriginOrDestinationCriteria();
                routedData.Data = xorCrit.HandleOnlyOriginOrDestSpecifiedInWhereRoute(Data.ToList(), routing.Origin, routing.Destination, pl, displayName, ReturnAllLegs, globals);
            }
            else if (routing.AreBothWaysCriteriaSpecified)
            {
                routedData.FiltersApplied = true;
                var bothCrit = new BothOriginAndDestCriteria();
                routedData.Data = bothCrit.HandleBothOriginAndDestSpecifiedInWhereRoute(Data.ToList(), pl, routing.Origin, routing.Destination, displayName, ReturnAllLegs, globals);
            }

            return routedData;
        }

        public WhereRouteData<T> GetMetroData(ReportGlobals globals, IFactory<string> displayNameFactory, IFactory<RoutingCriteria> routingFactory)
        {
            var whereRoute = new WhereRouteApplier<T>();
            var routedData = new WhereRouteData<T> { FiltersApplied = false };
            
            var routing = routingFactory.Build();
            var displayName = displayNameFactory.Build();

            var pl = new PickListParms(globals);

            if (routing.IsOnlyOneWayCriteriaSpecified)
            {
                var placeList = routing.Origin + routing.Destination;

                pl.ProcessList(placeList, string.Empty, "METRO");
                
                if (pl.PickList.Any())
                {
                    routedData.FiltersApplied = true;
                    var notIn = NotInDecider.IsMetroNotInSet(routing, globals);

                    var metroPorts = LookupFunctions.LookupAirportRailroadCombo(pl.PickList, LookupFunctions.OrgDestType.Metro, _masterStore);

                    routedData.Data = whereRoute.GetDataBasedOnOriginOrDestinationPlusModeCriteria(Data, metroPorts, notIn, ReturnAllLegs).ToList();
                    globals.WhereText = _whereTextBuilder.AddToWhereText(globals.WhereText, pl.PickName, displayName, notIn, placeList);
                }
            }
            else if (routing.AreBothWaysCriteriaSpecified)
            {
                pl.ProcessList(routing.Origin, string.Empty, "METRO");
                var originPickList = pl.PickList;
                var originPickName = pl.PickName;

                pl.ProcessList(routing.Destination, string.Empty, "METRO");
                var destinationPickList = pl.PickList;
                var destinationPickName = pl.PickName;
                
                var pickName = GetPicklistName(originPickName, destinationPickName);
                var notIn = NotInDecider.IsMetroNotInSet(routing, globals);

                var metroPortsOrg = LookupFunctions.LookupAirportRailroadCombo(originPickList, LookupFunctions.OrgDestType.Metro, _masterStore);
                var metroPortsDest = LookupFunctions.LookupAirportRailroadCombo(destinationPickList, LookupFunctions.OrgDestType.Metro, _masterStore);

                if (metroPortsOrg.Any() || metroPortsDest.Any())
                {
                    routedData.FiltersApplied = true;
                }

                routedData.Data = whereRoute.GetDataBasedOnOriginAndDestinationPlusModeCriteria(Data, metroPortsOrg, metroPortsDest, notIn, ReturnAllLegs).ToList();

                globals.WhereText = _whereTextBuilder.AddToWhereText(globals.WhereText, pickName, displayName, notIn, $"'{routing.Origin}', '{routing.Destination}'");
            }

            return routedData;
        }
        
        public WhereRouteData<T> GetCountryData(ReportGlobals globals, IFactory<string> displayNameFactory, IFactory<RoutingCriteria> routingFactory)
        {
            var whereRoute = new WhereRouteApplier<T>();
            var routedData = new WhereRouteData<T> { FiltersApplied = false };
            
            var routing = routingFactory.Build();
            var pl = new PickListParms(globals);
            
            var displayName = displayNameFactory.Build();

            var notIn = NotInDecider.IsCountryNotInSet(routing, globals);

            if (routing.IsOnlyOneWayCriteriaSpecified)
            {
                var placeList = routing.Origin + routing.Destination;
                pl.ProcessList(placeList, "", "COUNTRY");

                if (pl.PickList.Any())
                {
                    routedData.FiltersApplied = true;
                    var countryPorts = LookupFunctions.LookupAirportRailroadCombo(pl.PickList, LookupFunctions.OrgDestType.Country, _masterStore);

                    routedData.Data = whereRoute.GetDataBasedOnOriginOrDestinationPlusModeCriteria(Data, countryPorts, notIn, ReturnAllLegs).ToList();

                    globals.WhereText = _whereTextBuilder.AddToWhereText(globals.WhereText, pl.PickName, displayName, notIn, placeList);
                }

            }
            else if (routing.AreBothWaysCriteriaSpecified)
            {
                pl.ProcessList(routing.Origin, string.Empty, "COUNTRY");
                var orgPickList = pl.PickList;
                var orgPickName = pl.PickName;

                pl.ProcessList(routing.Destination, string.Empty, "COUNTRY");
                var destPickList = pl.PickList;
                var destPickName = pl.PickName;

                var pickName = GetPicklistName(orgPickName, destPickName);

                var countryPortsOrg = LookupFunctions.LookupAirportRailroadCombo(orgPickList, LookupFunctions.OrgDestType.Country, _masterStore);
                var countryPortsDest = LookupFunctions.LookupAirportRailroadCombo(destPickList, LookupFunctions.OrgDestType.Country, _masterStore);
                
                if (countryPortsOrg.Any() || countryPortsDest.Any())
                {
                    routedData.FiltersApplied = true;
                }

                routedData.Data = whereRoute.GetDataBasedOnOriginAndDestinationPlusModeCriteria(Data, countryPortsOrg, countryPortsDest, notIn, ReturnAllLegs);

                globals.WhereText = _whereTextBuilder.AddToWhereText(globals.WhereText, pickName, displayName, notIn, $"{routing.Origin}, {routing.Destination}");
            }

            return routedData;
        }

        public WhereRouteData<T> GetRegionData(ReportGlobals globals, IFactory<string> displayNameFactory, IFactory<RoutingCriteria> routingFactory)
        {
            var whereRoute = new WhereRouteApplier<T>();
            var routedData = new WhereRouteData<T> { FiltersApplied = false };
            
            var routing = routingFactory.Build();
            var pl = new PickListParms(globals);
            
            var displayName = displayNameFactory.Build();

            var notIn = NotInDecider.IsRegionNotInSet(routing, globals);

            if (routing.IsOnlyOneWayCriteriaSpecified)
            {
                var placeList = routing.Origin + routing.Destination;
                pl.ProcessList(placeList, string.Empty, "REGION");
                
                if (pl.PickList.Any())
                {
                    routedData.FiltersApplied = true;
                    var regionPorts = LookupFunctions.LookupAirportRailroadCombo(pl.PickList, LookupFunctions.OrgDestType.Region, _masterStore);

                    routedData.Data = whereRoute.GetDataBasedOnOriginOrDestinationPlusModeCriteria(Data, regionPorts, notIn, ReturnAllLegs);

                    globals.WhereText = _whereTextBuilder.AddToWhereText(globals.WhereText, pl.PickName, displayName, notIn, placeList);
                }
            }
            else if (routing.AreBothWaysCriteriaSpecified)
            {
                pl.ProcessList(routing.Origin, string.Empty, "REGION");
                var orgPickList = pl.PickList;
                var orgPickName = pl.PickName;

                pl.ProcessList(routing.Destination, string.Empty, "REGION");
                var destPickList = pl.PickList;
                var destPickName = pl.PickName;

                var pickName = GetPicklistName(orgPickName, destPickName);

                var regionPortsOrg = LookupFunctions.LookupAirportRailroadCombo(orgPickList, LookupFunctions.OrgDestType.Region, _masterStore);
                var regionPortsDest = LookupFunctions.LookupAirportRailroadCombo(destPickList, LookupFunctions.OrgDestType.Region, _masterStore);

                if (regionPortsOrg.Any() || regionPortsDest.Any())
                {
                    routedData.FiltersApplied = true;
                }

                routedData.Data = whereRoute.GetDataBasedOnOriginAndDestinationPlusModeCriteria(Data, regionPortsOrg, regionPortsDest, notIn, ReturnAllLegs).ToList();

                globals.WhereText = _whereTextBuilder.AddToWhereText(globals.WhereText, pickName, displayName, notIn, $"{routing.Origin}, {routing.Destination}");
            }

            return routedData;
        }

        private string GetPicklistName(string originPickName, string destinationPickName)
        {
            if (string.IsNullOrEmpty(originPickName) || string.IsNullOrEmpty(destinationPickName))
            {
                return originPickName + destinationPickName;
            }
            else
            {
                return $"{originPickName}, {destinationPickName}";
            }
        }
    }
}
