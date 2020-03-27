using System.Collections.Generic;
using System.Linq;

using Domain.Helper;
using Domain.Interfaces;

using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using iBank.Services.Implementation.Shared.WhereRoute.Helpers;

using iBankDomain.Interfaces;

namespace iBank.Services.Implementation.Shared.WhereRoute.RouteFilters
{
    public class DestinationRouting<T> : IRouting<T> where T : class, IRouteWhere
    {
        private readonly WhereTextBuilder _whereTextBuilder = new WhereTextBuilder();

        private readonly IMasterDataStore _masterStore;

        private bool UseFirstDestination { get; set; }

        private string FirstDestinationText { get; set; }

        private bool ReturnAllLegs { get; set; }

        private IList<T> Data { get; set; }

        private IList<T> FirstDestinationData { get; set; } = new List<T>();

        public DestinationRouting(bool useFirstOrigin, string firstDestinationText, bool returnAllLegs, IList<T> data, IMasterDataStore store)
        {
            _masterStore = store;
            UseFirstDestination = useFirstOrigin;
            FirstDestinationText = firstDestinationText;
            ReturnAllLegs = returnAllLegs;
            Data = data;

            if (UseFirstDestination) FirstDestinationData = data.Where(x => x.SeqNo == 1).ToList();
        }

        public WhereRouteData<T> GetAirportData(ReportGlobals globals, IFactory<string> displayNameFactory, IFactory<RoutingCriteria> routingFactory)
        {
            var routedData = new WhereRouteData<T> { FiltersApplied = false };
            var whereRoute = new WhereRouteApplier<T>();
            
            var notInDestList = globals.IsParmValueOn(WhereCriteria.NOTINDESTS);
            var routing = routingFactory.Build();

            var pl = new PickListParms(globals);
            pl.ProcessList(routing.Destination, string.Empty, "AIRPORTS");
            if (pl.PickList.Any())
            {
                routedData.FiltersApplied = true;
                var displayName = displayNameFactory.Build();

                if (UseFirstDestination)
                {
                    routedData.Data = whereRoute.GetDataBasedOnDestinationCriteria(FirstDestinationData, pl.PickList, notInDestList, ReturnAllLegs).ToList();

                    globals.WhereText = _whereTextBuilder.AddToWhereText(globals.WhereText, pl.PickName + FirstDestinationText, displayName,
                        notInDestList, pl.PickListString);
                }
                else
                {
                    routedData.Data = whereRoute.GetDataBasedOnDestinationCriteria(Data, pl.PickList, notInDestList, ReturnAllLegs).ToList();

                    globals.WhereText = _whereTextBuilder.AddToWhereText(globals.WhereText, pl.PickName, displayName,
                        notInDestList, pl.PickListString);
                }
            }

            return routedData;
        }

        public WhereRouteData<T> GetMetroData(ReportGlobals globals, IFactory<string> displayNameFactory, IFactory<RoutingCriteria> routingFactory)
        {
            var whereRoute = new WhereRouteApplier<T>();
            var routedData = new WhereRouteData<T> { FiltersApplied = false };
            
            var notInMetroDestlist = globals.IsParmValueOn(WhereCriteria.NOTINMETRODESTS);
            var routing = routingFactory.Build();

            var pl = new PickListParms(globals);
            pl.ProcessList(routing.Destination, string.Empty, "METRO");
            if (pl.PickList.Any()) 
            {
                routedData.FiltersApplied = true;
                var displayName = displayNameFactory.Build();
                var metroPorts = LookupFunctions.LookupAirportRailroadCombo(pl.PickList, LookupFunctions.OrgDestType.Metro, _masterStore);

                if (UseFirstDestination)
                {
                    routedData.Data = whereRoute.GetDataBasedOnDestinationAndModeCriteria(FirstDestinationData, metroPorts, notInMetroDestlist, ReturnAllLegs).ToList();

                    globals.WhereText = _whereTextBuilder.AddToWhereText(globals.WhereText, pl.PickName + FirstDestinationText, displayName,
                        notInMetroDestlist, pl.PickListString);
                }
                else
                {
                    routedData.Data = whereRoute.GetDataBasedOnDestinationAndModeCriteria(Data, metroPorts, notInMetroDestlist, ReturnAllLegs).ToList();

                    globals.WhereText = _whereTextBuilder.AddToWhereText(globals.WhereText, pl.PickName, displayName,
                        notInMetroDestlist, pl.PickListString);
                }
            }

            return routedData;
        }

