using Domain.Interfaces;

using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared.WhereRoute.Helpers;

using iBankDomain.Interfaces;

namespace iBank.Services.Implementation.Shared.WhereRoute.RouteFilters
{
    public interface IRouting<T> where T : class, IRouteWhere
    {
        WhereRouteData<T> GetAirportData(ReportGlobals globals, IFactory<string> displayNameFactory, IFactory<RoutingCriteria> routingFactory);

        WhereRouteData<T> GetMetroData(ReportGlobals globals, IFactory<string> displayNameFactory, IFactory<RoutingCriteria> routingFactory);

        WhereRouteData<T> GetCountryData(ReportGlobals globals, IFactory<string> displayNameFactory, IFactory<RoutingCriteria> routingFactory);

        WhereRouteData<T> GetRegionData(ReportGlobals globals, IFactory<string> displayNameFactory, IFactory<RoutingCriteria> routingFactory);
    }
}