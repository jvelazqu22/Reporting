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
    public class OriginRouting<T> : IRouting<T> where T : class, IRouteWhere
    {
        private readonly WhereTextBuilder _whereTextBuilder = new WhereTextBuilder();

        private readonly IMasterDataStore _masterStore;

        private bool UseFirstOrigin { get; set; }

        private string FirstOriginText { get; set; }

        private bool ReturnAllLegs { get; set; }

        private IList<T> Data { get; set; }

        private IList<T> FirstOriginData { get; set; } = new List<T>();

        public OriginRouting(bool useFirstOrigin, string firstOriginText, bool returnAllLegs, IList<T> data, IMasterDataStore store)
        {
            _masterStore = store;
            UseFirstOrigin = useFirstOrigin;
            FirstOriginText = firstOriginText;
            ReturnAllLegs = returnAllLegs;
            Data = data;

            //only need the first origin data if we are using the first origin only filter
            if (useFirstOrigin) FirstOriginData = data.Where(x => x.SeqNo == 1).ToList();
        }

        public WhereRouteData<T> GetAirportData(ReportGlobals globals, IFactory<string> displayNameFactory, IFactory<RoutingCriteria> routingFactory)
        {
            var routedData = new WhereRouteData<T> { FiltersApplied = false };
            var whereRoute = new WhereRouteApplier<T>();
            
            var routing = routingFactory.Build();
            
            var notInAirportList = globals.IsParmValueOn(WhereCriteria.NOTINORIGIN);

            var pl = new PickListParms(globals);
            pl.ProcessList(routing.Origin, string.Empty, "AIRPORT");

            if (pl.PickList.Any())
            {
                routedData.FiltersApplied = true;
                var displayName = displayNameFactory.Build();

                if (UseFirstOrigin)
                {
                    routedData.Data = whereRoute.GetDataBasedOnOriginCriteria(FirstOriginData, pl.PickList, notInAirportList, ReturnAllLegs).ToList();

                    globals.WhereText = _whereTextBuilder.AddToWhereText(globals.WhereText, pl.PickName + FirstOriginText, displayName,
                        notInAirportList, pl.PickListString);
                }
                else
                {
                    routedData.Data = whereRoute.GetDataBasedOnOriginCriteria(Data, pl.PickList, notInAirportList, ReturnAllLegs).ToList();

                    globals.WhereText = _whereTextBuilder.AddToWhereText(globals.WhereText, pl.PickName, displayName, notInAirportList,
                        pl.PickListString);
                }
            }

            return routedData;
        }

        public WhereRouteData<T> GetMetroData(ReportGlobals globals, IFactory<string> displayNameFactory, IFactory<RoutingCriteria> routingFactory)
        {
            var whereRoute = new WhereRouteApplier<T>();
            var routedData = new WhereRouteData<T> { FiltersApplied = false };
            
            var notInMetroOrgList = globals.IsParmValueOn(WhereCriteria.NOTINMETROORGS);

            var routing = routingFactory.Build();

            var pl = new PickListParms(globals);
            pl.ProcessList(routing.Origin, string.Empty, "METRO");

            if (pl.PickList.Any()) //Metro
            {
                routedData.FiltersApplied = true;
                var displayName = displayNameFactory.Build();

                var metroPorts = LookupFunctions.LookupAirportRailroadCombo(pl.PickList, LookupFunctions.OrgDestType.Metro, _masterStore);

                if (UseFirstOrigin)
                {
                    routedData.Data = whereRoute.GetDataBasedOnOriginAndModeCriteria(FirstOriginData, metroPorts, notInMetroOrgList, ReturnAllLegs).ToList();

                    globals.WhereText = _whereTextBuilder.AddToWhereText(globals.WhereText, pl.PickName + FirstOriginText, displayName,
                        notInMetroOrgList, pl.PickListString);
                }
                else
                {
                    routedData.Data = whereRoute.GetDataBasedOnOriginAndModeCriteria(Data, metroPorts, notInMetroOrgList, ReturnAllLegs).ToList();

                    globals.WhereText = _whereTextBuilder.AddToWhereText(globals.WhereText, pl.PickName, displayName,
                        notInMetroOrgList, pl.PickListString);
                }
            }

            return routedData;
        }

        public WhereRouteData<T> GetCountryData(ReportGlobals globals, IFactory<string> displayNameFactory, IFactory<RoutingCriteria> routingFactory)
        {
            var routedData = new WhereRouteData<T> { FiltersApplied = false };
            var whereRoute = new WhereRouteApplier<T>();
            
            var notInCountryOrgList = globals.IsParmValueOn(WhereCriteria.NOTINORIGCOUNTRY);

            var routing = routingFactory.Build();

            var pl = new PickListParms(globals);
            pl.ProcessList(routing.Origin, string.Empty, "COUNTRY");

            if (pl.PickList.Any())
            {
                routedData.FiltersApplied = true;
                var displayName = displayNameFactory.Build();
                var countryPorts = LookupFunctions.LookupAirportRailroadCombo(pl.PickList, LookupFunctions.OrgDestType.Country, _masterStore);

                if (UseFirstOrigin)
                {
                    routedData.Data = whereRoute.GetDataBasedOnOriginAndModeCriteria(FirstOriginData, countryPorts, notInCountryOrgList, ReturnAllLegs).ToList();

                    globals.WhereText = _whereTextBuilder.AddToWhereText(globals.WhereText, pl.PickName + FirstOriginText, displayName,
                        notInCountryOrgList, pl.PickListString);
                }
                else
                {
                    routedData.Data = whereRoute.GetDataBasedOnOriginAndModeCriteria(Data, countryPorts, notInCountryOrgList, ReturnAllLegs).ToList();

                    globals.WhereText = _whereTextBuilder.AddToWhereText(globals.WhereText, pl.PickName, displayName,
                        notInCountryOrgList, pl.PickListString);
                }
            }

            return routedData;
        }

        public WhereRouteData<T> GetRegionData(ReportGlobals globals, IFactory<string> displayNameFactory, IFactory<RoutingCriteria> routingFactory)
        {
            var routedData = new WhereRouteData<T> { FiltersApplied = false };
            var whereRoute = new WhereRouteApplier<T>();

            var notInRegionOrgList = globals.IsParmValueOn(WhereCriteria.NOTINORIGREGION);
            var routing = routingFactory.Build();

            var pl = new PickListParms(globals);
            pl.ProcessList(routing.Origin, string.Empty, "REGION");

            if (pl.PickList.Any())
            {
                routedData.FiltersApplied = true;
                var displayName = displayNameFactory.Build();
                var regionPorts = LookupFunctions.LookupAirportRailroadCombo(pl.PickList, LookupFunctions.OrgDestType.Region, _masterStore);

                if (UseFirstOrigin)
                {
                    routedData.Data = whereRoute.GetDataBasedOnOriginAndModeCriteria(FirstOriginData, regionPorts, notInRegionOrgList, ReturnAllLegs).ToList();

                    globals.WhereText = _whereTextBuilder.AddToWhereText(globals.WhereText, pl.PickName + FirstOriginText, displayName,
                        notInRegionOrgList, pl.PickListString);
                }
                else
                {
                    routedData.Data = whereRoute.GetDataBasedOnOriginAndModeCriteria(Data, regionPorts, notInRegionOrgList, ReturnAllLegs).ToList();

                    globals.WhereText = _whereTextBuilder.AddToWhereText(globals.WhereText, pl.PickName, displayName,
                        notInRegionOrgList, pl.PickListString);
                }
            }

            return routedData;
        }
    }
}