        public WhereRouteData<T> GetCountryData(ReportGlobals globals, IFactory<string> displayNameFactory, IFactory<RoutingCriteria> routingFactory)
        {
            var routedData = new WhereRouteData<T> { FiltersApplied = false };
            var whereRoute = new WhereRouteApplier<T>();

            var notInDestCountryList = globals.IsParmValueOn(WhereCriteria.NOTINDESTCOUNTRY);
            var routing = routingFactory.Build();

            var pl = new PickListParms(globals);
            pl.ProcessList(routing.Destination, string.Empty, "COUNTRY");
            if (pl.PickList.Any())
            {
                routedData.FiltersApplied = true;
                var displayName = displayNameFactory.Build();
                var countryPorts = LookupFunctions.LookupAirportRailroadCombo(pl.PickList, LookupFunctions.OrgDestType.Country, _masterStore);

                if (UseFirstDestination)
                {
                    routedData.Data = whereRoute.GetDataBasedOnDestinationAndModeCriteria(FirstDestinationData, countryPorts, notInDestCountryList, ReturnAllLegs).ToList();

                    globals.WhereText = _whereTextBuilder.AddToWhereText(globals.WhereText, pl.PickName + FirstDestinationText, displayName,
                        notInDestCountryList, pl.PickListString);
                }
                else
                {
                    routedData.Data = whereRoute.GetDataBasedOnDestinationAndModeCriteria(Data, countryPorts, notInDestCountryList, ReturnAllLegs).ToList();

                    globals.WhereText = _whereTextBuilder.AddToWhereText(globals.WhereText, pl.PickName, displayName,
                        notInDestCountryList, pl.PickListString);
                }
            }

            return routedData;
        }

        public WhereRouteData<T> GetRegionData(ReportGlobals globals, IFactory<string> displayNameFactory, IFactory<RoutingCriteria> routingFactory)
        {
            var routedData = new WhereRouteData<T> { FiltersApplied = false };
            var whereRoute = new WhereRouteApplier<T>();
            
            var notInDestRegionList = globals.IsParmValueOn(WhereCriteria.NOTINDESTREGION);
            var routing = routingFactory.Build();

            var pl = new PickListParms(globals);
            pl.ProcessList(routing.Destination, string.Empty, "REGION");
            if (pl.PickList.Any())
            {
                routedData.FiltersApplied = true;
                var displayName = displayNameFactory.Build();
                var regionPorts = LookupFunctions.LookupAirportRailroadCombo(pl.PickList, LookupFunctions.OrgDestType.Region, _masterStore);

                if (UseFirstDestination)
                {
                    routedData.Data = whereRoute.GetDataBasedOnDestinationAndModeCriteria(FirstDestinationData, regionPorts, notInDestRegionList, ReturnAllLegs).ToList();

                    globals.WhereText = _whereTextBuilder.AddToWhereText(globals.WhereText, pl.PickName + FirstDestinationText, displayName,
                        notInDestRegionList, pl.PickListString);
                }
                else
                {
                    routedData.Data = whereRoute.GetDataBasedOnDestinationAndModeCriteria(Data, regionPorts, notInDestRegionList, ReturnAllLegs).ToList();

                    globals.WhereText = _whereTextBuilder.AddToWhereText(globals.WhereText, pl.PickName, displayName,
                        notInDestRegionList, pl.PickListString);
                }
            }
        
            return routedData;
        }
    }
}
